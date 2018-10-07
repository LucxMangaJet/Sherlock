using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Holmes_Control;
using Holmes_Menu;
using UnityEngine.UI;

public class TutorialHandler : MonoBehaviour {
    [SerializeField] GameObject doorEnd,UiTasksText,UiDescriptionText;
 
    GameState state;
    // VideoPlayer player;
    // Vector2 videoPlayerStartPos;

    string controls = "Controls:\nWalking - W / A / S / D \nJump - Space \nLog - E / I / TAB\nDrawing - Q / P\nSwitch Evidences - Mousewheel\nSave and Quit Game - F4 \nSave Game - F5\nLoad Game - F9\n(Saving and Loading is disabled in the tutorial)\n";
    bool videoIsOpen = true;
    bool inTransition = false;
    GameObject Light;

	void Start () {
        state = GetComponent<GameState>();
      //  player = videoObj.GetComponent<VideoPlayer>();
        state.updatedVarEvent += CheckVarUpdate;
        CheckVarUpdate("Start", true);
        // videoObj.SetActive(false);
        //RRR.SetActive(true);
        //videoPlayerStartPos = videoObj.GetComponent<RectTransform>().anchoredPosition;


        Light = GameObject.FindWithTag("Lighttutorial");
        Light.SetActive(false);

        GetComponent<DialogueHandler>().enterDialogueEvent += Deactivate;
       GetComponent<DialogueHandler>().exitDialogueEvent += Activate;
       GetComponent<MenuHandler>().enterMenu += Deactivate;
       GetComponent<MenuHandler>().exitMenu += Activate;
       GetComponent<EvidenceDetecting>().enterDrawing += Deactivate;
       GetComponent<EvidenceDetecting>().exitDrawing += Activate;

        SetTask("Task: Talk to Sherlock.", controls);

    }

    public void Deactivate()
    {
       // tutorialUIObj.SetActive(false);
        //StartCoroutine(PanInOutVideoPlayer(false));
        //player.Stop();
    }

    public void Activate()
    {
        //tutorialUIObj.SetActive(true);
    }

    private void Update()
    {
        //if (Input.GetMouseButtonDown(1)&& !inTransition)
        //{
        //    OpenCloseVideo();
        //}

        //if (player.isPlaying)
        //{
        //    Debug.Log(player.time);
        //}
    }

    //void OpenCloseVideo()
    //{
    //    videoIsOpen = !videoIsOpen;
    //   // Debug.Log(videoIsOpen);
    //    if (videoIsOpen)
    //    {
    //       StartCoroutine(PlayVideo());
    //    }
    //    else
    //    {
    //      //  player.Stop();
    //      //  player.Prepare();
    //       // videoObj.GetComponent<AudioSource>().Stop();
    //    }
    //    StartCoroutine(PanInOutVideoPlayer(videoIsOpen));

    //}

    void CheckVarUpdate(string s,bool b)
    {
        //Debug.Log("VarUpdateCalled: " + s + " " + b);
        switch (s){

            case "FinishedTutorial":
                Debug.Log("Unlocking End Door");
                doorEnd.GetComponentInChildren<DoorMovement>().isLocked = false;
                doorEnd.GetComponentInChildren<DoorMovement>().Open();
                Light.SetActive(true);
                GameObject.FindGameObjectWithTag("Main").GetComponent<SoundHandler>().PlayClip(SoundHandler.ClipEnum.DoorOpening, SoundHandler.OutputEnum.UI);


                SetTask("Task: Go to the concert.","When you are ready leave the house.");
                break;
            case "TellSherlockName":
                SetTask("Task: Read the letter and tell Sherlock who the sender is.", controls +"You can drag and drop text into your evidences from a dialog and from the log.");
                break;

            case "TellSherlockRelation":
                SetTask("Task: Take a drawing of the name on the violin.", controls +"Drawings will be saved in your evidences.");
                break;
                //case "ReadInvitation":
                //    if (b)
                //        door1.GetComponentInChildren<DoorMovement>().isLocked = false;
                //    LoadClip(v_DoorAndTalk,"Interaction");
                //        //Display: Opening doors and talking with characters
                //    break;
                //case "FinishedTutorial":
                //    if (b)
                //        door2.GetComponentInChildren<DoorMovement>().isLocked = false;
                //    break;

                //case "Start":
                //    LoadClip(v_MoveAndObj,"Basics");
                //    //Display: movement and interacting with things Vid
                //    break;

                //case "AskedAboutGoing":
                //    LoadClip(v_MenuEvLogAndAsk,"Menu");
                //    //Display: Navigating Menu, Saving Evidences, Dialogue Log, Asking About Evidences
                //    break;
                //case "Taunted":
                //    LoadClip(v_DrawAndAsk,"Drawings");
                //    //Display: Make Drawings ask about Drawings
                //    break;

                //case "SignatureFromViolin":
                //    LoadClip(v_MultAndMerge,"Advanced Interactions");
                //    //Display: Merging Evidences and Multiple Evidences Selection
                //    break;
                //case "Rick":
                //    LoadClip(v_RRR, "ERROR CORRUPTED FILE");
                //    break;
                //default:
                //    break;
        }


    }

    void SetTask(string s,string s2)
    {
        UiTasksText.GetComponent<Text>().text = "<b><i>" + s+"</i> </b>";
        UiDescriptionText.GetComponent<Text>().text = "<i>"+s2+"</i>";
    }

    //void LoadClip(VideoClip c,string s)
    //{
    //    descriptionObj.GetComponent<Text>().text = "Tutorial: "+s;
    //    player.clip = c;
    //    player.audioOutputMode = VideoAudioOutputMode.AudioSource;
    //    player.EnableAudioTrack(0, true);
    //    player.SetTargetAudioSource(0, videoObj.GetComponent<AudioSource>());
    //    player.Prepare();
    //}

    //IEnumerator PanInOutVideoPlayer(bool b)
    //{
    //    Debug.Log("Starting Pan");
    //    inTransition = true;
    //    RectTransform r = videoObj.GetComponent<RectTransform>();
    //    const int amount = 450;
    //    while (true)
    //    {
    //        if (b)
    //        {
    //            if(r.anchoredPosition.x <= videoPlayerStartPos.x)
    //            {
    //                r.anchoredPosition = videoPlayerStartPos;
    //                break;
    //            }
    //            else
    //            {
    //                r.anchoredPosition = new Vector2(r.anchoredPosition.x -10 , r.anchoredPosition.y);
    //            }
    //        }
    //        else
    //        {
    //            if (r.anchoredPosition.x >= videoPlayerStartPos.x+amount)
    //            {
    //                r.anchoredPosition = new Vector2(videoPlayerStartPos.x + amount,videoPlayerStartPos.y);
    //                break;
    //            }
    //            else
    //            {
    //                r.anchoredPosition = new Vector2(r.anchoredPosition.x + 10, r.anchoredPosition.y);
    //            }

    //        }
    //        yield return null;
    //    }
    //    inTransition = false;
    //    Debug.Log("Pan ended");
    //}

    //IEnumerator PlayVideo() { 

    //    while (!player.isPrepared)
    //    {
    //        yield return null;
    //    }
    //    player.Play();
    //    Debug.Log("Playing:" + player.isPlaying);
       
    //    videoObj.GetComponent<AudioSource>().Play();



    //}
}
