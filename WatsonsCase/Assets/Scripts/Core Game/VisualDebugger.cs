using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holmes_Control;

public class VisualDebugger : MonoBehaviour {

    [SerializeField] KeyCode debuggerKey= KeyCode.Comma; //Key to trigger the debugger
    [SerializeField] bool debuggerActive=false;            // is the debugger Active?
    [SerializeField] float buttonMaxCooldown = 0.5f;        // Cooldown to retrigger the next button;
    GameState state;


    //debugger vars
    string inputField="";
    string cutsceneField= "Test";
    float buttonCooldown = 0;
    bool inCutscene = false;

	void Start () {
        state = GetComponent<GameState>();
	}
	
	void Update () {

        //activate and deactivate the debug mode
        if (Input.GetKeyDown(debuggerKey))
        {
            EnterExitDebugMode();
        }

 

        //cooldown countdown
        if (buttonCooldown > 0)
        {
            buttonCooldown -= Time.deltaTime;
        }
	}

    /// <summary>
    /// Called to enable/disable the visual debugger.
    /// </summary>
    void EnterExitDebugMode()
    {
        debuggerActive = !debuggerActive;
        if (debuggerActive)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonControllerMod>().BlockMovement();
        }
        else
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonControllerMod>().UnlockMovement();
        }

    }

    private void OnGUI()
    {
        if (debuggerActive)
        {
            //Rappresentation 


            //Variables
            string s_variable = "Variables:\n";
            int counter = 0;
            foreach (KeyValuePair<string, bool> entry in state.variables)
            {
                counter++;
                s_variable += entry.Key + ": " + ((entry.Value) ? "\u2713" : "x") + "\n"; 
            }
            GUI.TextArea(new Rect(0, 0, 250, counter*17), s_variable);

            //FirstPersonController locked state
            GUI.TextArea(new Rect(0, counter*17 +5, 250, 100), "FirstPersonController locked states: \n" + GameObject.FindGameObjectWithTag("Player").GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonControllerMod>().GetBlockersAsString());


            //Buttons and Features

            //Toggle the Drawing Effect
           if (GUI.RepeatButton(new Rect(255, 50, 150, 40), "Toggle Drawing Effect") && buttonCooldown <= 0)
            {
                EnableDisableDrawingCamera();
                buttonCooldown = buttonMaxCooldown;
            }

                //manually change variables
            string oldInput = inputField;
            inputField = GUI.TextField(new Rect(255, 10, 150, 30), "Change: " + inputField).Remove(0, 8) ;
            if (oldInput != inputField )
            {
                if (state.VarCheck(inputField))
                {
                    state.VarSet(inputField, !state.VarGet(inputField));
                }
            }

            //Manually start cutscenes
            if(!inCutscene)
                cutsceneField = GUI.TextField(new Rect(255, 100, 150, 30), "Cutscene name:" + cutsceneField).Remove(0, 14);

            if(GUI.RepeatButton(new Rect(255,140,150,40),(inCutscene? "Stop" : "Start"))&& buttonCooldown <=0){
                EnableDisableCutscene();
                buttonCooldown = buttonMaxCooldown;
            }


            //Manually save/load game;
            if (GUI.RepeatButton(new Rect(410, 0, 150, 40), ("Save Game")) && buttonCooldown <= 0)
            {
                GetComponent<Holmes_Control.SaveGameHandler>().SaveGame();
                buttonCooldown = buttonMaxCooldown;
            }
            if (GUI.RepeatButton(new Rect(410, 50, 150, 40), ("Load Save")) && buttonCooldown <= 0)
            {
                GetComponent<Holmes_Control.SaveGameHandler>().LoadSaveFile();
                buttonCooldown = buttonMaxCooldown;
            }

            //Time
            GUI.TextField(new Rect(600, 0, 100, 40),""+state.time);


            //Texture
            //if (GetComponent<EvidenceHandler>().GetEvidences().Count > 0)
            //{
            //    Sprite[] aaa = GetComponent<EvidenceHandler>().GetEvidences()[0].drawings;
            //    if (aaa != null)
            //    {
            //        GUI.DrawTexture(new Rect(300, 300, 300, 200), aaa[0].texture);
            //    }
            //}
            
        }
    }


    /// <summary>
    /// Used to start and stop cutscenes with the given inputfield string "cutsceneField".
    /// </summary>
    void EnableDisableCutscene()
    {
        CutsceneHandler cutscene = GetComponent<CutsceneHandler>();

        inCutscene = !inCutscene;
        if (inCutscene)
        {
            cutscene.StartCutScene(cutsceneField);
        }
        else
        {
            cutscene.StopCutScene(cutsceneField);
        }

    }


    /// <summary>
    /// Toggles between normal Camera and Drawing Effect Camera
    /// </summary>
    void EnableDisableDrawingCamera()
    {
        GameObject mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        if(mainCam.GetComponent<Camera>().targetDisplay == 0)
        {
            mainCam.GetComponent<Camera>().targetDisplay =1;
            mainCam.transform.GetChild(0).GetComponent<Camera>().enabled = true;
        }
        else
        {
            mainCam.GetComponent<Camera>().targetDisplay = 0;
            mainCam.transform.GetChild(0).GetComponent<Camera>().enabled = false;
        }
        
    }
}
