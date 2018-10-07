using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoKeyPlayer : MonoBehaviour {
    public string keyName;
    AudioSource source;
     static string sequence = "";
     static string sequence2 = "";
    float timeUntilUp=0;
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlayNote()
    {
        source.Play();
        AddToSequence();
        float temp = timeUntilUp;
        timeUntilUp = 0.4f;
        if (temp <= 0)
        StartCoroutine(PushDownAnim());
        



    }
	
    IEnumerator PushDownAnim()
    {
        transform.localEulerAngles = new Vector3(3, 0, 0);
        while (timeUntilUp>0) {
            yield return null;
            timeUntilUp -= Time.deltaTime;
                }
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }

	
	
	public void AddToSequence()
    {
        sequence += keyName;
        sequence2 += keyName;

        if (sequence.Length == 5)
           sequence=sequence.Remove(0, 1);
        if (sequence.Length == 6)
            sequence = sequence.Remove(0, 2);

        if (sequence2.Length > 9)
            sequence2 = sequence2.Remove(0, 1);
        if (sequence.Length == 10)
            sequence = sequence.Remove(0, 2);


        if (sequence == "FDFC")
        {

            GameObject g = GameObject.FindGameObjectWithTag("PianoPannel");
            if(g!=null)
                g.GetComponent<PianoPannelOpener>().StartOpeningClosing();
            
        }
        if(sequence2 == "EDCEDCEDC")
        {
            GameObject.FindGameObjectWithTag("Main").GetComponent<SoundHandler>().PlayClip(SoundHandler.ClipEnum.AutoPlayingMusic2 ,SoundHandler.OutputEnum.Piano);
        }
        if (sequence2 == "EDCEDCEDC")
        {
            GameObject.FindGameObjectWithTag("Main").GetComponent<SoundHandler>().PlayClip(SoundHandler.ClipEnum.AutoPlayingMusic2, SoundHandler.OutputEnum.Piano);
        }
    }

}
