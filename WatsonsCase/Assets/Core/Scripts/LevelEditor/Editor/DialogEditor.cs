using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

class DialogEditor : EditorWindow
{
    //the number of evidences hidden in the text
    int textEvidenceSize = 0;

    //the max number of evidences hidden in the text
    int textEvidenceSizeMax = 8;

    //the number of evidences hidden in the text
    int requiredEvidenceSize = 1;
    //the max number of evidences hidden in the text
    int requiredEvidenceSizeMax = 8;

    //the number of evidences hidden in the text
    int setEvidenceSize = 1;
    //the max number of evidences hidden in the text
    int setEvidenceSizeMax = 8;

    //use question or evidence for asking
    bool useEvidenceToAsk;

    //one time question
    bool oneTimeQuestion;

    //the question the player can ask
    string question;

    //the answer given by the npc (allready containing the parts that contain evidences)
    string answer;

    //the text that contains a specific evidence
    string[] textEvidenceText;
    //the id of the selected evidence
    int[] textEvidenceVar;
    TextEvidence[] textEvidences;

    //the id of the required evidence
    int[] requiredEvidence = new int[] { };
    bool[] requiredEvidenceValue = new bool[] { };
    TextBool[] requiredEvidences;

    //the id of the required evidence
    int[] setEvidence = new int[] { };
    bool[] setEvidenceValue = new bool[] { };
    TextBool[] setEvidences;

    //the id of the question evidence
    int questionEvidence;

    //the NPC the dialog content should be added to
    int NPC;

    //the folder prefix the editor uses


    string finalOutput = "checked up!";
    string output = "click the 'Check' Button to check the Dialog for Errors!";

    //the height of the q&a box
    int boxHeight = 32;

    //the width of the visual drop down menu for selecting an evidence
    int textEvidenceBoxWidth = 64;

    //scroll
    Vector2 scrollPosition = Vector2.zero;

    //Add the EditorWindow
    [MenuItem("LevelEditor/DialogEditor")]
    public static void Init()
    {
       
        EditorWindow.GetWindow(typeof(DialogEditor));
    }


    void OnGUI()
    {
        LevelEditorProperties.Setup();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        //Display the question as a Textfield
        DisplayQuestionAndAnswer();

        DisplayEvidencesInAnswer();

        DisplayRequiredEvidences();

        DisplaySetEvidences();

        //select the npc 
        NPC = EditorGUILayout.Popup("NPC Name:", NPC, LevelEditorProperties.GetCharacters().ToArray());

        //ceck for errors
        if (GUILayout.Button("Check"))
        {
            output = Check();
        }

        //show the output of the checkup
        EditorGUILayout.HelpBox(new GUIContent(output));

        //if the check was succesful
        if (output == finalOutput)
        {
            if (GUILayout.Button("Export!"))
            {
                if (Check() == finalOutput)
                {
                    Export();
                } else
                {
                    output =  Check();
                }
            }
        }

        GUILayout.EndScrollView();
    }

    private void Export()
    {
        //convert requiredEvidences to textbools
        if (requiredEvidenceSize > 0)
        {
            requiredEvidences = new TextBool[requiredEvidenceSize];
            for (int i = 0; i < requiredEvidenceSize; i++)
            {
                requiredEvidences[i] = new TextBool(LevelEditorProperties.GetTextEvidences()[requiredEvidence[i]],requiredEvidenceValue[i]);
            }
        }

        //convert setEvidences to textbools
        if (setEvidenceSize > 0)
        {
            setEvidences = new TextBool[setEvidenceSize];
            for (int i = 0; i < setEvidenceSize; i++)
            {
                setEvidences[i] = new TextBool(LevelEditorProperties.GetTextEvidences()[setEvidence[i]],setEvidenceValue[i]);
            }
        }

        //convert Evidences hidden in the answer into the TextEvidence Format
        if (textEvidenceSize > 0)
        {
            textEvidences = new TextEvidence[textEvidenceSize];
            for (int i = 0; i < textEvidenceSize; i++)
            {
                int startIndx, endIndx;
                startIndx = answer.IndexOf(textEvidenceText[i]);
                endIndx = startIndx + textEvidenceText[i].Length;

                textEvidences[i] = new TextEvidence(LevelEditorProperties.GetTextEvidences()[textEvidenceVar[i]],textEvidenceText[i],startIndx,endIndx);
            }
        }

        //construct dialog options
        DialogueOption newDialogue;
        if (useEvidenceToAsk)
        {
            string[] EvidenceToAsk = new string[1];
            EvidenceToAsk[0] = LevelEditorProperties.GetTextEvidences()[questionEvidence];

            newDialogue = new DialogueOption(false, EvidenceToAsk,answer,requiredEvidences,setEvidences,textEvidences,oneTimeQuestion);
        }
        else
        {

            newDialogue = new DialogueOption(question, answer, requiredEvidences, setEvidences, textEvidences, oneTimeQuestion);
        }

        string DialogINJson = JsonUtility.ToJson(newDialogue);

        File.AppendAllText(LevelEditorController.PATH_CHARACTERS + LevelEditorProperties.GetCharacters()[NPC] + ".json", DialogINJson);


    }

    private string Check()
    {
        if (answer == "")
        {
            return "please formulate an answer!";
        }

        if (textEvidenceSize == 0)
        {
            textEvidenceText = new string[0];
        } else
        {
            foreach (string s in textEvidenceText)
            {
                if (!answer.Contains(s))
                {
                    return s + " does not appear in the answer!";
                }
            }
        } 
        //throw new NotImplementedException();
        return "checked up!";
    }

    private void DisplaySetEvidences()
    {
        //recieve the number of required evidences
        setEvidenceSize = Mathf.Clamp(EditorGUILayout.IntField("Set Evidences:", setEvidenceSize), 0, setEvidenceSizeMax);

        //if the number of evindences was edited, update it
        if (setEvidence.Length != setEvidenceSize && setEvidenceSize != 0)
        {
            setEvidence = ResizeIntArray(setEvidence, setEvidenceSize);
            setEvidenceValue = ResizeBoolArray(setEvidenceValue, setEvidenceSize);
        }

        //displays all evidences that are required
        if (setEvidenceSize > 0)
        {
            for (int j = 0; j < setEvidenceSize; j++)
            {
                EditorGUILayout.BeginHorizontal();
                setEvidence[j] = EditorGUILayout.Popup(string.Empty, setEvidence[j], LevelEditorProperties.GetTextEvidences().ToArray());
                setEvidenceValue[j] = EditorGUILayout.Toggle(setEvidenceValue[j]);
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void DisplayRequiredEvidences()
    {
        //recieve the number of required evidences
        requiredEvidenceSize = Mathf.Clamp(EditorGUILayout.IntField("Required Evidences:", requiredEvidenceSize), 0, requiredEvidenceSizeMax);

        //if the number of evindences was edited, update it
        if (requiredEvidence.Length != requiredEvidenceSize && requiredEvidenceSize != 0)
        {
            requiredEvidence = ResizeIntArray(requiredEvidence, requiredEvidenceSize);
            requiredEvidenceValue = ResizeBoolArray(requiredEvidenceValue, requiredEvidenceSize);
        }

        //displays all evidences that are required
        if (requiredEvidenceSize > 0)
        {
            for (int j = 0; j < requiredEvidenceSize; j++)
            {
                EditorGUILayout.BeginHorizontal();
                requiredEvidence[j] = EditorGUILayout.Popup(string.Empty, requiredEvidence[j], LevelEditorProperties.GetTextEvidences().ToArray());
                requiredEvidenceValue[j] = EditorGUILayout.Toggle(requiredEvidenceValue[j]);
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void DisplayEvidencesInAnswer()
    {
        //recieve the number of evidences that answer has
        textEvidenceSize = Mathf.Clamp(EditorGUILayout.IntField("TextEvidences:", textEvidenceSize), 0, textEvidenceSizeMax);

        //if the number of evindences was edited, update it
        if (textEvidenceText != null && textEvidenceText.Length != textEvidenceSize && textEvidenceSize != 0)
        {
            textEvidenceText = ResizeStringArray(textEvidenceText, textEvidenceSize);
            textEvidenceVar = ResizeIntArray(textEvidenceVar, textEvidenceSize);
        }

        //displays all evidences for that answer
        if (textEvidenceSize > 0)
        {
            for (int i = 0; i < textEvidenceSize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                textEvidenceText[i] = EditorGUILayout.TextArea(textEvidenceText[i]);
                textEvidenceVar[i] = EditorGUILayout.Popup(string.Empty, textEvidenceVar[i], LevelEditorProperties.GetTextEvidences().ToArray(), GUILayout.MaxWidth(textEvidenceBoxWidth));
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void DisplayQuestionAndAnswer()
    {
        GUILayout.Label("Question", EditorStyles.label);

        useEvidenceToAsk = EditorGUILayout.Toggle("Use Evidence as Question", useEvidenceToAsk);
        if (useEvidenceToAsk)
        {
            questionEvidence = EditorGUILayout.Popup(string.Empty, questionEvidence, LevelEditorProperties.GetTextEvidences().ToArray());
        }
        else
        {
            question = EditorGUILayout.TextArea(question, GUILayout.MinHeight(boxHeight));
            oneTimeQuestion = EditorGUILayout.Toggle("Can only be asked once", oneTimeQuestion);
        }

        //Display the Answer as a Textfield
        GUILayout.Label("Answer", EditorStyles.label);
        answer = EditorGUILayout.TextArea(answer, GUILayout.MinHeight(boxHeight));
    }

    //resizes a string array but keeping the content
    string[] ResizeStringArray(string[] input, int newSize)
    {
        List<string> temp = new List<string>(input);
        int sizeDiff = newSize - input.Length;

        if (sizeDiff > 0)
        {
            temp.AddRange(new string[sizeDiff]);
        }
        else
        {
            temp.RemoveRange(temp.Count + sizeDiff, -sizeDiff);
        }

        return temp.ToArray();
    }

    //resizes a int array but keeping the content
    int[] ResizeIntArray(int[] input, int newSize)
    {
        List<int> temp = new List<int>(input);
        int sizeDiff = newSize - input.Length;

        if (sizeDiff > 0)
        {
            temp.AddRange(new int[sizeDiff]);
        }
        else
        {
            temp.RemoveRange(temp.Count + sizeDiff, -sizeDiff);
        }

        return temp.ToArray();
    }

    //resizes a TextBool array but keeping the content
    TextBool[] ResizeTextBoolArray(TextBool[] input, int newSize)
    {
        List<TextBool> temp = new List<TextBool>(input);
        int sizeDiff = newSize - input.Length;

        if (sizeDiff > 0)
        {
            temp.AddRange(new TextBool[sizeDiff]);
        }
        else
        {
            temp.RemoveRange(temp.Count + sizeDiff, -sizeDiff);
        }

        return temp.ToArray();
    }

    //resizes a bool array but keeping the content
    bool[] ResizeBoolArray(bool[] input, int newSize)
    {
        List<bool> temp = new List<bool>(input);
        int sizeDiff = newSize - input.Length;

        if (sizeDiff > 0)
        {
            temp.AddRange(new bool[sizeDiff]);
        }
        else
        {
            temp.RemoveRange(temp.Count + sizeDiff, -sizeDiff);
        }

        return temp.ToArray();
    }

    public static DialogEditor Instance
    {
        get { return GetWindow<DialogEditor>(); }
    }
}
