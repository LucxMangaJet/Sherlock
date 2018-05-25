using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holmes_Control;
public class TutorialHandler : MonoBehaviour {
    [SerializeField] GameObject door1, door2;
    //[SerializeField] GameObject 
    GameState state;
	// Use this for initialization
	void Start () {
        state = GetComponent<GameState>();
        state.updatedVarEvent += CheckVarUpdate;
        CheckVarUpdate("Start", true);
	}
	
	void CheckVarUpdate(string s,bool b)
    {
        Debug.Log("VarUpdateCalled: " + s + " " + b);
        switch (s){
            case "ReadInvitation":
                if (b)
                    door1.GetComponentInChildren<DoorMovement>().isLocked = false;
                    //Display: Opening doors and talking with characters
                break;
            case "FinishedTutorial":
                if (b)
                    door2.GetComponentInChildren<DoorMovement>().isLocked = false;
                break;

            case "Start":
                //Display: movement and interacting with things Vid
                break;

            case "AskedAboutGoing":
                //Display: Navigating Menu, Saving Evidences, Dialogue Log, Asking About Evidences
                break;
            case "Taunted":
                //Display: Make Drawings ask about Drawings
                break;

            case "SignatureFromViolin":
                //Display: Merging Evidences and Multiple Evidences Selection
                break;
            default:
                break;
        }


    }
}
