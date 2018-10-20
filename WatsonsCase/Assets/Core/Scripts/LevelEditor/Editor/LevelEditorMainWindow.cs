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

        GUILayout.Label("WARNING: this will delete your previous Custom Level!", EditorStyles.helpBox);
        if (GUILayout.Button("New Level"))
        {
            LevelEditorController.NewCustomLevel();
        }
        GUILayout.Label("Use this to add characters to the game logic that you have already imported into the scene. Make sure to give both the object and the myName field the same name as the one given here.", EditorStyles.helpBox);
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


        GUILayout.Label("Don't forget to Save changes to your Scene before exporting!", EditorStyles.helpBox);
        
        if (GUILayout.Button("Export Level"))
        {
            LevelEditorController.ExportLevel();
        }
        GUILayout.Label("Exporting may take some time.", EditorStyles.helpBox);

    }
}
