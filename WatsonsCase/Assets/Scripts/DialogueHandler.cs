using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour {

	[SerializeField] bool inDialogue =false;
	[SerializeField] GameObject ui_Dialogue ,ui_Text,ui_Buttons,ui_ErrorField,ui_VisualHelpAddEvidence;
	[SerializeField] GameObject pref_Button , pref_TextButton,pref_drawing;
	[SerializeField] Sprite i_HighlightSprite;
	[SerializeField] Font usedFont;
    public GameObject partner;
    public Holmes_Control.GameState state;


	public delegate  void EmptyDel();
	public event EmptyDel enterDialogueEvent,exitDialogueEvent;
   

	private DialogueOption currentDialogue=null;
	private List<GameObject> temp = new List<GameObject> ();

	private VisualFeedbackHandler visualFeedback;


	//highlight
	[SerializeField]	private List<GameObject> answer = new List<GameObject>();
	private int highlightIndx=-1;


    private void Start()
    {
        state = GetComponent<Holmes_Control.GameState>();
		visualFeedback = GetComponent<VisualFeedbackHandler> ();
    }





    public void EnterConversation(GameObject g){
		Debug.Log ("Entering Conversation with: " + g.GetComponent<Character>().myName);
		inDialogue = true;
		if (enterDialogueEvent != null)
			enterDialogueEvent();
		partner = g;
		ui_Dialogue.SetActive (true);
		partner.GetComponent<Character> ().UpdateOptions ();
        GetComponent<Holmes_Menu.MenuHandler>().lastCharTalkedTo = partner.GetComponent<Character>();
		LoadAndDisplayConversation ();
        ui_VisualHelpAddEvidence.SetActive(true);

    }
		
	public void ExitConversation(){
		//Debug.Log ("Ending Conversation");
		inDialogue = false;
		if (exitDialogueEvent != null)
			exitDialogueEvent ();
        if(partner.GetComponent<NavigationHandler>()!=null)
		partner.GetComponent<NavigationHandler> ().ExitConversation ();
		partner = null;
		currentDialogue = null;
		ui_Dialogue.SetActive (false);
        ui_VisualHelpAddEvidence.SetActive(false);

        ClearTemp ();
	}

	//dialogue Methods
	void LoadAndDisplayConversation(){
		Character c = partner.GetComponent<Character> ();
		answer.Clear ();
		float heightButton = -16;
		float heightText = 0;
		//float xText = 0;
		float boxwidth = ui_Text.GetComponent<RectTransform> ().sizeDelta.x;

		//text
	//	Debug.Log(currentDialogue);
		if (currentDialogue == null) {
			currentDialogue = c.option_begin;
		}
			//string splitting process
			string[] str = DevideAnswerIntoSentences(currentDialogue.answer);

			for(int i =0; i<str.Length;i++){
				string s = str [i];
				GameObject g = Instantiate (pref_TextButton, ui_Text.transform);
				temp.Add (g);

            //checking if contains evidences for easy mode
            bool colored = false;
            if (!state.HardMode)
            {
                int counter = 0;
                for (int j = 0; j < i; j++)
                {
                    counter += 1 + str[j].Length;
                }
                int selectionStartIndx = counter;
                counter = 0;
                for (int j = 0; j < i+1; j++)
                {
                    counter += 1 + str[j].Length;
                }
                int selectionEndIndx = counter;

                if (currentDialogue.evidences != null)
                {
                    foreach (TextEvidence t in currentDialogue.evidences)
                    {
                        if (selectionStartIndx - 3 <= t.startIndx && selectionEndIndx + 3 >= t.endIndx)
                        {
                            colored = true;
                        }
                    }
                }
            }
                 s = (colored ? "<i><b>" : "") + s + (colored ? "</b></i>" : "");
                g.GetComponentInChildren<Text> ().text =s;
				RectTransform rect = g.GetComponent<RectTransform> ();
				float width = GetWordWidth (s);
				float height = Mathf.Ceil (width / boxwidth);
				width = (height > 1) ? boxwidth : width;
				heightText -=  (height/2)* 18;
				rect.sizeDelta = new Vector2 (width,height*18);
				rect.anchoredPosition = new Vector2 ( width/2,heightText);
				heightText -=  (height/2)* 18;

				answer.Add (g);
				int param = i;
              g.GetComponent<DialogDragHandler>().selectedIndx = param;
    //        g.GetComponent<Button> ().onClick.AddListener (delegate {
				//	HighlightText(param);	
			//	});

			}

        //buttons
        int x = 0;
		foreach (DialogueOption d in c.aviableDialogueOptions) {
            if (x > 4) // to avoid getting outside the screen because of too many questions
                break;
			GameObject g = Instantiate (pref_Button, ui_Buttons.transform);
			temp.Add (g);

            DialogueOption param = d;
            g.GetComponent<Button>().onClick.AddListener(delegate
            {
                PickDialogue(param);
            });
            g.GetComponentInChildren<Text> ().text = d.question;
			RectTransform rect = g.GetComponent<RectTransform> ();
			float width = GetWordWidth (d.question);
			rect.sizeDelta = new Vector2 (width+10, 30);
			rect.anchoredPosition = new Vector2 ( -40 -width/2,heightButton);
			heightButton -= 30;
            x++;
		}


        //quit button
        if (c.myName == "Sherlock")
        {
            return;
        }
        const string QuitButtonText = "Thats it for now.";

        GameObject gQuit = Instantiate(pref_Button, ui_Buttons.transform);
        temp.Add(gQuit);
        gQuit.GetComponent<Button>().onClick.AddListener(delegate {
            ExitConversation();
        });
        gQuit.GetComponentInChildren<Text>().text = QuitButtonText;
        RectTransform r = gQuit.GetComponent<RectTransform>();
        float wi = GetWordWidth(QuitButtonText);
        r.sizeDelta = new Vector2(wi + 10, 30);
        r.anchoredPosition = new Vector2(-40 - wi / 2, heightButton);
        
    }

	public float GetWordWidth(string s){

 
		float totalLength = 0;
  
         CharacterInfo characterInfo = new CharacterInfo();
		usedFont.RequestCharactersInTexture (s,14);
		char[] arr = s.ToCharArray();
	
         foreach(char c in arr)
         {
			usedFont.GetCharacterInfo(c, out characterInfo, 14);  
			if (characterInfo.advance != 0) {
				totalLength += characterInfo.advance;
			} else {
				totalLength += 8f;
			}
         }
         return totalLength +8;
	}

	public string[] DevideAnswerIntoSentences(string a){
		if (a.Length < 1)
			return new string[0];

		List<string> sentences = new List<string> ();
		string temp = "";
		for (int i = 0; i < a.Length-1; i++) {
			if ((a [i] == '.' && a [i + 1] == ' ') || (a [i] == '!' && a [i + 1] == ' ') || (a [i] == '?' && a [i + 1] == ' ')) {
				temp += ""+ a[i];
				temp += ""+ a[i+1];
				sentences.Add (temp);
				temp = "";
				i++;
			} else {
				temp += a [i];
			}
				
		}
		temp += "" + a [a.Length - 1];
		sentences.Add (temp);
		return sentences.ToArray ();
	}


	public void HighlightText(int i){
		//Debug.Log ("Highlighted:" + answer [i].GetComponentInChildren<Text> ().text);

		highlightIndx = i;
			

		//foreach (GameObject g in answer) {
		//	g.GetComponentInChildren<Text> ().color = new Color(1,1,1,1);
		//}
		//if (highlightIndx < 0) {
		//	ui_SaveSelection.SetActive (false);
		//} else {
		//	ui_SaveSelection.SetActive (true);
		//	answer [highlightIndx].GetComponentInChildren<Text> ().color = new Color (0.5f, 0.1f, 0.1f, 1);
		//}
	}



	public void SaveSelection(){
		int selectionStartIndx= GetTextIndxFromButtonIndx(highlightIndx,false);
		int selectionEndIndx = GetTextIndxFromButtonIndx(highlightIndx,true);
		string content = answer [highlightIndx].GetComponentInChildren<Text> ().text;
		List<string> evidences = new List<string> ();

//		Debug.Log (selectionStartIndx + "  " + selectionEndIndx);
		if (currentDialogue.evidences != null) {
			foreach (TextEvidence t in currentDialogue.evidences) {
               // Debug.Log(t.name + " " + selectionStartIndx + " " + selectionEndIndx + " " +t.startIndx+" "+t.endIndx);
                if (selectionStartIndx-3 <= t.startIndx && selectionEndIndx+3 >= t.endIndx) {
					evidences.Add (t.name);
				}
			}
		}
		GetComponent<EvidenceHandler> ().AddTextEvidence (new SavedTextEvidence(partner.GetComponent<Character>().myName,content,evidences.ToArray()) );
		HighlightText(-1);
        int len = selectionEndIndx - selectionStartIndx;
        GetComponent<SoundHandler>().PlayClip(len<8?SoundHandler.ClipEnum.UIWritingShort:len<20? SoundHandler.ClipEnum.UIWritingMid: SoundHandler.ClipEnum.UIWritingLong, SoundHandler.OutputEnum.UI);

		// trigger visual feedback for saving evidence
		visualFeedback.ShowVisualFeedback("Evidence saved!");
	}



	int GetTextIndxFromButtonIndx(int indx,bool end){
		int counter = 0;
		for (int i = 0; i < indx+((end)?1:0) ; i++) {
            string s = RemoveSpecialCharacters(answer[i].GetComponentInChildren<Text>().text);
           // Debug.Log(s);
            counter += s.Length;
		}

		return counter;
	}

     string RemoveSpecialCharacters( string s)
    {
        string g = "";
        bool inParentheses=false;
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];

            if (c == '<')
            {
                inParentheses = true;
            }else if (c == '>')
            {
                inParentheses = false;
            }
            else {
                if (!inParentheses)
                    g += c;
            }
        }
        return g;
    }

	public void PickDialogue (DialogueOption d){
       // Debug.Log("pickingDialogue: "+ d.answer);
		currentDialogue = d;
		if (d != null) {
			if (!partner.GetComponent<Character> ().dialogueLog.Contains (d))
				partner.GetComponent<Character> ().dialogueLog.Add (d);
            if (d.oneTime)
            {
                partner.GetComponent<Character>().RemoveDialogueOption(d);
            }
            UpdateVars (d);
		}
		ClearTemp ();
        HighlightText(-1);
        LoadAndDisplayConversation ();
	}
		
		
	void UpdateVars(DialogueOption d){
		Holmes_Control.GameState h = GetComponent<Holmes_Control.GameState> ();
		if (d.setTB != null) {
			foreach (TextBool b in d.setTB) {
				h.VarSet (b.name, b.state);
			}
		}
		partner.GetComponent<Character> ().UpdateOptions ();
	}

	void ClearTemp(){
		foreach (GameObject g in temp) {
			Destroy (g);
		}
		temp.Clear ();
	}



	// error text
	public void SetErrorField(string text, float seconds){
		ui_ErrorField.GetComponent<Text> ().text = text;
		Invoke ("ClearErrorField", seconds);
	}

	void ClearErrorField(){
		ui_ErrorField.GetComponent<Text> ().text = "";
	}

}