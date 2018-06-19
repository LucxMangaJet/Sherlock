using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoKeyPlayer : MonoBehaviour {
    public string keyName;
    AudioSource source;
    static string Sequence = "";
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
        Sequence += keyName;
        if (Sequence.Length > 4)
           Sequence=Sequence.Remove(0, 1);
        if (Sequence == "FDFC")
        {
            GameObject g = GameObject.FindGameObjectWithTag("PianoPannel");
            if(g!=null)
                g.GetComponent<PianoPannelOpener>().StartOpening();
        }
    }

}
