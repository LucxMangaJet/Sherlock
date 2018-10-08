using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

public class GameLogicWindow : EditorWindow
{

    //set SpawnPoint
    Transform spawnPointObjContent=null;

    //vars
    string varNameContent="";
    bool varSetContent=false;

    //evidence
    string evidenceNameContent = "";

    //obj evidence
    string objNameContent = "";

    //scroll
    Vector2 scrollPosition= Vector2.zero;


    [MenuItem("LevelEditor/GameLogicWindow")]
    static void Init()
    {
        LevelEditorProperties.Setup(); //remove me later

        GameLogicWindow window = (GameLogicWindow)EditorWindow.GetWindow(typeof(GameLogicWindow));
        window.Show();
    }

 
    void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUIVariables();
        GUILayout.Space(10);
        GUIEvidences();
        GUILayout.Space(10);
        GUIObjEvidences();
        GUILayout.Space(10);
        GUISpawnPoint();


        //remove me later
        if(GUILayout.RepeatButton("TEST: Create Evidence .txt")){
            LevelEditorProperties.SaveVariablesAndEvidencesIntoTextfile();
        }

        GUILayout.EndScrollView();
    }

    private void GUIVariables()
    {
        GUILayout.Label("Variables", EditorStyles.boldLabel);
        GUILayout.Label("Use this section to define, edit and remove Variables, used in dialogues.", EditorStyles.helpBox);


        varNameContent = GUILayout.TextField(varNameContent);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Starting state:");
        varSetContent = GUILayout.Toggle(varSetContent, "");
        GUILayout.EndHorizontal();

        if (GUILayout.RepeatButton("Add/Change Variable"))
        {
            if (varNameContent != "")
            {
                AddOrChangeVariable();
            }
        }

        if (GUILayout.RepeatButton("Remove Variable"))
        {
            if (varNameContent != "")
            {
                try
                {
                    RemoveVariable();
                }
                catch
                {

                }
            }
        }

        GUILayout.Label("Current Variables:");

        string variablesString = "";

        foreach (KeyValuePair<string, bool> var in LevelEditorProperties.GetVariables())
        {
            variablesString += var.Key + "  " + (var.Value ? "✔" : "X") + "\n";
        }

        GUILayout.Label(variablesString, EditorStyles.helpBox);
    }

    private void AddOrChangeVariable()
    {
        LevelEditorProperties.AddOrChangeVariable(varNameContent.Trim(), varSetContent);
    }

    private void RemoveVariable()
    {
        LevelEditorProperties.RemoveVariable(varNameContent);
    }

    private void GUIEvidences()
    {
        GUILayout.Label("Text Evidences", EditorStyles.boldLabel);
        GUILayout.Label("Use this section to define, edit and remove Evidences, used inside dialogues.", EditorStyles.helpBox);


        evidenceNameContent = GUILayout.TextField(evidenceNameContent);


        if (GUILayout.RepeatButton("Add Text Evidence"))
        {
            if (evidenceNameContent != "")
            {
                AddTextEvidence();
            }
        }

        if (GUILayout.RepeatButton("Remove Text Evidence"))
        {
            if (evidenceNameContent != "")
            {
                RemoveTextEvidence();
            }
        }

        GUILayout.Label("Current Text Evidences:");

        string evidencesString = "";
        foreach (string var in LevelEditorProperties.GetTextEvidences())
        {
            evidencesString += var + "\n";
        }

        GUILayout.Label(evidencesString, EditorStyles.helpBox);
    }

    private void AddTextEvidence()
    {
        LevelEditorProperties.AddTextEvidence(evidenceNameContent);
    }

    private void RemoveTextEvidence()
    {
        LevelEditorProperties.RemoveTextEvidence(evidenceNameContent);
    }

    private void GUIObjEvidences() //uncomplete creating obj etc
    {
        GUILayout.Label("Object Evidences", EditorStyles.boldLabel);
        GUILayout.Label("Use this section to define, edit and remove Object Evidences, to be drawn thrughout the level.", EditorStyles.helpBox);

        objNameContent = GUILayout.TextField(objNameContent);

        if (GUILayout.RepeatButton("Add Object Evidence"))
        {
            if (objNameContent != "")
            {
                AddObjEvidence();
            }
        }

        if (GUILayout.RepeatButton("Remove Object Evidence"))
        {
            if (objNameContent != "")
            {
                RemoveObjEvidence();
            }
        }

        GUILayout.Label("Current Text Evidences:");

        string evidencesString = "";
        foreach (string var in LevelEditorProperties.GetObjEvidences())
        {
            evidencesString += var + "\n";
        }

        GUILayout.Label(evidencesString, EditorStyles.helpBox);
    }

    private void AddObjEvidence()
    {
        LevelEditorProperties.AddObjEvidence(objNameContent);
    }

    private void RemoveObjEvidence()
    {
        LevelEditorProperties.RemoveObjEvidence(objNameContent);
    }

    private void GUISpawnPoint()
    {
        GUILayout.Label("Spawn Point", EditorStyles.boldLabel);
        GUILayout.Label("Use this section to set the spawnpoint of the player. The Players feet will be set at the position of the given GameObject with the respective rotation.", EditorStyles.helpBox);

        spawnPointObjContent = EditorGUILayout.ObjectField(spawnPointObjContent, typeof(Transform),true) as Transform;

        if (GUILayout.RepeatButton("Set Spawn Point"))
        {
            if (spawnPointObjContent != null)
            {
                SetSpawnPoint();
            }
        }
    }

    private void SetSpawnPoint()
    {
        if (spawnPointObjContent != null)
        {
            LevelEditorProperties.SetSpawn(spawnPointObjContent);
        }
    }
}