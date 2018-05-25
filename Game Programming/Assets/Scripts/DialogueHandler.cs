using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueHandler : MonoBehaviour {

	[SerializeField] bool inDialogue =false;
	[SerializeField] GameObject ui_Dialogue ,ui_Text,ui_Buttons,  ui_SaveSelection,ui_ErrorField;
	[SerializeField] GameObject pref_Button , pref_TextButton,pref_drawing;
	[SerializeField] Sprite i_HighlightSprite;
	[SerializeField] Font usedFont;
    public GameObject partner;



	public delegate  void EmptyDel();
	public event EmptyDel enterDialogueEvent,exitDialogueEvent;
   

	private DialogueOption currentDialogue=null;
	private List<GameObject> temp = new List<GameObject> ();


	//highlight
	[SerializeField]	private List<GameObject> answer = new List<GameObject>();
	private int highlightIndx=-1;

	



	void Start(){
	}


	public void EnterConversation(GameObject g){
		Debug.Log ("Entering Conversation with: " + g.GetComponent<Character>().myName);
		inDialogue = true;
		if (enterDialogueEvent != null)
			enterDialogueEvent();
		partner = g;
		ui_Dialogue.SetActive (true);
		partner.GetComponent<Character> ().UpdateOptions ();
		LoadAndDisplayConversation ();
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
        ui_SaveSelection.SetActive(false);
	
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
				g.GetComponentInChildren<Text> ().text = s;
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
				g.GetComponent<Button> ().onClick.AddListener (delegate {
					HighlightText(param);	
				});

			}

		//buttons
		foreach (DialogueOption d in c.aviableDialogueOptions) {
			GameObject g = Instantiate (pref_Button, ui_Buttons.transform);
			temp.Add (g);

			DialogueOption param = d;
			g.GetComponent<Button> ().onClick.AddListener ( delegate {
				PickDialogue (param);
			});
			g.GetComponentInChildren<Text> ().text = d.question;
			RectTransform rect = g.GetComponent<RectTransform> ();
			float width = GetWordWidth (d.question);
			rect.sizeDelta = new Vector2 (width+10, 30);
			rect.anchoredPosition = new Vector2 ( 10 + width/2,heightButton);
			heightButton -= 30;
		}

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
			

		foreach (GameObject g in answer) {
			g.GetComponentInChildren<Text> ().color = new Color(1,1,1,1);
		}
		if (highlightIndx < 0) {
			ui_SaveSelection.SetActive (false);
		} else {
			ui_SaveSelection.SetActive (true);
			answer [highlightIndx].GetComponentInChildren<Text> ().color = new Color (0.5f, 0.1f, 0.1f, 1);
		}
	}



	public void SaveSelection(){
		int selectionStartIndx= GetTextIndxFromButtonIndx(highlightIndx,false);
		int selectionEndIndx = GetTextIndxFromButtonIndx(highlightIndx,true);
		string content = answer [highlightIndx].GetComponentInChildren<Text> ().text;
		List<string> evidences = new List<string> ();

//		Debug.Log (selectionStartIndx + "  " + selectionEndIndx);
		if (currentDialogue.evidences != null) {
			foreach (TextEvidence t in currentDialogue.evidences) {
				if (selectionStartIndx <= t.startIndx && selectionEndIndx >= t.endIndx) {
					evidences.Add (t.name);
				}
			}
		}
		GetComponent<Holmes_Menu.MenuHandler> ().AddTextEvidence (new Holmes_Menu.SavedTextEvidence(partner.GetComponent<Character>().myName,content,evidences.ToArray()) );
		HighlightText(-1);
        GetComponent<SoundHandler>().PlayClip(SoundHandler.ClipEnum.UIWriting, SoundHandler.OutputEnum.UI);
	}



	int GetTextIndxFromButtonIndx(int indx,bool end){
		int counter = 0;
		for (int i = 0; i < indx+((end)?1:0) ; i++) {
			counter += 1 + answer [i].GetComponentInChildren<Text> ().text.Length;
		}

		return counter;
	}

	public void PickDialogue (DialogueOption d){
       // Debug.Log("pickingDialogue: "+ d.answer);
		currentDialogue = d;
		if (d != null) {
			if (!partner.GetComponent<Character> ().dialogueLog.Contains (d))
				partner.GetComponent<Character> ().dialogueLog.Add (d);
            partner.GetComponent<Character>().RemoveDialogueOption(d);
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