using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEndingScript : MonoBehaviour {
    [SerializeField] GameObject WarningObj;
    [SerializeField] Transform TeleportPos;
    public string HoverText;
    bool WarningWindow = false;
    GameObject player;
	// Use this for initialization
	void Start () {
        player =GameObject.FindGameObjectWithTag("Player");
	}
	



    public void ActivateEndGameWarningText()
    {
        player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonControllerMod>().blocked = true;
        WarningObj.SetActive(true);
        Debug.Log("Activated Warning");
        WarningWindow = true;
    }

    public void DeactivateEndGameWarningText()
    {
        player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonControllerMod>().blocked = false;
        WarningObj.SetActive(false);
        Debug.Log("Deactivated Warning");
        WarningWindow = false;
    }

    public void ConfirmEndGameStart()
    {
        WarningWindow = false;
        WarningObj.SetActive(false);
        player.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonControllerMod>().blocked = false;
        player.transform.position = TeleportPos.position;
        player.transform.rotation = TeleportPos.rotation;
        GameObject.FindGameObjectWithTag("Main").GetComponent<Holmes_Control.GameState>().savable = false;
        Debug.Log("End Game Start");
    }
}
