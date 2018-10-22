using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

class DialogEditor : EditorWindow
{
    //the number of evidences hidden in the text
    int textEvidenceSize = 0;

    //the max number of evidences hidden in the text
    int textEvidenceSizeMax = 8;

    //the number of evidences hidden in the text
    int requiredVarSize = 0;
    //the max number of evidences hidden in the text
    int requiredVarsSizeMax = 8;

    //the number of evidences hidden in the text
    int setVarSize = 0;
    //the max number of evidences hidden in the text
    int setVarsSizeMax = 8;

    //use question or evidence for asking
    bool useEvidenceToAsk;

    //one time question
    bool oneTimeQuestion;

    //the question the player can ask
    string question;

    //the answer given by the npc (allready containing the parts that contain evidences)
    string answer;

    //the text that contains a specific evidence
    string[] textEvidenceText = new string[] { };
    //the id of the selected evidence
    int[] textEvidenceVar = new int[] { };
    TextEvidence[] textEvidences;

    //the id of the required evidence
    int[] requiredVars = new int[] { };
    bool[] requiredVarValue = new bool[] { };
    TextBool[] requiredEvidences;

    //the id of the required evidence
    int[] setVar = new int[] { };
    bool[] setVarValue = new bool[] { };
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

        DisplayRequiredVars();

        DisplaySetVars();

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
        if (requiredVarSize > 0)
        {
            requiredEvidences = new TextBool[requiredVarSize];
            for (int i = 0; i < requiredVarSize; i++)
            {
                requiredEvidences[i] = new TextBool(LevelEditorProperties.GetVariables().Keys.ToArray()[requiredVars[i]],requiredVarValue[i]);
            }
        }
        else
        {
            requiredEvidences = null;
        }

        //convert setEvidences to textbools
        if (setVarSize > 0)
        {
            setEvidences = new TextBool[setVarSize];
            for (int i = 0; i < setVarSize; i++)
            {
                setEvidences[i] = new TextBool(LevelEditorProperties.GetVariables().Keys.ToArray()[setVar[i]],setVarValue[i]);
            }
        }
        else
        {
            setEvidences = null;
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
        }else
        {
            textEvidences = null;
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

        File.AppendAllText(LevelEditorController.PATH_CHARACTERS + LevelEditorProperties.GetCharacters()[NPC] + ".json", DialogINJson + Environment.NewLine);
        AssetDatabase.Refresh();

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

        return "checked up!";
    }

    private void DisplaySetVars()
    {
        //recieve the number of required evidences
        setVarSize = Mathf.Clamp(EditorGUILayout.IntField("Set Variables:", setVarSize), 0, setVarsSizeMax);

        //if the number of evindences was edited, update it
        if (setVar.Length != setVarSize)
        {
            setVar = ResizeArray(setVar, setVarSize);
            setVarValue = ResizeArray(setVarValue, setVarSize);
        }

        //displays all evidences that are required
        if (setVarSize > 0)
        {
            for (int j = 0; j < setVarSize; j++)
            {
                EditorGUILayout.BeginHorizontal();
                setVar[j] = EditorGUILayout.Popup(string.Empty, setVar[j], LevelEditorProperties.GetVariables().Keys.ToArray());
                setVarValue[j] = EditorGUILayout.Toggle(setVarValue[j]);
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void DisplayRequiredVars()
    {
        //recieve the number of required evidences
        requiredVarSize = Mathf.Clamp(EditorGUILayout.IntField("Required Variables:", requiredVarSize), 0, requiredVarsSizeMax);

        //if the number of evindences was edited, update it
        if (requiredVars.Length != requiredVarSize )
        {
            requiredVars = ResizeArray(requiredVars, requiredVarSize);
            requiredVarValue = ResizeArray(requiredVarValue, requiredVarSize);
        }

        //displays all evidences that are required
        if (requiredVarSize > 0)
        {
            for (int j = 0; j < requiredVarSize; j++)
            {
                EditorGUILayout.BeginHorizontal();
                requiredVars[j] = EditorGUILayout.Popup(string.Empty, requiredVars[j], LevelEditorProperties.GetVariables().Keys.ToArray());
                requiredVarValue[j] = EditorGUILayout.Toggle(requiredVarValue[j]);
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void DisplayEvidencesInAnswer()
    {
        //recieve the number of evidences that answer has
        textEvidenceSize = Mathf.Clamp(EditorGUILayout.IntField("TextEvidences:", textEvidenceSize), 0, textEvidenceSizeMax);

        //if the number of evindences was edited, update it
        if (textEvidenceText != null && textEvidenceText.Length != textEvidenceSize)
        {
            textEvidenceText = ResizeArray(textEvidenceText, textEvidenceSize);
            textEvidenceVar = ResizeArray(textEvidenceVar, textEvidenceSize);
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

   

    
    //resize an array but keep the content
    T[] ResizeArray<T>(T[] input, int newSize)
    {
        List<T> temp = new List<T>(input);
        int sizeDiff = newSize - input.Length;

        if (newSize == 0)
        {
            return new T[0];
        }

        if (sizeDiff > 0)
        {
            temp.AddRange(new T[sizeDiff]);
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
