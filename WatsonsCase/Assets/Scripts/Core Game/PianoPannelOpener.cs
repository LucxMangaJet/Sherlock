using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoPannelOpener : MonoBehaviour {
    [SerializeField] GameObject endPosObj,pianoEvidencetoDisable;
    Vector3 startPosition;
    [SerializeField] float time;


    bool open = false;
    bool inTransition = false;

    private void Start()
    {
        startPosition = transform.position;

    }
    public void StartOpeningClosing()
    {
        if (inTransition)
            return;

        if (!open)
        {
            StartCoroutine("Open");
        }
        else
        {
            StartCoroutine("Close");
        }
        
    }

    IEnumerator Open()
    {
        pianoEvidencetoDisable.tag = "Untagged";
        inTransition = true;
        GameObject main = GameObject.FindGameObjectWithTag("Main");
        main.GetComponent<SoundHandler>().PlayClip(SoundHandler.ClipEnum.AutoPlayingMusic, SoundHandler.OutputEnum.Piano);
        main.GetComponent<JsonMIDItoUnity.PlayMIDI>().Play();
        Vector3 startPos = transform.position;
        Vector3 endPos = endPosObj.transform.position;
       // Debug.Log(startPos);
        //Debug.Log(endPos);
        float t = 0;
        while (t<time)
        {
            //Debug.Log(transform.position);
            transform.position = Vector3.Lerp(startPos, endPos,t/time);

            yield return null;
            t += Time.deltaTime;
        }
        open = true;
        inTransition = false;
        transform.position = endPos;
    }
    IEnumerator Close()
    {
        pianoEvidencetoDisable.tag = "Evidence";
        inTransition = true;
        Vector3 startPos = endPosObj.transform.position;
        Vector3 endPos = startPosition;
        // Debug.Log(startPos);
        //Debug.Log(endPos);
        float t = 0;
        while (t < time)
        {
            //Debug.Log(transform.position);
            transform.position = Vector3.Lerp(startPos, endPos, t / time);

            yield return null;
            t += Time.deltaTime;
        }
        inTransition = false;
        open = false;
        transform.position = endPos;
        GameObject main = GameObject.FindGameObjectWithTag("Main");
        main.GetComponent<JsonMIDItoUnity.PlayMIDI>().shouldPlay = false;
    }




}
