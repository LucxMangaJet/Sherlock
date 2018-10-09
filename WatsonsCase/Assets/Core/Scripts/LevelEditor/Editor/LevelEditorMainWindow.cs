using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelEditorMainWindow : EditorWindow {

    [MenuItem("LevelEditor/Main Window")]
    static void Init()
    {
        LevelEditorMainWindow window = (LevelEditorMainWindow)EditorWindow.GetWindow(typeof(LevelEditorMainWindow));
        window.Show();
    }

    void OnGUI()
    {
        if(GUILayout.Button("Load Level Editor"))
        {
            LevelEditorController.LoadLevelEditor();
        }

        if(GUILayout.Button("New Level"))
        {
            LevelEditorController.NewCustomLevel();
        }

        if (GUILayout.Button("Export Bundle"))
        {
            LevelEditorController.ExportLevel();
        }

    }
}
