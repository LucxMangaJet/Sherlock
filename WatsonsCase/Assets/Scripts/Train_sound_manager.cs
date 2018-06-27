using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train_sound_manager : MonoBehaviour {

    AudioSource Sound;
    float Timer;

	// Use this for initialization
	void Start ()
    {
        Sound = gameObject.GetComponent<AudioSource>();
        Timer = 1000;

    }
	
	// Update is called once per frame
	void Update ()
    {
        Timer = Timer + 1 * Time.deltaTime;
       // Debug.Log(Timer);
        if (Timer > 1200)
        {
            Timer = 0;
            Sound.Play();
        }
       
    }
}
