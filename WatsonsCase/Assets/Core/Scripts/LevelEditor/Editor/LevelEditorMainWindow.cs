using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class LevelEditorMainWindow : EditorWindow {

    string newCharacterName = "";
    int deleteCharacterIndex = 0;

    [MenuItem("LevelEditor/Main Window")]
    static void Init()
    {

        LevelEditorMainWindow window = (LevelEditorMainWindow)EditorWindow.GetWindow(typeof(LevelEditorMainWindow));
        window.Show();
    }

    void OnGUI()
    {
        LevelEditorProperties.Setup();

        if (GUILayout.Button("Load Level Editor"))
        {
            LevelEditorController.LoadLevelEditor();
        }

        if(GUILayout.Button("New Level"))
        {
            LevelEditorController.NewCustomLevel();
        }

        GUILayout.BeginHorizontal();
        newCharacterName = EditorGUILayout.TextField("Name: ", newCharacterName);
        if (GUILayout.Button("Add new Character"))
        {
            LevelEditorProperties.AddCharacter(newCharacterName);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        deleteCharacterIndex = EditorGUILayout.Popup(deleteCharacterIndex, LevelEditorProperties.GetCharacters().ToArray());

        if (GUILayout.Button("Delete Character"))
        {
            LevelEditorProperties.RemoveCharacter(deleteCharacterIndex);
        }
        GUILayout.EndHorizontal();



        if (GUILayout.Button("Export Level"))
        {
            LevelEditorController.ExportLevel();
        }

    }
}
