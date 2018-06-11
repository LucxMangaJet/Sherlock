using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogDragHandler : MonoBehaviour ,IDragHandler,IEndDragHandler,IBeginDragHandler {
     RectTransform dialogLogContent, myTransform,evidencesTransform;
     Camera mainCam;
    GameObject main;
    public bool inMenu=false;
    public int selectedIndx=-1;
    public int selectedIndx2 = -1;

    Vector2 startPos = Vector2.zero;

     void Start()
    {
        myTransform = GetComponent<RectTransform>();
        dialogLogContent = myTransform.parent.GetComponent<RectTransform>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        evidencesTransform = GameObject.FindGameObjectWithTag("EvidencesContent").GetComponent<RectTransform>();
        main = GameObject.FindGameObjectWithTag("Main");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //activate "Drop Here to add to evidences Obj"
        startPos = myTransform.anchoredPosition;
        //Debug.Log(startPos);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localpos= Vector2.zero;
        
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dialogLogContent, Input.mousePosition, mainCam, out localpos);
       // Debug.Log(localpos);
        if (!inMenu)
            localpos += new Vector2(+200, -100);
        myTransform.anchoredPosition=localpos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Add to evidences if in Evidences Rect
        if (Input.mousePosition.x<Screen.width/4)
        {
            Debug.Log("Added Evidence");
            //add evidence
            if (inMenu)
            {
                main.GetComponent<Holmes_Menu.MenuHandler>().HighlightText(selectedIndx, selectedIndx2);
                main.GetComponent<Holmes_Menu.MenuHandler>().SaveSelectedLogSentence();
            }
            else
            {
                main.GetComponent<DialogueHandler>().HighlightText(selectedIndx);
                main.GetComponent<DialogueHandler>().SaveSelection();
            }
        }
        else
        {
            Debug.Log("Dropped outside of Evidences");
        }
        myTransform.anchoredPosition = startPos;
    }

    
}
