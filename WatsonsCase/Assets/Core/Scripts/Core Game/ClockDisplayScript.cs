using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockDisplayScript : MonoBehaviour {

    private Holmes_Control.GameState state;
    [SerializeField] RectTransform i_sec, i_min, i_hour;
    // Use this for initialization
    private void Start()
    {
         state = GameObject.FindGameObjectWithTag("Main").GetComponent<Holmes_Control.GameState>();
    }

    // Update is called once per frame
    void Update () {
        float t = state.time;
        int sec = Mathf.FloorToInt(t % 60);
        int min = Mathf.FloorToInt((t / 60) % 60);
        int hour = Mathf.FloorToInt((t / (60 * 60)) % 12);
       //  Debug.Log(hour + " " + min + " " + sec);
        i_sec.localEulerAngles = new Vector3(0, 0,- sec*6);
        i_min.localEulerAngles = new Vector3(0, 0, -min*6);
        i_hour.localEulerAngles = new Vector3(0, 0,- hour*6);
    }

}
