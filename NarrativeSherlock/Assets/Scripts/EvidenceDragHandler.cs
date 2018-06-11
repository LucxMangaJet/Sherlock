using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EvidenceDragHandler : MonoBehaviour ,IDragHandler,IEndDragHandler,IBeginDragHandler {
     RectTransform parent, myTransform,evidencesTransform;
    public GameObject i_VisualHelpAskEvidence,i_VisualHelpDelete,i_VisualHelpMerge;
     Camera mainCam;
    GameObject main,canvas;
    Transform startParent;
    public bool inDialog=false,isInMergeOption=false;
    public int selectedIndx=-1;
    Vector2 startPos = Vector2.zero;

     void Start()
    {
        myTransform = GetComponent<RectTransform>();
        evidencesTransform = GameObject.FindGameObjectWithTag("EvidencesContent").GetComponent<RectTransform>();
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

        if (inDialog)
        {
            i_VisualHelpAskEvidence.SetActive(true);
        }

        i_VisualHelpDelete.SetActive(true);
        i_VisualHelpMerge.SetActive(true);


    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localpos = Input.mousePosition;
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, Input.mousePosition, mainCam, out localpos);
        //Debug.Log(localpos);
        if (Screen.width > 1280)
        {
            localpos.x = 1280 * (localpos.x / Screen.width);
        }
        if (Screen.height> 720)
        {
            localpos.y = 720 * (localpos.y / Screen.height);
        }
        myTransform.anchoredPosition=localpos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(startParent);
        myTransform.anchoredPosition = startPos;
        if (inDialog)
            i_VisualHelpAskEvidence.SetActive(false);
        i_VisualHelpDelete.SetActive(false);
        i_VisualHelpMerge.SetActive(false);

        if (Input.mousePosition.x > Screen.width * 2 / 4)
        {
           main.GetComponent<EvidenceHandler>().AskAboutEvidence(selectedIndx);
           // Debug.Log("Asking About Evidence");

        }else if (Input.mousePosition.x < Screen.width / 4 && Input.mousePosition.y<Screen.height/5)
        {
            main.GetComponent<EvidenceHandler>().DeleteEvidence(selectedIndx);
            Debug.Log("Delete");
        }else if (Input.mousePosition.x > Screen.width *3/ 8 && Input.mousePosition.x < Screen.width * 5 / 8 && Input.mousePosition.y < Screen.height*2/ 5)
        {
            main.GetComponent<EvidenceHandler>().TryMerge(selectedIndx);
            Debug.Log("Added to Merge");
        } else
        {
            main.GetComponent<EvidenceHandler>().RefitEvidenceContent();
            Debug.Log("Dropped outside of Area");
        }

        
    }

    
}
