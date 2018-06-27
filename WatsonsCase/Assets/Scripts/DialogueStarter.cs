using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueStarter : MonoBehaviour {
	 bool inConversation =false,inMenu=false;
	[SerializeField] GameObject highlightTextObj,pointer,directionObj;
	private Text highlightText;
	private GameObject handler;
    private GameObject partner;
    private int rotSpeed = 7;

	void Awake(){
		handler = GameObject.FindGameObjectWithTag ("Main");
		highlightText = highlightTextObj.GetComponent<Text>();
		if (handler == null)
			throw new UnityException ("NO OBJ TAGGED Main FOUND");

		handler.GetComponent<DialogueHandler> ().enterDialogueEvent += EnterDialogue;
		handler.GetComponent<DialogueHandler> ().exitDialogueEvent += ExitDialogue;
		handler.GetComponent<Holmes_Menu.MenuHandler> ().enterMenu += EnterMenu;
		handler.GetComponent<Holmes_Menu.MenuHandler> ().exitMenu += ExitMenu;
		handler.GetComponent<EvidenceDetecting> ().enterDrawing += EnterDialogue;
		handler.GetComponent<EvidenceDetecting> ().exitDrawing += ExitDialogue;
        handler.GetComponent<CutsceneHandler>().CutSceneStartEvent += EnterCutscene;
        handler.GetComponent<CutsceneHandler>().CutSceneEndEvent += ExitCutScene;


	}

    // Update is called once per frame
    void Update() {
        if (inConversation) {
            if(partner!=null&&!inMenu){
                RotateTowardsWithMovement(partner.transform.GetChild(0).position);
            }
            return;
    }


        if (inMenu)
            return;

		RaycastHit b;
		GameObject o=null;
		bool pointingChar = false;
		bool pointingDoor = false;
        bool pointingEndDoor = false;
        bool pointingPianoKey = false;
        //targeting
        Vector3 dir = directionObj.transform.position - transform.position;
        dir.Normalize();

        if (Physics.Raycast (transform.position+dir*0.3f,dir, out b, 3)) {
            //Debug.Log (b.collider.name);
            o = b.collider.gameObject;
            if (b.collider.tag == "Character")
            {
                SetHighlightText(o.GetComponent<Character>().highlightText);
                pointingChar = true;
            }
            else if (b.collider.tag == "Door")
            {
                SetHighlightText((o.GetComponent<DoorMovement>().isLocked) ? "" : (b.collider.gameObject.GetComponent<DoorMovement>().toOpen) ? "Open" : "Close");
                pointingDoor = true;
            }
            else if (b.collider.tag == "SpecialDoor")
            {
                pointingEndDoor = true;
                SetHighlightText(o.GetComponent<DoorEndingScript>().HoverText);

            }
            else if(o.tag == "PianoKey")
            {
                pointingPianoKey = true;
                SetHighlightText(o.GetComponent<PianoKeyPlayer>().keyName);
            }
            else { 
				SetHighlightText ("");
			}
		} else {
			SetHighlightText ("");

		}

        bool mouseDown = Input.GetMouseButtonDown(0);
        if (pointingChar && mouseDown) {
            if(b.collider.gameObject.GetComponent<NavigationHandler>()!=null)
			b.collider.gameObject.GetComponent<NavigationHandler> ().EnterConversation ();
            partner = b.collider.gameObject;
			handler.GetComponent<DialogueHandler> ().EnterConversation (o);
		}
		if (pointingDoor && mouseDown) {
			o.GetComponent<DoorMovement> ().Interact();

		}
        if(pointingEndDoor&& mouseDown)
        {
            o.GetComponent<DoorEndingScript>().ActivateEndGameWarningText();
        }
        if(pointingPianoKey && mouseDown)
        {
            o.GetComponent<PianoKeyPlayer>().PlayNote();
        }


	}

	void SetHighlightText(string s){
		highlightText.text = s;
	}

	void EnterDialogue(){
		inConversation = true;
		SetHighlightText ("");
		pointer.SetActive (false);
	}

	void ExitDialogue(){
		inConversation = false;
		pointer.SetActive (true);
        partner = null;
	}

    void EnterMenu() {
        inMenu = true;
        SetHighlightText("");
        pointer.SetActive(false);

    }

    void ExitMenu()
    {
        inMenu = false;
        pointer.SetActive(true);
        partner = null;
    }

    void EnterCutscene(string s)
    {
        inMenu = true;
        inConversation = true;
        SetHighlightText("");
        pointer.SetActive(false);
        partner = null;
    }

    void ExitCutScene(string s)
    {
        inMenu = false;
        inConversation = false;
        pointer.SetActive(true);
    }


    // based on: https://answers.unity.com/questions/540120/how-do-you-update-navmesh-rotation-after-stopping.html
    void RotateTowardsWithMovement(Vector3 partnerPos)
    {
        Vector3 v1 = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 v2 = new Vector3(partnerPos.x, partnerPos.y, partnerPos.z);
        Vector3 v3 = v1 - v2;
        Vector3 pos =partnerPos+ (Vector3.Cross(v3, Vector3.up).normalized*(v3.magnitude/4));


        
        Vector3 direction = (pos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, direction.y, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpeed);
    }
}


