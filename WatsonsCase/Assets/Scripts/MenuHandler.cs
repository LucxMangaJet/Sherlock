using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Holmes_Control;
using System.IO;

namespace Holmes_Menu
{
    public class MenuHandler : MonoBehaviour
    {
        [SerializeField] GameObject i_menu, pref_text,i_bottomBookPref,i_dialog, i_AddEvidenceVisualHelp, ui_VisualHelpAddEvidence;
        [SerializeField] Transform  i_mainCam;
        [SerializeField] RectTransform  i_BookBottomTransform,i_CharactersLogTransform;
        [SerializeField] RectTransform[] i_charButtons;
        [SerializeField] GameObject[] i_charObj;
       
        bool usable = true;
        bool inDialogue = false;
        int charAmount;
        
        SoundHandler sound;
        
        
        Character[] characters;
        Character selectedChar = null;
        public Character lastCharTalkedTo = null;
       
        DialogueOption selectedLogOption = null;
        int selectedLogOptionAnswerIndx = 0;

        List<GameObject> DialogLogObjs;


        //button anim
        bool[] charButtoninAnim;
        bool[] charButtonPannedOver;
        float[] charButtonStartX;

        public event DialogueHandler.EmptyDel enterMenu;
        public event DialogueHandler.EmptyDel exitMenu;

        //rotateVars
        float startRotX;
        bool inRotAnimation = false;

        GameState state;

        
        // Use this for initialization
        void Start()
        {
            sound = GetComponent<SoundHandler>();
            state = GetComponent<GameState>();

            charAmount = i_charButtons.Length;
            characters = new Character[charAmount];
            charButtoninAnim = new bool[charAmount];
            charButtonPannedOver = new bool[charAmount];
            charButtonStartX = new float[charAmount];
            DialogLogObjs = new List<GameObject>();

            for (int i = 0; i < charAmount; i++)
            {
                charButtonStartX[i] = i_charButtons[i].anchoredPosition.x;
                characters[i] = i_charObj[i].GetComponent<Character>();
            }



            GetComponent<DialogueHandler>().enterDialogueEvent += EnterDialogue;
            GetComponent<DialogueHandler>().exitDialogueEvent += ExitDialogue;
            GetComponent<EvidenceDetecting>().enterDrawing += Disable;
            GetComponent<EvidenceDetecting>().exitDrawing += Enable;
            GetComponent<CutsceneHandler>().CutSceneStartEvent += EnterCutScene;
            GetComponent<CutsceneHandler>().CutSceneEndEvent += ExitCutScene;
            SelectCharacter(0);
        }

        // Update is called once per frame
        void Update()
        {
            if ((Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.I)) && usable && !inRotAnimation){
                TurnMenuOnOff();
            }
               


            if (Input.GetKeyDown(KeyCode.F4))
            {
                state.Save(-1);
                Application.Quit();
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                state.Save(-1);
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                GetComponent<Holmes_Control.SaveGameHandler>().LoadSaveFile();
            }
        }

        /// <summary>
        /// /
        /// </summary>
        /// <returns>The cam enumerator.</returns>
        /// <param name="duration">Duration.</param>
        /// <param name="downAnim">If set to <c>true</c> down animation.</param>
        private IEnumerator RotateCamEnumerator(float duration, bool downAnim)
        {
            inRotAnimation = true;
            float t = duration;
            RectTransform tr = i_menu.GetComponent<RectTransform>();
            RectTransform td = null;
            if (downAnim)
            {
                startRotX = i_mainCam.transform.eulerAngles.x;
                tr.anchoredPosition = new Vector2(tr.anchoredPosition.x, -700);
            }

            float j = 700 * duration;


            while (t > 0)
            {
                Vector3 r = i_mainCam.transform.eulerAngles;
                i_mainCam.transform.eulerAngles = new Vector3(r.x + (downAnim ? 1 : -1), r.y, r.z);
                tr.anchoredPosition = new Vector2(tr.anchoredPosition.x, tr.anchoredPosition.y + (downAnim ? j * Time.deltaTime : -j * Time.deltaTime));
                td.anchoredPosition = new Vector2(td.anchoredPosition.x, td.anchoredPosition.y + (downAnim ? j * Time.deltaTime : -j * Time.deltaTime));
                yield return null;
                t -= Time.deltaTime;
            }

            if (exitMenu != null && !downAnim)
                exitMenu();
            if (!downAnim)
            {
                i_menu.SetActive(false);
                tr.anchoredPosition = new Vector2(tr.anchoredPosition.x, 700);
                td.anchoredPosition = new Vector2(td.anchoredPosition.x, 0);
                selectedChar = null;
                RefitDialogueLog();
                sound.PlayClip(SoundHandler.ClipEnum.UICloseBook, SoundHandler.OutputEnum.UI);
            }
            else
            {
                td.anchoredPosition = new Vector2(td.anchoredPosition.x, 700);
                tr.anchoredPosition = new Vector2(tr.anchoredPosition.x, 0);
            }
            inRotAnimation = false;
        }

        

        public void HighlightText(int i, int j)
        {
            if (i >= 0)
            {
                selectedLogOption = selectedChar.dialogueLog[i];
            }
            else
            {
                selectedLogOption = null;
            }

            selectedLogOptionAnswerIndx = j;
            RefitDialogueLog();
            //turn button on;

        }

        int GetTextIndxFromButtonIndx(int indx, bool end, string[] storage)
        {
            int counter = 0;
            for (int i = 0; i < indx + ((end) ? 1 : 0); i++)
            {
                counter += 1 + storage[i].Length;
            }

            return counter;
        }
        //Public Methods
        public void SaveSelectedLogSentence()
        {
            string[] answer = GetComponent<DialogueHandler>().DevideAnswerIntoSentences(selectedLogOption.answer);
            int selectionStartIndx = GetTextIndxFromButtonIndx(selectedLogOptionAnswerIndx, false, answer);
            int selectionEndIndx = GetTextIndxFromButtonIndx(selectedLogOptionAnswerIndx, true, answer);
            string content = answer[selectedLogOptionAnswerIndx];
            List<string> evidences = new List<string>();

            //		Debug.Log (selectionStartIndx + "  " + selectionEndIndx);
            if (selectedLogOption.evidences != null)
            {
                foreach (TextEvidence t in selectedLogOption.evidences)
                {
                    if (selectionStartIndx-3 <= t.startIndx && selectionEndIndx+3 >= t.endIndx)
                    {
                        evidences.Add(t.name);
                    }
                }
            }
            GetComponent<EvidenceHandler>().AddTextEvidence(new SavedTextEvidence(selectedChar.myName, content, evidences.ToArray()));
            HighlightText(-1, -1);
            int len = selectionEndIndx - selectionStartIndx;
            sound.PlayClip(len < 8 ? SoundHandler.ClipEnum.UIWritingShort : len < 20 ? SoundHandler.ClipEnum.UIWritingMid : SoundHandler.ClipEnum.UIWritingLong, SoundHandler.OutputEnum.UI);
        }

        public void TurnMenuOnOff()
        {
            bool b = !i_menu.activeSelf;

            if (b)
            {
                i_menu.SetActive(true);
                ui_VisualHelpAddEvidence.SetActive(true);
                SelectCharacter(lastCharTalkedTo);
                if (enterMenu != null)
                {
                    enterMenu();
                }
               // SelectCharacter(0);
                // StartCoroutine(RotateCamEnumerator(1, true));
                if (inDialogue)
                    i_dialog.SetActive(false);

                sound.PlayClip(SoundHandler.ClipEnum.UIOpenBook, SoundHandler.OutputEnum.UI);
            }
            else
            {
                // StartCoroutine(RotateCamEnumerator(1, false));
                i_menu.SetActive(false);
                selectedChar = null;
                RefitDialogueLog();
                if(!inDialogue)
                   ui_VisualHelpAddEvidence.SetActive(false);
                if (exitMenu != null)
                    exitMenu();
                if (inDialogue)
                    i_dialog.SetActive(true);
                sound.PlayClip(SoundHandler.ClipEnum.UICloseBook, SoundHandler.OutputEnum.UI);
            }

        }

        public void SelectCharacter(int i)
        {
            //Debug.Log(characters[i].myName);
            selectedChar = characters[i]; 

            HighlightText(-1, -1);
            
        }

        public void SelectCharacter(Character c)
        {
            if (c == null)
                SelectCharacter(0);

            selectedChar = lastCharTalkedTo;
            HighlightText(-1, -1);
        }

        public void RefitDialogueLog()
 	       {

            
			float heightText = -30;


            //float boxwidth = i_CharactersLogTransform.GetComponent<RectTransform>().sizeDelta.y-40;
            float boxwidth = 350;
            

            for (int i = 0; i < DialogLogObjs.Count; i++)
            {
                Destroy(DialogLogObjs[i]); 
            }
            DialogLogObjs.Clear();

            if (selectedChar != null)
            {
                //add char name
                GameObject charName = Instantiate(pref_text, i_CharactersLogTransform);
                DialogLogObjs.Add(charName);
                RectTransform charNameRect = charName.GetComponent<RectTransform>();
                charName.GetComponentInChildren<Text>().text = "<color=maroon><size=20>" + selectedChar.myName + "</size></color>";
                float widthName = 100;
                float heightName = 30;
                heightText -= 15;
                charNameRect.sizeDelta = new Vector2(widthName, heightName);
                charNameRect.anchoredPosition = new Vector2(widthName / 2 + 20, heightText);
                heightText -= 15;
                charName.GetComponent<DialogDragHandler>().enabled = false;

                // Debug.Log(selectedChar.dialogueLog.Count);
                for (int i = 0; i < selectedChar.dialogueLog.Count; i++)
                {
                    
                    string temp = selectedChar.dialogueLog[i].question;
                    GameObject go = Instantiate(pref_text, i_CharactersLogTransform);
                    DialogLogObjs.Add(go);
                    RectTransform rect1 = go.GetComponent<RectTransform>();
                    go.GetComponentInChildren<Text>().text = "<color=maroon>" + temp + "</color>";
                    float width1 = GetComponent<DialogueHandler>().GetWordWidth(temp);   
                    float height1 = Mathf.Ceil(width1 / boxwidth);
                    width1 = (height1 > 1) ? boxwidth : width1;
                    heightText -= (height1 / 2) * 18;
                    rect1.sizeDelta = new Vector2(width1, height1 * 18);
                    rect1.anchoredPosition = new Vector2(width1 / 2  +20, heightText);
                    heightText -= (height1 / 2) * 18;
                    go.GetComponent<DialogDragHandler>().enabled = false;

                    // string splitting process
                    string[] str = GetComponent<DialogueHandler>().DevideAnswerIntoSentences(selectedChar.dialogueLog[i].answer);
                    for (int j = 0; j < str.Length; j++)
                    {
                        



                        string s = str[j];
                        GameObject g = Instantiate(pref_text, i_CharactersLogTransform);
                        DialogLogObjs.Add(g);
                        //checking if contains evidences for easy mode
                        bool colored = false;
                        if (!state.HardMode)
                        {
                            int counter = 0;
                            for (int k = 0; k < j; k++)
                            {
                                counter += 1 + str[k].Length;
                            }
                            int selectionStartIndx = counter;
                            counter = 0;
                            for (int k = 0; k < j + 1; k++)
                            {
                                counter += 1 + str[k].Length;
                            }
                            int selectionEndIndx = counter;

                            if (selectedChar.dialogueLog[i].evidences != null)
                            {
                                foreach (TextEvidence t in selectedChar.dialogueLog[i].evidences)
                                {
                                    if (selectionStartIndx - 3 <= t.startIndx && selectionEndIndx + 3 >= t.endIndx)
                                    {
                                        colored = true;
                                    }
                                }
                            }
                        }

                        s = (colored ? "<i><b>" : "") + s + (colored ? "</b></i>" : "");
                        g.GetComponentInChildren<Text>().text = s;
                        RectTransform rect = g.GetComponent<RectTransform>();
                        float width = GetComponent<DialogueHandler>().GetWordWidth(s);
                        float height = Mathf.Ceil(width / boxwidth);
                        width = (height > 1) ? boxwidth : width;
                        heightText -= (height / 2) * 18;
                        rect.sizeDelta = new Vector2(width, height * 18);
                        rect.anchoredPosition = new Vector2(width / 2 +20, heightText);
                        heightText -= (height / 2) * 18;
                        int param = i; int param2 = j;
                        g.GetComponent<DialogDragHandler>().inMenu=true;
                        g.GetComponent<DialogDragHandler>().selectedIndx = param;
                        g.GetComponent<DialogDragHandler>().selectedIndx2 = param2;
                        //g.GetComponent<Button>().onClick.AddListener(delegate
                        //{
                        //    HighlightText(param, param2);
                        //});
                    }
                }
            }

            //display infinite book

            i_CharactersLogTransform.sizeDelta = new Vector2 (i_CharactersLogTransform.sizeDelta.x, -heightText);
			i_CharactersLogTransform.anchoredPosition = new Vector2 (0, 0);

            const float startPos= -275;
            const float tilableHeight = 350;
			float amount =  Mathf.Ceil(Mathf.Abs((heightText - 275) / tilableHeight)) +3;

            for (int i = 0; i < i_BookBottomTransform.childCount; i++)
            {
                Destroy(i_BookBottomTransform.GetChild(i).gameObject);
            }

            for (int i = 0; i <= amount; i++)
            {
                GameObject g = Instantiate(i_bottomBookPref, i_BookBottomTransform);
                RectTransform rt = g.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, startPos + i * tilableHeight * -1);

            }
        }


        //events and interactions functions

        public void EnterHover(int i)
        {
            charButtonPannedOver[i] = true;
            if (!charButtoninAnim[i])
            {
                charButtoninAnim[i] = true;
                StartCoroutine(ButtonHighlightAnim(i));
            }     
            
        }

        public void ExitHover(int i)
        {
            charButtonPannedOver[i] = false;
            if(!charButtoninAnim[i])
            {
                charButtoninAnim[i] = true;
                StartCoroutine(ButtonHighlightAnim(i));
            }
            
        }

        IEnumerator ButtonHighlightAnim(int i)
        {
            const float movX = 30;
            const float duration = 0.3f;

            float curtime = charButtonPannedOver[i]?0:duration;
            while (true)
            {
                curtime += Time.deltaTime * (charButtonPannedOver[i] ? 1 : -1);
                i_charButtons[i].anchoredPosition = new Vector2( Mathf.Lerp(charButtonStartX[i], charButtonStartX[i] + movX, curtime / duration),i_charButtons[i].anchoredPosition.y);

                if (charButtonPannedOver[i])
                {
                    if (curtime >= 0.3f)
                        break;
                }
                else
                {
                    if (curtime <= 0)
                        break;
                }
                yield return null;
            }
            charButtoninAnim[i] = false;

        }

        public void Enable()
        {
            usable = true;
        }

        public void Disable()
        {
            usable = false;
        }

        public void EnterDialogue()
        {
            inDialogue = true;
        }

        public void ExitDialogue()
        {
            inDialogue = false;
        }

        public void EnterCutScene(string s)
        {
            Disable();
        }

        public void ExitCutScene(string s)
        {
            Enable();
        }

    }
}

	[System.Serializable]
	public class SavedTextEvidence{
		public string text,autor;
		public string[] containedEvidencesName;
		public SavedTextEvidence(string _autor, string _text, string[] _evidencesName){
			autor = _autor;
			text = _text;
			containedEvidencesName = _evidencesName;
		}
	}

/// <summary>
/// The general class that is used to store Evidences
/// </summary>
[System.Serializable]
public class UnitedEvidence {
        public string[] autors= null;
        public string[] texts = null;
        public Sprite[] drawings =null;
        public string[] evidences=null;

        public UnitedEvidence(SavedTextEvidence[] _textEv, Drawing[] _drawings)
        {
        List<string> evidencesList = new List<string>();
        if (_textEv != null)
        {
            autors = new string[_textEv.Length];
            texts = new string[_textEv.Length];
            for (int i = 0; i < _textEv.Length; i++)
            {
                autors[i] = _textEv[i].autor;
                texts[i] = _textEv[i].text;

                foreach( string s in _textEv[i].containedEvidencesName)
                {
                    evidencesList.Add(s);
                }
            }
        }

        if(_drawings != null)
        {
            drawings = new Sprite[_drawings.Length];

            for (int i = 0; i < _drawings.Length; i++)
            {
                drawings[i] = _drawings[i].sprite;

                foreach(DrawingEvidence d in _drawings[i].evidences)
                {
                    evidencesList.Add(d.name);
                }

            }
        }
        evidences = evidencesList.ToArray();
        }

        public UnitedEvidence(UnitedEvidence[] _UnitedEvidence)
        {
        List<string> autorsList = new List<string>();
        List<string> textsList = new List<string>();
        List<Sprite> drawingsList = new List<Sprite>();
        List<string> evidencesList = new List<string>();


        for (int i = 0; i < _UnitedEvidence.Length; i++)
        {
            if (_UnitedEvidence[i].autors != null)
            {
                foreach (string s in _UnitedEvidence[i].autors)
                {
                    autorsList.Add(s);
                }
            }
            if (_UnitedEvidence[i].texts != null)
            {
                foreach (string s in _UnitedEvidence[i].texts)
                {
                    textsList.Add(s);
                }
            }
            if (_UnitedEvidence[i].drawings != null) {
                foreach (Sprite s in _UnitedEvidence[i].drawings)
                {
                    drawingsList.Add(s);
                }
            }
            if (_UnitedEvidence[i].evidences != null)
            {
                foreach (string s in _UnitedEvidence[i].evidences)
                {
                    evidencesList.Add(s);
                }
            }
        }

        autors = autorsList.ToArray();
        texts = textsList.ToArray();
        drawings = drawingsList.ToArray();
        evidences = evidencesList.ToArray();
    }

        public UnitedEvidence(string[] _autors, string[] _texts, Sprite[] _drawings, string[] _evidences)
        {
            autors = _autors;
            texts = _texts;
            drawings = _drawings;
            evidences = _evidences;
        }
            
    }
    
    [System.Serializable]
    public class UnitedEvidenceSerializable
{
    public string[] autors = null;
    public string[] texts = null;
    //public Vector3Serializable[][]Pixels;
    public float[] resWidth, resHeight;
    public string[] evidences = null;


    public UnitedEvidenceSerializable(UnitedEvidence e)
    {
        autors = e.autors;
        texts = e.texts;
        evidences = e.evidences;

        if (e.drawings != null)
        {
            resWidth = new float[e.drawings.Length];
            resHeight = new float[e.drawings.Length];


            for (int i = 0; i < e.drawings.Length; i++)
            {
                resWidth[i] = e.drawings[i].rect.width;
                resHeight[i] = e.drawings[i].rect.height;
            }


        }
        else
        {
            resHeight = null;
            resWidth = null;
            //Pixels = null;
        }
    }

    public static UnitedEvidenceSerializable Make(UnitedEvidence e)
    {
        return new UnitedEvidenceSerializable(e);
    }

    public static UnitedEvidence Read(UnitedEvidenceSerializable e,int evIndx)
    {
        if(e.resWidth==null)
            return new UnitedEvidence(e.autors, e.texts, null, e.evidences);

        Sprite[] drawings = new Sprite[e.resHeight.Length];
        //for (int i = 0; i < drawings.Length; i++)
        //{
        //    Texture2D t = new Texture2D((int)e.resWidth[i], (int)e.resWidth[i], TextureFormat.RGB24, false);
        //    Color[] col = new Color[e.Pixels[i].Length];

        //    for (int j = 0; j < e.Pixels[i].Length; j++)
        //    {
        //        col[j] = new Color(e.Pixels[i][j].x, e.Pixels[i][j].y, e.Pixels[i][j].z);
        //    }
        //    t.SetPixels(col);
        //    t.Apply();
        //    drawings[i] = Sprite.Create(t, new Rect(0, 0, (int)e.resWidth[i], e.resHeight[i]), new Vector2(0, 0));
        //}

        for(int i=0; i < e.resWidth.Length; i++)
        {
            byte[] FileData = File.ReadAllBytes(Application.persistentDataPath+"/" +evIndx+i);
            SaveGameHandler.EncryptDecryptBytes(ref FileData);
            Texture2D t = new Texture2D(1,1);
            t.LoadImage(FileData);
            
            if (t.height == 1)
                throw new UnityException("Unable to load Saved drawing");

            //Texture2D f = Texture2D.Instantiate(t);
            //Texture2D.Destroy(t);

            //Debug.Log(e.resHeight[i] + " " + e.resWidth[i]);
            drawings[i] = Sprite.Create(t, new Rect(0, 0,(int)e.resWidth[i],(int) e.resHeight[i]), new Vector2(0, 0));
            
        }

        return new UnitedEvidence(e.autors,e.texts,drawings,e.evidences);
    }
}



