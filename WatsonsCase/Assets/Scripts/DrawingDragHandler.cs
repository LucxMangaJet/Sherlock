using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DrawingDragHandler : MonoBehaviour ,IDragHandler,IEndDragHandler,IBeginDragHandler {
     RectTransform parent, myTransform;
     Camera mainCam;
    GameObject main,canvas;
    Transform startParent;
    Vector2 startPos = Vector2.zero;

     void Start()
    {
        myTransform = GetComponent<RectTransform>();
        //evidencesTransform = GameObject.FindGameObjectWithTag("EvidencesContent").GetComponent<RectTransform>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        main = GameObject.FindGameObjectWithTag("Main");
        canvas = GameObject.FindGameObjectWithTag("Menu");
        parent = canvas.GetComponent<RectTransform>(); 
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //activate "Drop Here to add to evidences Obj"
        startParent = transform.parent;
        startPos = myTransform.anchoredPosition;

        transform.SetParent(canvas.transform);
        //i_VisualHelpAddEvidence.SetActive(true);
        //Debug.Log(startPos);

        


    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localpos = Input.mousePosition;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, Input.mousePosition, mainCam, out localpos);
        //Debug.Log(localpos);

        myTransform.anchoredPosition=Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(startParent);
        myTransform.anchoredPosition = startPos;
        

        if (Input.mousePosition.x > Screen.width * 3 / 4)
        {
            main.GetComponent<EvidenceDetecting>().OperateButton(false);
        } else if (Input.mousePosition.x < Screen.width  / 4)
        {
            main.GetComponent<EvidenceDetecting>().OperateButton(true);
        }
        else
        {
            main.GetComponent<EvidenceHandler>().RefitEvidenceContent();
            Debug.Log("Dropped outside of Area");
        }

        
    }

    
}
