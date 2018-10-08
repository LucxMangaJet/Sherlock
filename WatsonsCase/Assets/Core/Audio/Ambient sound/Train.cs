using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class Train : MonoBehaviour {


	// Use this for initialization
	void Start ()
    {
        AudioSource Tictac = gameObject.GetComponent<AudioSource>();
        Tictac.Play();
    }
	
	// Update is called once per frame
	void Update ()
    {

    }
}
