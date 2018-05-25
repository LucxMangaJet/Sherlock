using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holmes_Menu{
    public class MenuHandler : MonoBehaviour
    {
        [SerializeField] GameObject i_menu, pref_drawing, pref_text, i_EvTextDelete, i_EvTextAskAbout,i_EvTextMerge,pref_CharAll,i_DialogueLogSaveSelection,i_EvDrawingDelete,i_EvDrawingAskAbout;
        [SerializeField] Transform i_pannelsObjTransform, i_mainCam;
        [SerializeField] RectTransform i_drawingsContentTransform, i_textEvidenceContentTransform, i_dialogueTransform, i_CharactersAllTransform, i_CharactersLogTransform;
        [SerializeField] List<Drawing> drawings;
        [SerializeField] List<SavedTextEvidence> evidences;
        bool usable = true;
        bool inDialogue = false;
        SoundHandler sound;

        //drawings Page
        int selectedDrawing=-1;

        //evidences Page
        List<SavedTextEvidence> selectedTextEvidences;
        List<int> selectedTextEvidencesIndx;
        //Characters Page
        Character[] characters;
        Character selectedChar = null;
        DialogueOption selectedLogOption = null;
        
        int  selectedLogOptionAnswerIndx=0;

        public event DialogueHandler.EmptyDel enterMenu;
        public event DialogueHandler.EmptyDel exitMenu;

        //rotateVars
        float startRotX;
        bool inRotAnimation = false;


        private GameObject[] m_pannels;
        private float startXdrawingsContent;
        // Use this for initialization
        void Start()
        {
            sound = GetComponent<SoundHandler>();
            evidences = new List<SavedTextEvidence>();
            drawings = new List<Drawing>();
            selectedTextEvidences = new List<SavedTextEvidence>();
            selectedTextEvidencesIndx = new List<int>();
            i_menu.SetActive(false);
            startXdrawingsContent = i_drawingsContentTransform.position.x;

            int temp = i_pannelsObjTransform.childCount;
            m_pannels = new GameObject[temp];
            for (int i = 0; i < temp; i++)
            {
                m_pannels[i] = i_pannelsObjTransform.GetChild(i).gameObject;
                m_pannels[i].SetActive(false);
            }
            ChangePannel(0); //default Pannel

            //characters tab
            GameObject[] g = GameObject.FindGameObjectsWithTag("Character");
            characters = new Character[g.Length];
            for (int i = 0; i < g.Length; i++)
            {
                characters[i] = g[i].GetComponent<Character>();
            }
            RefitAllCharacters();


            GetComponent<DialogueHandler>().enterDialogueEvent += EnterDialogue;
            GetComponent<DialogueHandler>().exitDialogueEvent += ExitDialogue;
            GetComponent<EvidenceDetecting>().enterDrawing += Disable;
            GetComponent<EvidenceDetecting>().exitDrawing += Enable;
        }

        // Update is called once per frame
        void Update()
        {
            if ((Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Escape)) && usable && !inRotAnimation)
                
                TurnMenuOnOff();
        }


        private IEnumerator RotateCamEnumerator(float duration, bool downAnim)
        {
            inRotAnimation = true;
            float t = duration;
            RectTransform tr = i_menu.GetComponent<RectTransform>();
            RectTransform td = i_dialogueTransform;
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
                selectedDrawing = -1;
                RefitDrawingsContent();
                RefitDialogueLog();
                sound.PlayClip(SoundHandler.ClipEnum.UICloseBook, SoundHandler.OutputEnum.UI);
            }else {
                td.anchoredPosition = new Vector2(td.anchoredPosition.x, 700);
                tr.anchoredPosition = new Vector2(tr.anchoredPosition.x, 0  );
            }
            inRotAnimation = false;
        }

        private void RefitAllCharacters()
        {
            //Debug.Log("Refitting Characters Tab");
            for (int i = 0; i < i_CharactersAllTransform.childCount; i++)
            {
                Destroy(i_CharactersAllTransform.GetChild(i).gameObject);
            }

            for (int i = 0; i < characters.Length; i++)
            {
                GameObject g= Instantiate(pref_CharAll, i_CharactersAllTransform);
                g.GetComponent<RectTransform>().anchoredPosition =new Vector2(0 ,-50 - 100 * i);
                g.GetComponentInChildren<Image>().sprite = characters[i].img;
                g.GetComponentInChildren<Text>().text = characters[i].myName;
                int param1 = i;
                g.GetComponent<Button>().onClick.AddListener(delegate { SelectCharacter(param1);});
            }
            i_CharactersAllTransform.sizeDelta = new Vector2(i_CharactersAllTransform.sizeDelta.x, 100*characters.Length);
        }

        public void RefitDialogueLog()
        {
            i_DialogueLogSaveSelection.SetActive(selectedLogOption != null);
            float heightText = 0;
            float boxwidth = i_CharactersLogTransform.GetComponent<RectTransform>().sizeDelta.x;
            for (int i = 0; i < i_CharactersLogTransform.childCount; i++)
            {
                Destroy(i_CharactersLogTransform.GetChild(i).gameObject);
            }
            if(selectedChar==null)
                return;

            for (int i = 0; i < selectedChar.dialogueLog.Count; i++)
            {
                bool isSelected = selectedLogOption == selectedChar.dialogueLog[i];
                string temp = selectedChar.dialogueLog[i].question;
                GameObject go = Instantiate(pref_text, i_CharactersLogTransform);
                RectTransform rect1 = go.GetComponent<RectTransform>();
                go.GetComponentInChildren<Text>().text = "<color=maroon>"+temp+"</color>";
                float width1 = GetComponent<DialogueHandler>().GetWordWidth(temp);
                float height1 = Mathf.Ceil(width1 / boxwidth);
                width1 = (height1 > 1) ? boxwidth : width1;
                heightText -= (height1 / 2) * 18;
                rect1.sizeDelta = new Vector2(width1, height1 * 18);
                rect1.anchoredPosition = new Vector2(width1 / 2, heightText);
                heightText -= (height1 / 2) * 18;
                
                //string splitting process
                string[] str = GetComponent<DialogueHandler>().DevideAnswerIntoSentences(selectedChar.dialogueLog[i].answer);
                for (int j = 0; j < str.Length; j++)
                {
                    bool thisIsSelected = isSelected && j == selectedLogOptionAnswerIndx;
                    string s = str[j];
                    GameObject g = Instantiate(pref_text, i_CharactersLogTransform);
                    g.GetComponentInChildren<Text>().text =(thisIsSelected? "<b>":"") + s+ (thisIsSelected? "</b>" : "");
                    RectTransform rect = g.GetComponent<RectTransform>();
                    float width = GetComponent<DialogueHandler>().GetWordWidth(s);
                    float height = Mathf.Ceil(width / boxwidth);
                    width = (height > 1) ? boxwidth : width;
                    heightText -= (height / 2) * 18;
                    rect.sizeDelta = new Vector2(width, height * 18);
                    rect.anchoredPosition = new Vector2(width / 2, heightText);
                    heightText -= (height / 2) * 18;
                    int param = i;int param2 = j;
                    g.GetComponent<Button>().onClick.AddListener(delegate {
                        HighlightText(param,param2);
                    });

                }


            }
        }

        void HighlightText(int i,int j)
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

        int GetTextIndxFromButtonIndx(int indx, bool end,string[] storage)
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
            int selectionStartIndx = GetTextIndxFromButtonIndx(selectedLogOptionAnswerIndx, false,answer);
            int selectionEndIndx = GetTextIndxFromButtonIndx(selectedLogOptionAnswerIndx, true,answer);
            string content = answer[selectedLogOptionAnswerIndx];
            List<string> evidences = new List<string>();

            //		Debug.Log (selectionStartIndx + "  " + selectionEndIndx);
            if (selectedLogOption.evidences != null)
            {
                foreach (TextEvidence t in selectedLogOption.evidences)
                {
                    if (selectionStartIndx <= t.startIndx && selectionEndIndx >= t.endIndx)
                    {
                        evidences.Add(t.name);
                    }
                }
            }
            AddTextEvidence(new SavedTextEvidence(selectedChar.myName, content, evidences.ToArray()));
            HighlightText(-1,-1);
            sound.PlayClip(SoundHandler.ClipEnum.UIWriting, SoundHandler.OutputEnum.UI);
        }

        public void RefitDrawingsContent()
        {
            float width = i_drawingsContentTransform.GetComponent<RectTransform>().offsetMax.x;

            if (selectedDrawing < 0)
            {
                i_EvDrawingAskAbout.SetActive(false);
                i_EvDrawingDelete.SetActive(false);
            }
            else
            {
                if(inDialogue)
                i_EvDrawingAskAbout.SetActive(true);
                i_EvDrawingDelete.SetActive(true);
            }

            for (int i = 0; i < i_drawingsContentTransform.childCount; i++)
            {
                Destroy(i_drawingsContentTransform.GetChild(i).gameObject);
            }

            i_drawingsContentTransform.sizeDelta = new Vector2(0, drawings.Count * -200);

            for (int i = 0; i < drawings.Count; i++)
            {
                GameObject g = Instantiate(pref_drawing, i_drawingsContentTransform);
                RectTransform t = g.GetComponent<RectTransform>();
                t.anchoredPosition = new Vector2(140,-100  - 200 * i);
                g.GetComponent<Image>().sprite = drawings[i].sprite;
                g.GetComponent<DrawingComponent>().evidences = drawings[i].evidences;
                int param = i;
                g.GetComponent<Button>().onClick.AddListener(delegate { SelectDrawing(param); });
            }

        }

        public void RefitEvidenceContent()
        {
            for (int i = 0; i < i_textEvidenceContentTransform.childCount; i++)
            {
                Destroy(i_textEvidenceContentTransform.GetChild(i).gameObject);
            }
            float boxwidth = 250;
            float heightText = 0;
            i_textEvidenceContentTransform.sizeDelta = new Vector2(0, evidences.Count * 31);

            for (int i = 0; i < evidences.Count; i++)
            {
                SavedTextEvidence s = evidences[i];
                string text = "<color=maroon>" + s.autor +": "+ "</color>"+s.text ;
                GameObject go = Instantiate(pref_text, i_textEvidenceContentTransform);
                RectTransform rect1 = go.GetComponent<RectTransform>();
                go.GetComponentInChildren<Text>().text =  text;
                float width1 = GetComponent<DialogueHandler>().GetWordWidth(text);
                float height1 = Mathf.Ceil(width1 / boxwidth);
                width1 = (height1 > 1) ? boxwidth : width1;
                heightText -= (height1 / 2) * 18;
                rect1.sizeDelta = new Vector2(width1, height1 * 18);
                rect1.anchoredPosition = new Vector2(width1 / 2, heightText);
                heightText -= (height1 / 2) * 18;

                int param1 = i;
                go.GetComponent<Button>().onClick.AddListener(delegate { SelectTextEvidence(param1); });
            }

        }

        public void TurnMenuOnOff()
        {
            bool b = !i_menu.activeSelf;

            if (b)
            {
                i_menu.SetActive(true);
                selectedTextEvidences.Clear();
                selectedTextEvidencesIndx.Clear();
                i_EvTextAskAbout.SetActive(false);
                i_EvTextDelete.SetActive(false);
                if (enterMenu != null)
                {
                    enterMenu();
                }
                StartCoroutine(RotateCamEnumerator(1, true));
                sound.PlayClip(SoundHandler.ClipEnum.UIOpenBook, SoundHandler.OutputEnum.UI);
            }
            else
            {
                StartCoroutine(RotateCamEnumerator(1, false));
                
            }

        }

        public void ChangePannel(int pannelIndx)
        {
            selectedTextEvidences.Clear();
            selectedTextEvidencesIndx.Clear();
            SelectTextEvidence(-1);
            for (int i = 0; i < m_pannels.Length; i++)
                m_pannels[i].SetActive((i == pannelIndx) ? true : false);

            sound.PlayClip(SoundHandler.ClipEnum.UITurningPage,SoundHandler.OutputEnum.UI);
        }

        public void AddDrawing(Drawing d)
        {
            drawings.Add(d);
            RefitDrawingsContent();
        }

        public void AddTextEvidence(SavedTextEvidence s)
        {
            evidences.Add(s);
            RefitEvidenceContent();
        }

        public List<SavedTextEvidence> GetSavedTextEvidences()
        {
            return evidences;
        }

        public List<Drawing> GetSavedDrawings()
        {
            return drawings;
        }

        public bool RemoveSavedTextEvidence(SavedTextEvidence s)
        {
            return evidences.Remove(s);
        }

        public bool RemoveSavedDrawing(Drawing s)
        {
            return drawings.Remove(s);
        }

        public void SelectTextEvidence(int i)
        {

            if (i >= 0)
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (selectedTextEvidences.Contains(evidences[i]))
                    {
                        selectedTextEvidences.Remove(evidences[i]);
                        selectedTextEvidencesIndx.Remove(i);
                    }
                    else
                    {
                        selectedTextEvidences.Add(evidences[i]);
                        selectedTextEvidencesIndx.Add(i);
                    }
                }
                else
                {
                    selectedTextEvidences.Clear();
                    selectedTextEvidencesIndx.Clear();
                    selectedTextEvidences.Add(evidences[i]);
                    selectedTextEvidencesIndx.Add(i);
                }
            }
                
            //Debug.Log(selectedTextEvidence.text);
            for (int j = 0; j < i_textEvidenceContentTransform.childCount; j++)
            {
                GameObject g = i_textEvidenceContentTransform.GetChild(j).gameObject;
                if (selectedTextEvidencesIndx.Contains(j))
                {
                    g.GetComponentInChildren<Text>().color = new Color(0.2f, 0.2f, 0.2f, 1);
                }
                else
                {
                    g.GetComponentInChildren<Text>().color = new Color(0, 0, 0, 1);
                   
                }
            }

            if (inDialogue&& selectedTextEvidences.Count == 1)
            {
                i_EvTextAskAbout.SetActive(true);
            }
            else
            {
                i_EvTextAskAbout.SetActive(false);
            }

            if (selectedTextEvidences.Count == 0)
            {
                i_EvTextDelete.SetActive(false);
            }
            else
            {
                i_EvTextDelete.SetActive(true);
            }

            if (selectedTextEvidences.Count > 1)
            {
                i_EvTextMerge.SetActive(true);
            }
            else
            {
                i_EvTextMerge.SetActive(false);
            }
            

        }

        public void SelectCharacter(int i)
        {
            //Debug.Log(characters[i].myName);
            selectedChar = characters[i];

            for (int j = 0; j < i_CharactersAllTransform.childCount; j++)
            {
                i_CharactersAllTransform.GetChild(j).GetComponentInChildren<Text>().color = ((j == i) ? new Color(0.5f, 0.1f, 0.1f, 1): Color.black);
            }

            HighlightText(-1, -1);
            RefitDialogueLog();
        }

        public void SelectDrawing(int i)
        {
            selectedDrawing = i;
            RefitDrawingsContent();
        }

        public void AskAboutTextEvidence()
        {
            DialogueHandler dh = GetComponent<DialogueHandler>();
            Character c = dh.partner.GetComponent<Character>();
            DialogueOption[] evidences = c.aviableEvidenceOptions;
            DialogueOption o = c.option_noAnswer;



            if (selectedTextEvidences.Count > 1)
                return;

            SavedTextEvidence selectedTextEvidence = selectedTextEvidences[0];

            if (c.myName == "Sherlock Holmes")
            {
                if (selectedTextEvidence.containedEvidencesName.Length > 0)
                {
                    o = new DialogueOption("", "That looks important!", null, null, null);
                }
                else
                {
                    o = new DialogueOption("", "Looks like nothing to me..", null, null, null);
                }
            }
            else
            {//single evidence search
                foreach (string s in selectedTextEvidence.containedEvidencesName)
                {
                    foreach (DialogueOption d in evidences)
                    {
                        foreach(string str in d.reqEvidence)
                        {
                            if (s == str)
                                o = d;
                        }
                    }
                }

                //multiple evidence search
                if (selectedTextEvidence.containedEvidencesName.Length > 1)
                {
                    foreach (DialogueOption d in evidences)
                    {
                        if(selectedTextEvidence.containedEvidencesName.Length == d.reqEvidence.Length)
                        {
                            bool goodEvidence=true;
                            foreach (string str in d.reqEvidence)
                            {
                                bool strIsContained = false;
                                foreach (string str2 in selectedTextEvidence.containedEvidencesName)
                                {
                                    if (str == str2)
                                    {
                                        strIsContained = true;
                                    }
                                }
                                if (!strIsContained)
                                {
                                    goodEvidence = false;
                                } 
                            }

                            if (goodEvidence)
                            {
                                o = d;
                            }
                        }
                    }
                }
            }
            dh.PickDialogue(o);
            TurnMenuOnOff();
        }

        public void DeleteTextEvidence()
        {
            foreach(SavedTextEvidence d in selectedTextEvidences)
            {
                evidences.Remove(d);
            }
               
             selectedTextEvidences.Clear();
            selectedTextEvidencesIndx.Clear();
            SelectTextEvidence(-1); //resets the color changing on the evidences
            i_EvTextDelete.SetActive(false);
            i_EvTextAskAbout.SetActive(false);
            RefitEvidenceContent();

            sound.PlayClip(SoundHandler.ClipEnum.UICrossOut, SoundHandler.OutputEnum.UI);
        }

        public void MergeTextEvidences()
        {
            if (selectedTextEvidences.Count < 2)
            { 
                Debug.LogError("Trying to merge less then 2 evidences");
                return;
            }
            string autor = "";
            string text = "";
            List<string> evidencesName = new List<string>();

            for (int i = 0; i < selectedTextEvidences.Count; i++)
            {
                autor = ((autor=="")?selectedTextEvidences[i].autor:(autor==selectedTextEvidences[i].autor)? autor:"Various");
                text += " " + selectedTextEvidences[i].text;
                if(selectedTextEvidences[i].containedEvidencesName != null)
                {
                    foreach(string s in selectedTextEvidences[i].containedEvidencesName)
                    {
                        if(!evidencesName.Contains(s))
                        evidencesName.Add(s);
                    } 
                }
               
            }

            AddTextEvidence(new SavedTextEvidence(autor,text,evidencesName.ToArray()));
            selectedTextEvidences.Clear();
            selectedTextEvidencesIndx.Clear();
            SelectTextEvidence(-1);
            sound.PlayClip(SoundHandler.ClipEnum.UIWriting, SoundHandler.OutputEnum.UI);
        }

        public void AskAboutDrawing()
        {
            DialogueHandler dh = GetComponent<DialogueHandler>();
            Character c = dh.partner.GetComponent<Character>();
            DialogueOption[] evidences = c.aviableEvidenceOptions;
            DialogueOption o = c.option_noAnswer;

            Drawing selectedDrawingEvidence = drawings[selectedDrawing];

            if (c.myName == "Sherlock Holmes")
            {
                if (selectedDrawingEvidence.evidences.Length > 0)
                {
                    o = new DialogueOption("", "That looks important!", null, null, null);
                }
                else
                {
                    o = new DialogueOption("", "Looks like nothing to me..", null, null, null);
                }
            }
            else
            {//single evidence search
                foreach (DrawingEvidence s in selectedDrawingEvidence.evidences)
                {
                    foreach (DialogueOption d in evidences)
                    {
                        foreach (string str in d.reqEvidence)
                        {
                            if (s.name == str)
                                o = d;
                        }
                    }
                }

            }
            dh.PickDialogue(o);
            TurnMenuOnOff();

        }

        public void DeleteDrawing()
        {
            if (selectedDrawing < 0)
                return;
            drawings.RemoveAt(selectedDrawing);
            selectedDrawing = -1;
            RefitDrawingsContent();
         

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



}

