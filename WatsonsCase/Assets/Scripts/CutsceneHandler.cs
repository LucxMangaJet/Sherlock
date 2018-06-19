using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holmes_Control;
using UnityEngine.Animations;

public class CutsceneHandler : MonoBehaviour {

    GameState state;
    Animator animator;
 
    public delegate void stringDel(string s);
    public event stringDel CutSceneStartEvent; //Event called when a cutscene is started, containing the respective cutscene name as a string.
    public event stringDel CutSceneEndEvent;    // Same for end of Cutscene.
    [SerializeField] GameObject CutSceneCameraObj; //The Obj containing the camera that will do the cutscenes. Cutscenes will be done through Animations done on the camera.

    void Start () {
        state = GetComponent<GameState>();
        state.updatedVarEvent += CheckVarUpdate;
        animator = CutSceneCameraObj.GetComponent<Animator>();
	}
	
	

    /// <summary>
    /// Gets called by GameState.updatedVarEvent when a variable gets set. Triggers cutscenes when the respective variables get set;
    /// </summary>
    /// <param name="s">Name of the Variable beeing set</param>
    /// <param name="b">State the Variable is beeing set to </param>
    void CheckVarUpdate(string s, bool b)
    {
        //if correct s call CutSceneStartEvent;
    }
    /// <summary>
    /// Starts a cutscene.
    /// </summary>
    /// <param name="s">Name of the cutscene.</param>
    public void StartCutScene(string s)
    {
        Debug.Log("Started cutscene: " + s);
        CutSceneStartEvent(s);
    }
    /// <summary>
    /// Stops a cutscene.
    /// </summary>
    /// <param name="s">Name of the cutscene</param>
    public void StopCutScene(string s)
    {
        Debug.Log("Stopped cutscene: " + s);
        CutSceneEndEvent(s);
    }

}
