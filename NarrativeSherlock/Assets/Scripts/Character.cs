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
		//Invoke ("UpdateOptions", 0.1f);
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

    
    public CharacterSaveFormat GetCharacterInSaveFormat()
    {
        NavigationHandler navigation = GetComponent<NavigationHandler>();
        if (navigation != null)
        {
            return new CharacterSaveFormat(transform.position, transform.eulerAngles, dialogueOptions, dialogueLog.ToArray(),navigation.currentMovement,navigation.currentMovementIndx);
        }
 
        return new CharacterSaveFormat(transform.position, transform.eulerAngles, dialogueOptions, dialogueLog.ToArray(),null,0);
    }

    public void LoadCharacterFromSave(CharacterSaveFormat save)
    {
        transform.position = Vector3Serializable.Read(save.position);
        transform.eulerAngles = Vector3Serializable.Read(save.rotation);
        dialogueOptions = save.dialogueOptions;
        dialogueLog.Clear();
        foreach (DialogueOption d in save.dialogueLog)
        {
            dialogueLog.Add(d);
        }

        if(save.currentMovement != null)
        {
            GetComponent<NavigationHandler>().LoadNavigationMovement(NavigationMovementSerializable.Read(save.currentMovement), save.currentMovementIndx);
        }

    }
}

[System.Serializable]
public class CharacterSaveFormat{
    public Vector3Serializable position, rotation;
    public DialogueOption[] dialogueOptions,dialogueLog;
    public NavigationMovementSerializable currentMovement;
    public int currentMovementIndx;

    public CharacterSaveFormat(Vector3 pos, Vector3 rot , DialogueOption[] options, DialogueOption[] log, NavigationMovement _currentMovement, int _currentMovementIndx)
    {
        position = Vector3Serializable.Make(pos);
        rotation = Vector3Serializable.Make(rot);
        dialogueOptions = options;
        dialogueLog = log;
        if (_currentMovement==null)
        {
            currentMovement = null;
        }
        else
        {
            currentMovement = NavigationMovementSerializable.Make(_currentMovement);
        }

        currentMovementIndx = _currentMovementIndx;
    }
}
