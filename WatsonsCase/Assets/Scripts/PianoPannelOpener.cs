using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoPannelOpener : MonoBehaviour {
    [SerializeField] GameObject endPosObj;
    [SerializeField] float time;
    public void StartOpening()
    {
     
        gameObject.tag = "Untagged";
        StartCoroutine("Open");
    }

    IEnumerator Open()
    {

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

        transform.position = endPos;
    }
	
	
}
