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

    [MenuItem("LevelEditor/GameLogicWindow")]
    static void Init()
    {
        LevelEditorProperties.Setup(); //remove me later

        GameLogicWindow window = (GameLogicWindow)EditorWindow.GetWindow(typeof(GameLogicWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUIVariables();
        GUILayout.Space(10);
        GUIEvidences();
        GUILayout.Space(10);
        GUIObjEvidences();
        GUILayout.Space(10);
        GUISpawnPoint();
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

        foreach (KeyValuePair<string, bool> var in LevelEditorProperties.GetVariables())
        {
            GUILayout.Label(var.Key + "  " + (var.Value ? "✔" : "X"));
        }
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
            if (varNameContent != "")
            {
                AddTextEvidence();
            }
        }

        if (GUILayout.RepeatButton("Remove Text Evidence"))
        {
            if (varNameContent != "")
            {
                RemoveTextEvidence();
            }
        }

        GUILayout.Label("Current Text Evidences:");

        foreach (string var in LevelEditorProperties.GetTextEvidences())
        {
            GUILayout.Label(var);
        }
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
            if (varNameContent != "")
            {
                AddObjEvidence();
            }
        }

        if (GUILayout.RepeatButton("Remove Object Evidence"))
        {
            if (varNameContent != "")
            {
                RemoveObjEvidence();
            }
        }

        GUILayout.Label("Current Text Evidences:");

        foreach (string var in LevelEditorProperties.GetObjEvidences())
        {
            GUILayout.Label(var);
        }
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

    }


    
}