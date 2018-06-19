using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holmes_Menu;
using UnityEngine.UI;

public class EvidenceHandler : MonoBehaviour {
    [SerializeField] GameObject i_Merge, i_Delete, i_AskAbout, i_EvidenceObj,i_VisualHelpDelete,i_VisualHelpMerge,I_VisualHelp_Ask,ui_VisualHelpAddEvidence;
    [SerializeField] RectTransform i_EvidencesTransform;
    [SerializeField] GameObject pref_Evidence, pref_Drawing, pref_Text;

    private MenuHandler menu;
    private SoundHandler sound;

    //transitions
    public bool inDialogue = false;
    public bool inMenu = false;
    bool hoveringOverEvidences = false;
    bool inhoverTransition = false;

    List<GameObject> postIts;
    [SerializeField] List<UnitedEvidence> evidences;
    [SerializeField] List<UnitedEvidence> selectedEvidences;
    List<int> selectedEvidencesIndx;
    int lookedatEvidenceIndx = 0;
    int evidenceInMergeOption = -1;

    //events
    public delegate void GivenInDel(string[] s,string phase);
    public event GivenInDel GivenInEvidence;

    private void Awake()
    {
        postIts = new List<GameObject>();
        evidences = new List<UnitedEvidence>();
        selectedEvidences = new List<UnitedEvidence>();
        selectedEvidencesIndx = new List<int>();
        menu = GetComponent<MenuHandler>();
        sound = GetComponent<SoundHandler>();

        GetComponent<MenuHandler>().enterMenu += EnterMenu;
        GetComponent<MenuHandler>().exitMenu += ExitMenu;
        GetComponent<DialogueHandler>().enterDialogueEvent += EnterDialog;
        GetComponent<DialogueHandler>().exitDialogueEvent += ExitDialog;
    }


    private void Update()
    {
        if (!inDialogue && !inMenu)
            return;
        if (hoveringOverEvidences)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                ScrollTroughEvidences(true);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                ScrollTroughEvidences(false);
            }
        }
    }

    public void AddDrawing(Drawing d)
    {
        evidences.Add(new UnitedEvidence(null, new Drawing[1] { d }));
    }

    public void AddTextEvidence(SavedTextEvidence s)
    {
        evidences.Add(new UnitedEvidence(new SavedTextEvidence[1] { s }, null));
        RefitEvidenceContent();
    }

    public void SetEvidences(List<UnitedEvidence> e)
    {
        evidences = e;
    }

    public List<UnitedEvidence> GetEvidences()
    {
        return evidences;
    }

    public void AddEvidence(UnitedEvidence e)
    {
        evidences.Add(e);
    }

    public bool RemoveEvidence(UnitedEvidence s)
    {
        return evidences.Remove(s);
    }



    //EventsInteraction;

    void EnterDialog()
    {
        inDialogue = true;
        i_EvidenceObj.SetActive(true);
    }

    void ExitDialog()
    {
        inDialogue = false;
        i_EvidenceObj.SetActive(inMenu);
    }

    void EnterMenu()
    {
        inMenu = true;
        i_EvidenceObj.SetActive(true);
    }

    void ExitMenu()
    {
        inMenu = false;
        i_EvidenceObj.SetActive(inDialogue);
    }

    //interactions

    public void SelectEvidence(int i)
    {

        if (i >= 0)
        {
            Debug.Log("Selecting Evidence");
            if (selectedEvidences.Contains(evidences[i]))
            {
                selectedEvidences.Remove(evidences[i]);
                selectedEvidencesIndx.Remove(i);
            }
            else
            {
                selectedEvidences.Add(evidences[i]);
                selectedEvidencesIndx.Add(i);
            }
        }
        else
        {
            selectedEvidences.Clear();
            selectedEvidencesIndx.Clear();
            selectedEvidences.Add(evidences[i]);
            selectedEvidencesIndx.Add(i);
        }


        //Debug.Log(selectedTextEvidence.text);
        //for (int j = 0; j < postIts.Count; j++)
        //{
        //    GameObject g = postIts[j];
        //    if (selectedEvidencesIndx.Contains(int.Parse(g.name)))
        //    {
        //        Vector2 v = g.GetComponent<RectTransform>().anchoredPosition;
        //        g.GetComponent<RectTransform>().anchoredPosition = new Vector2(140, v.y); ;
        //    }
        //    else
        //    {
        //        Vector2 v = g.GetComponent<RectTransform>().anchoredPosition;
        //        g.GetComponent<RectTransform>().anchoredPosition = new Vector2(125, v.y); ;
        //    }
        //}

        //if (inDialogue && selectedEvidences.Count == 1)
        //{
        //    i_AskAbout.SetActive(true);
        //}
        //else
        //{
        //    i_AskAbout.SetActive(false);
        //}

        //if (selectedEvidences.Count == 0)
        //{
        //    i_Delete.SetActive(false);
        //}
        //else
        //{
        //    i_Delete.SetActive(true);
        //}

        //if (selectedEvidences.Count > 1)
        //{
        //    i_Merge.SetActive(true);
        //}
        //else
        //{
        //    i_Merge.SetActive(false);
        //}

    }

    //inProgress
    public void RefitEvidenceContent()
    {
        for (int i = 0; i < postIts.Count; i++)
        {
            Destroy(postIts[i]);
        }
        postIts.Clear();
        float boxWidth = 250;
        //float boxHeight = 0;
        int limit = evidences.Count >= 6 ? 6 : evidences.Count;

        //Create all Postits
        for (int k = 0; k < limit; k++)
        {
            int i = lookedatEvidenceIndx + k;

            if (i >= evidences.Count)
            {
                i = i - evidences.Count;
            }



            float minHeight = -187;
            float boxHeight = 0;
            UnitedEvidence s = evidences[i];
            GameObject g = Instantiate(pref_Evidence, i_EvidencesTransform);
            if (s.autors == null)
            {
                g.transform.GetChild(0).GetComponent<Image>().enabled = false;
            }

            g.name = "" + i;
            postIts.Add(g);
            int param1 = i;
            g.GetComponent<EvidenceDragHandler>().i_VisualHelpAskEvidence = i_AskAbout;
            g.GetComponent<EvidenceDragHandler>().i_VisualHelpMerge = i_Merge;
            g.GetComponent<EvidenceDragHandler>().i_VisualHelpDelete = i_Delete;
            g.GetComponent<EvidenceDragHandler>().selectedIndx = param1;
            g.GetComponent<EvidenceDragHandler>().inDialog = inDialogue;
            //g.GetComponent<Button>().onClick.AddListener(delegate { SelectEvidence(param1); });

            if (s.autors != null)
            {
                for (int j = 0; j < s.autors.Length; j++)
                {
                    float height = boxHeight;

                    string text = "<color=maroon>" + s.autors[j] + ": " + "</color>" + s.texts[j];
                    GameObject go = Instantiate(pref_Text, g.transform);
                    RectTransform rect1 = go.GetComponent<RectTransform>();
                    go.GetComponent<Text>().text = text;
                    float width1 = GetComponent<DialogueHandler>().GetWordWidth(text);
                    float height1 = Mathf.Ceil(width1 / boxWidth);
                    width1 = (height1 > 1) ? boxWidth : width1;
                    height -= (height1 / 2) * 18;
                    rect1.sizeDelta = new Vector2(width1, height1 * 18);
                    rect1.anchoredPosition = new Vector2(width1 / 2, height);
                    height -= (height1 / 2) * 18;
                    boxHeight = height;
                }
            }

            if (s.drawings != null)
            {
                for (int o = 0; o < s.drawings.Length; o++)
                {
                    boxHeight -= 94;
                    GameObject go = Instantiate(pref_Drawing, g.transform);
                    RectTransform rect1 = go.GetComponent<RectTransform>();
                    go.transform.GetChild(1).GetComponent<Image>().sprite = s.drawings[0];
                    rect1.anchoredPosition = new Vector2(boxWidth / 2, boxHeight);
                    boxHeight -= 94;
                }
            }
            float h = Mathf.Min(minHeight, boxHeight);
            g.GetComponent<RectTransform>().sizeDelta = new Vector2(boxWidth, Mathf.Abs(h));
            //  g.GetComponent<RectTransform>().anchoredPosition = new Vector2(boxWidth/2,h/2);
        }


        //PlacePostits;
        float posy =  Screen.height * 0.5f;
        for (int i = 0; i < postIts.Count; i++)
        {
            float sizey = postIts[i].GetComponent<RectTransform>().sizeDelta.y;
            postIts[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(boxWidth / 2, posy - sizey / 2);
            postIts[i].transform.SetSiblingIndex(postIts.Count - 1 - i);
            posy += 25;
        }



        //DisplayMergeObj

        float minHeightM = -187;
        float boxHeightM = 0;
        if (evidenceInMergeOption >= 0)
        {

            UnitedEvidence sM = evidences[evidenceInMergeOption];
            GameObject gM = Instantiate(pref_Evidence, i_EvidenceObj.transform);
            if (sM.autors == null)
            {
                gM.transform.GetChild(0).GetComponent<Image>().enabled = false;
            }

            gM.name = "" + evidenceInMergeOption;
            postIts.Add(gM);
            int param1M = evidenceInMergeOption;
            gM.GetComponent<EvidenceDragHandler>().i_VisualHelpAskEvidence = i_AskAbout;
            gM.GetComponent<EvidenceDragHandler>().i_VisualHelpMerge = i_Merge;
            gM.GetComponent<EvidenceDragHandler>().i_VisualHelpDelete = i_Delete;
            gM.GetComponent<EvidenceDragHandler>().selectedIndx = param1M;
            gM.GetComponent<EvidenceDragHandler>().inDialog = inDialogue;
            gM.GetComponent<EvidenceDragHandler>().isInMergeOption = true;
            //g.GetComponent<Button>().onClick.AddListener(delegate { SelectEvidence(param1); });

            if (sM.autors != null)
            {
                for (int j = 0; j < sM.autors.Length; j++)
                {
                    float height = boxHeightM;

                    string text = "<color=maroon>" + sM.autors[j] + ": " + "</color>" + sM.texts[j];
                    GameObject go = Instantiate(pref_Text, gM.transform);
                    RectTransform rect1 = go.GetComponent<RectTransform>();
                    go.GetComponent<Text>().text = text;
                    float width1 = GetComponent<DialogueHandler>().GetWordWidth(text);
                    float height1 = Mathf.Ceil(width1 / boxWidth);
                    width1 = (height1 > 1) ? boxWidth : width1;
                    height -= (height1 / 2) * 18;
                    rect1.sizeDelta = new Vector2(width1, height1 * 18);
                    rect1.anchoredPosition = new Vector2(width1 / 2, height);
                    height -= (height1 / 2) * 18;
                    boxHeightM = height;
                }
            }

            if (sM.drawings != null)
            {
                for (int o = 0; o < sM.drawings.Length; o++)
                {
                    boxHeightM -= 94;
                    GameObject go = Instantiate(pref_Drawing, gM.transform);
                    RectTransform rect1 = go.GetComponent<RectTransform>();
                    go.transform.GetChild(1).GetComponent<Image>().sprite = sM.drawings[0];
                    rect1.anchoredPosition = new Vector2(boxWidth / 2, boxHeightM);
                    boxHeightM -= 94;
                }
            }
            float hM = Mathf.Min(minHeightM, boxHeightM);
            gM.GetComponent<RectTransform>().sizeDelta = new Vector2(boxWidth, Mathf.Abs(hM));
            //  g.GetComponent<RectTransform>().anchoredPosition = new Vector2(boxWidth/2,h/2);

            float sizeyM = gM.GetComponent<RectTransform>().sizeDelta.y;
            float width = (Screen.width > 1280) ? 640 : Screen.width / 2;
            gM.GetComponent<RectTransform>().anchoredPosition = new Vector2( width, +Screen.height/4 - sizeyM / 2);
            Debug.Log(Screen.width / 2);
            Debug.Log(gM.GetComponent<RectTransform>().anchoredPosition.x);
        }
    }

    void ScrollTroughEvidences(bool up)
    {
        lookedatEvidenceIndx += up ? 1 : -1;

        if (lookedatEvidenceIndx < 0)
        {
            lookedatEvidenceIndx = evidences.Count - 1;
        }
        else if (lookedatEvidenceIndx >= evidences.Count)
        {
            lookedatEvidenceIndx = 0;
        }

        RefitEvidenceContent();
        //SelectEvidence(-1);
    }
    

    public void AskAboutEvidence(int i)
    {
        Debug.Log("Asking About Evidence");
        DialogueHandler dh = GetComponent<DialogueHandler>();
        Character c = dh.partner.GetComponent<Character>();
        DialogueOption[] _evidences = c.aviableEvidenceOptions;
        DialogueOption o = c.option_noAnswer;
        

        //if (selectedEvidences.Count > 1)
        //    return;

        UnitedEvidence selectedEvidence = evidences[i];

        

        //single evidence search
            
                foreach (string s in selectedEvidence.evidences)
                {
                    foreach (DialogueOption d in _evidences)
                    {
                    if (selectedEvidence.evidences.Length >= d.reqEvidence.Length && d.reqEvidence.Length==1)
                    {
                        foreach (string str in d.reqEvidence)
                        {
                            if (s == str)
                                o = d;
                        }
                    }
                }
            }

           // Debug.Log(selectedEvidence.evidences.Length);
            //multiple evidence search
            if (selectedEvidence.evidences.Length > 1)
            {
                foreach (DialogueOption d in _evidences)
                {
                    //Debug.Log(selectedEvidence.evidences.Length+ " : " + d.reqEvidence.Length);
                    if (selectedEvidence.evidences.Length == d.reqEvidence.Length)
                    {
                        bool goodEvidence = true;
                        foreach (string str in d.reqEvidence)
                        {
                            bool strIsContained = false;
                            foreach (string str2 in selectedEvidence.evidences)
                            {
                                if (str == str2)
                                {
                                    strIsContained = true;
                                }
                            }
                            if (!strIsContained)
                            {
                                goodEvidence = false;
                            }
                        }
                        if (goodEvidence)
                        {
                            o = d;
                        }
                    }
                }
            }

        //Special End game await evidence input 
        if (c.myName == "Sherlock")
        {
            bool CallEvent = false;
            foreach (DialogueOption e in _evidences)
            {
                foreach (string req in e.reqEvidence)
                {
                    if (req == "ANY")
                    {
                        o = e;
                        CallEvent = true;
                    }
                }
            }

            
            //TO WRITE

            if (CallEvent && GivenInEvidence != null)
                GivenInEvidence(selectedEvidence.evidences,o.reqTB[0].name);

        }
        dh.PickDialogue(o);
    }

    public void DeleteEvidence(int i)
    {
        Debug.Log("Deleting Evidence");

        //foreach (UnitedEvidence d in selectedEvidences)
        //{
        //    evidences.Remove(d);
        //}

        //selectedEvidences.Clear();
        //selectedEvidencesIndx.Clear();
        //SelectEvidence(-1); //resets the color changing on the evidences
        //i_Delete.SetActive(false);
        //i_AskAbout.SetActive(false);


        if (selectedEvidences.Contains(evidences[i]))
        {
            selectedEvidences.Remove(evidences[i]);
            if (selectedEvidences.Count < 1)
            {
                evidenceInMergeOption = -1;
            }
        }
        else
        {
         evidences.Remove(evidences[i]);
        }
        RefitEvidenceContent();

        sound.PlayClip(SoundHandler.ClipEnum.UICrossOut, SoundHandler.OutputEnum.UI);
    }
    
    public void MergeTextEvidences()
    {
        Debug.Log("Merging Evidences");

        if (selectedEvidences.Count < 2)
        {
            Debug.LogError("Trying to merge less then 2 evidences");
            return;
        }

        AddEvidence(new UnitedEvidence(selectedEvidences.ToArray()));

        
        int len = 0;

        foreach( UnitedEvidence e in selectedEvidences)
        {
            if (e.texts != null)
            {
                foreach (string s in e.texts)
                    len += s.Length;
            }
           // RemoveEvidence(e);
        }

        selectedEvidences.Clear();
        selectedEvidencesIndx.Clear();


        lookedatEvidenceIndx = evidences.Count - 1;
        RefitEvidenceContent();
        

        //i_Delete.SetActive(false);
        //i_Merge.SetActive(false);
        //i_AskAbout.SetActive(false);
        sound.PlayClip(len < 8 ? SoundHandler.ClipEnum.UIWritingShort : len < 20 ? SoundHandler.ClipEnum.UIWritingMid : SoundHandler.ClipEnum.UIWritingLong, SoundHandler.OutputEnum.UI);
    }

    public void TryMerge(int i)
    {
        if (selectedEvidences.Count >= 1)
        {
            selectedEvidences.Add(evidences[i]);
            evidenceInMergeOption = -1;
            MergeTextEvidences();
        }
        else
        {
            evidenceInMergeOption = i;
            selectedEvidences.Add(evidences[i]);
        }

        //Debug.Log(selectedEvidences.Count);
        RefitEvidenceContent();
    }


    //Transitions and such

    public void HoverStarted()
    {
        hoveringOverEvidences = true;

        StartCoroutine(EvidenceTransition(true));
        ui_VisualHelpAddEvidence.SetActive(false);
        
    }

    public void HoverEnded()
    {
        hoveringOverEvidences = false;

        StartCoroutine(EvidenceTransition(false));
        ui_VisualHelpAddEvidence.SetActive(true);
        
    }

    IEnumerator EvidenceTransition(bool start)
    {

        float time = 0.3f;
        float timeSinceStart= (start?0:time);
        float xSt = -30, xEnd = 160;

        while (inhoverTransition)
        {
            yield return null;
        }
        if (start && i_EvidencesTransform.anchoredPosition.x==xEnd)
            yield break;
        if (!start && i_EvidencesTransform.anchoredPosition.x == xSt)
            yield break;

        inhoverTransition = true;
        while (true)
        {
            i_EvidencesTransform.anchoredPosition = new Vector2(Mathf.Lerp(xSt, xEnd, timeSinceStart / time), i_EvidencesTransform.anchoredPosition.y);
            if (start)
            {
                if (timeSinceStart / time > 1)
                    break;
            }
            else
            {
                if (timeSinceStart / time < 0)
                    break;
            }
            yield return null;
            timeSinceStart += Time.deltaTime * (start ? 1 : -1);


        }
        inhoverTransition = false;



    }
}
