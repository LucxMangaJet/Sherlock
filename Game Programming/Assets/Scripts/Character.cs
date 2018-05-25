using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holmes_Import;
using Holmes_Control;

public class Character : MonoBehaviour {
	
	public string myName;
	public Sprite img;
	public string highlightText;

	public DialogueOption option_begin,option_noAnswer; 


	public  List<DialogueOption> dialogueLog;
	public DialogueOption[] dialogueOptions;
	public DialogueOption[] aviableDialogueOptions;
	public DialogueOption[] aviableEvidenceOptions;

	private GameObject handler;

	// Use this for initialization
	void Start () {
		handler = GameObject.FindGameObjectWithTag ("Main");
		dialogueLog = new List<DialogueOption> ();
		Invoke ("UpdateOptions", 0.1f);
	}

	public	void UpdateOptions(){
		List<DialogueOption> o = new List<DialogueOption> ();
		List<DialogueOption> e = new List<DialogueOption> ();

		GameState s = handler.GetComponent<GameState> ();
		foreach (DialogueOption d in dialogueOptions) {
			bool aviable = true;
			if (d.reqTB != null) {
				foreach (TextBool t  in d.reqTB) {
					//Debug.Log (t.state + t.name + s.VarGet (t.name));
					if (t.state != s.VarGet (t.name))
						aviable = false;
				
				}}

			if (aviable) {
				if (d.fromEvidence) {
					e.Add (d);
				} else {
					o.Add (d);
				}
			}
			
		}

		aviableDialogueOptions = o.ToArray ();
		aviableEvidenceOptions = e.ToArray ();
	}

    public void RemoveDialogueOption(DialogueOption d)
    {
        List<DialogueOption> j =new List<DialogueOption>(dialogueOptions);
        j.Remove(d);
        dialogueOptions = j.ToArray() ;

    }

	// Update is called once per frame
	void Update () {
		
	}
}
