using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Holmes_Control;
using System;

namespace Holmes_Import{
public class DialogueParser : MonoBehaviour {
	[SerializeField]	TextAsset dialogue; 
	[SerializeField] bool debug,testing = false;
	[SerializeField] DialogueOption[] dialogueOptions;
	
	private GameState gameState;
	// Use this for initialization
	void Start () {
		gameState = GameObject.FindGameObjectWithTag ("Main").GetComponent<GameState>();

            if (gameState.SaveFileLoaded)
                return;

            if (gameState.LoadFromLevelEditor)
            {
                LoadDialogsFromCustomLevel();
                return;
            }

		string s = T_Symplify(dialogue.text);

		if (debug) {
			Debug.Log ("Original Simplyfied: "+s);
		}

		string[] dialogues = T_DevideIntoDialogues (s);
		 dialogueOptions = new DialogueOption[dialogues.Length];

		for (int i = 0; i < dialogues.Length; i++) {
			dialogueOptions [i] = T_TranslateDialogueIntoDialogueOption (dialogues [i]);
		}

		if (debug) {
			foreach (string j in dialogues) {
				Debug.Log (j);
			}
		}
		T_CheckForVarIntegrity();

			if (!testing) {
				Character c =	GetComponent<Character> ();
				if (c != null) {
					c.dialogueOptions = dialogueOptions;
					//Debug.Log (c.myName + ": DialogueOptions uploaded.");
				}
			}
	}

        private void LoadDialogsFromCustomLevel()
        {
            
            string name = GetComponent<Character>().myName;
            AssetBundle ab = GameObject.FindGameObjectWithTag("DontDestroyOnLoadObj").GetComponent<ABHolder>().ab;
            TextAsset ta = ab.LoadAsset<TextAsset>(name+ ".json");

            string[] lines =  ta.text.Split('\n');

            if (lines == null || lines.Length < 1)
            {
                return;
            }

            List<DialogueOption> dialogs = new List<DialogueOption>();

            foreach (string line in lines)
            {
                if (line.Length > 10)
                {
                    dialogs.Add(JsonUtility.FromJson<DialogueOption>(line));
                }
            }

            GetComponent<Character>().dialogueOptions = dialogs.ToArray();
        }

        void T_CheckForVarIntegrity(){

		foreach (DialogueOption j in dialogueOptions) {

				if (j.reqTB != null) {
					foreach (TextBool i in j.reqTB) {
						if (!gameState.VarCheck (i.name))
							throw new DialogueParserException ("Error in: '" + j.question + "' no variable called '" + i.name + "' defined.");
					}
				}

				if (j.setTB != null) {
					foreach (TextBool i in j.setTB) {
						if (!gameState.VarCheck (i.name))
							throw new DialogueParserException ("Error in: '" + j.question + "' no variable called '" + i.name + "' defined.(Set section)");
					}
				}
				if(j.evidences != null){
			foreach (TextEvidence i in j.evidences) {
				if (!gameState.EvidenceTextCheck (i.name))
					throw new DialogueParserException("Error in: '" + j.question + "' no evidence called '" + i.name + "' defined. ("+ i.startIndx + "-" + i.endIndx + ")");
					}}
                if (j.fromEvidence)
                {
                    foreach (string s in j.reqEvidence)
                    {
                        if (!gameState.EvidencesCheck(s))
                            throw new DialogueParserException("Error in: '" + j.question + "' no evidence called '" + s + "' defined.");
                    }
                }

		}
	}


	string T_Symplify(string s){
		string t="";
		for (int i = 0; i < s.Length; i++) {
			if (s[i].GetHashCode () > 31)
				t += s[i];
		}
		return t;
	}


	string[] T_DevideIntoDialogues(string s){
		List<string> dialogues = new List<string> ();
		bool inDialogue = false;
		string temp = "";
		for (int i = 0; i < s.Length; i++) {
			char c = s [i];


			if (c == '}') {
				inDialogue = false;
				dialogues.Add (temp);
				temp = "";
			}
				
			if (inDialogue) {
				temp += c;
			}

			if (c == '{') {
				if (inDialogue)
					throw new DialogueParserException ("Starting new Dialogue before the old one ended!");
				inDialogue = true;
			}
		}
		if(debug)
			Debug.Log (" # of Dialogues: "+dialogues.Count);
		
		return dialogues.ToArray();
	}


	DialogueOption T_TranslateDialogueIntoDialogueOption (string dial)
	{
			string dialogue =dial;
		string temp = "";
			List<string> diaLines = new List<string> ();
			string s_reqV = "", s_setV = "", s_question = "", s_answer = "" , s_evidenceName ="";
		TextBool[] reqV, setV;
            string[] reqEvidences=null;
			bool isEvidence=false;
			bool objEvidence=false;
            bool oneTime = false;

			if (dialogue [0] == '@' || dialogue[0] == '#') {
				isEvidence = true;
				objEvidence = dialogue[0] =='#';
				dialogue = dialogue.Remove (0, 1);
			}

            if (dialogue[0] == '1')
            {
                oneTime = true;
                dialogue = dialogue.Remove(0, 1);
            }
				

		//devide into 4 strings;
		int indx = 0;
		for (int i = 0; i < dialogue.Length; i++) {
			char c = dialogue [i];
			if (c == ';') {
				if (indx > 3)
					throw new DialogueParserException ("More then 4 ';' in a Dialogue!");
					diaLines.Add(temp);
					temp = "";
			} else {
				temp += c;
			}
		}
		
			foreach (string s in diaLines) {
				if (s.Length < 3)
					continue;

				if (s [0] == 'R' && s [1] == ':') {
					s_reqV = s.Remove (0, 2);
				} else if (s [0] == 'Q' && s [1] == ':') {
					if (isEvidence) {
						s_evidenceName = s.Remove (0, 2);
					} else {
						s_question = s.Remove (0, 2);
					}
				} else if (s [0] == 'A' && s [1] == ':') {
					s_answer = s.Remove (0, 2);
				} else if (s [0] == 'S' && s [1] == ':') {
					s_setV = s.Remove (0, 2);
				} else {
					throw new DialogueParserException ("Found section without specification: (" + s+")");
				}

			}
				
			s_setV = s_setV.Replace (" ", "");
			s_reqV = s_reqV.Replace (" ", "");
			s_evidenceName = s_evidenceName.Replace (" ", "");

			string[] lines = new string[4];
			lines [0] = s_reqV;
			lines [1] = (isEvidence) ? s_evidenceName : s_question;
			lines [2] = s_answer;
			lines [3] = s_setV;

		List<TextBool> tempL = new List<TextBool> ();
		//decode reqVar line
	
				string[] tempS = lines [0].Split (',');
			if (lines [0] != "") {
				foreach (string s in tempS) {
					string k = s;
					bool b = true;

					if (k == "")
						throw new DialogueParserException ("Extra ',' at reqVar!");

					if (k.Contains ("!")) {
						b = false;
						k = k.Replace ("!", "");
					}
					tempL.Add (new TextBool (k, b));
				}
				reqV = tempL.ToArray ();

			} else
				reqV = null;
			
		//decode setVars line
			if (lines [3] != "") {
				tempL.Clear ();
				tempS = lines [3].Split (',');
				foreach (string s in tempS) {
					string k = s;
					bool b = true;
					if (k == "")
						throw new DialogueParserException ("Extra ',' at setVar!");

					if (k.Contains ("!")) {
						b = false;
						k = k.Replace ("!", "");
					}
					tempL.Add (new TextBool (k, b));
				}
				setV = tempL.ToArray ();
			} else {
				setV = null;
			}

            //decode is Evidence Questioned Evidences
            if (isEvidence)
            {
                reqEvidences = lines[1].Split(',');
            }
		//decode Evidence
		List<TextEvidence> evidences = new List<TextEvidence> ();
		string evidenceName = "", evidenceText = "";
		bool inEvidence = false;
		bool inEvidenceName = false;
		int evStart = 0 , nameEnd=0;
		for (int i = 0; i < lines [2].Length; i++) {
			char c = lines [2] [i];

			if (c == '<') {
				if (inEvidence || inEvidenceName)
					throw new DialogueParserException ("Nested Evidences are not integrated. Extra '<' found.");
				inEvidenceName = true;
				evStart = i;

				lines [2] = lines [2].Remove (i,1);
				i--;
			} else if (c == '>') {
				if (inEvidence) {
					inEvidence = false;
					//lines [2] = lines [2].Remove (i); 
					//lines [2] = lines [2].Remove (evStart, nameEnd - evStart);
					//i -= (nameEnd-evStart) + 1;
					evidences.Add (new TextEvidence (evidenceName, evidenceText, evStart, evStart += evidenceText.Length-1));

					evStart = 0;
					nameEnd = 0;
					evidenceName = "";
					evidenceText = "";
				} else if (inEvidenceName) {
					nameEnd = i;
					inEvidenceName = false;
					inEvidence = true;
				} else {
					throw new DialogueParserException ("Unexpected '>' found.");
				}

				lines [2] = lines [2].Remove (i,1);
				i--;

			} else {
				if (inEvidence)
					evidenceText += c;
				if (inEvidenceName) {
					evidenceName += c;
					lines [2] = lines [2].Remove (i,1);
					i--;
				}
			}
		}


		if (debug) {
			foreach (string s in lines) {
				Debug.Log (s);
			}
		}

			if (isEvidence) {
				return new DialogueOption (objEvidence,reqEvidences,lines[2],reqV,setV,(evidences.Count>0)? evidences.ToArray():null,oneTime);
			}
		return new DialogueOption (lines [1], lines [2], reqV, setV, (evidences.Count>0)? evidences.ToArray():null,oneTime);
	}


		public void RunTest(TextAsset t,bool _debug){
			dialogue = t;
			testing = true;
			debug = _debug;
			Start();
		}
}

}

[System.Serializable]
public class DialogueOption{
	
			public string question;
			public string answer;
			public TextBool[] reqTB,setTB;
			public TextEvidence[] evidences;
            public bool oneTime;

	//evidences 
	public bool fromEvidence =false;
	public bool ObjEvidence = false;
	public string[] reqEvidence;

	public DialogueOption(string _question, string _answer, TextBool[] _reqVariables, TextBool[] _SetVariables, TextEvidence[] _evidences,bool _oneTime){
		reqTB = _reqVariables;
		setTB = _SetVariables;
		question = _question;
		answer = _answer;
		evidences = _evidences;
        oneTime = _oneTime;
	}

	public DialogueOption(bool _ObjEvidence, string[] _reqEvidence, string _answer, TextBool[] _reqVariables, TextBool[] _SetVariables, TextEvidence[] _evidences,bool _oneTime){
		fromEvidence = true;
		reqTB = _reqVariables;
		setTB = _SetVariables;
		question = "Evidence Dialogue from: "+_reqEvidence[0];
		answer = _answer;
		evidences = _evidences;
		ObjEvidence = _ObjEvidence;
		reqEvidence = _reqEvidence;
        oneTime = _oneTime;
	}

}



[System.Serializable]
public struct TextBool{
	public string name;
	public bool state;
	public TextBool(string _name,bool _state){
		name = _name;
		state = _state;
	}
}

[System.Serializable]
public class TextEvidence{
	public string name,text;
 	public int startIndx, endIndx;

public TextEvidence(string _name , string _text, int _startIndx, int _endIndx){

name = _name;
text = _text;
startIndx = _startIndx;
endIndx = _endIndx;
}
}

public class DialogueParserException : UnityException{
	public DialogueParserException(string message):base(message){
	}
}