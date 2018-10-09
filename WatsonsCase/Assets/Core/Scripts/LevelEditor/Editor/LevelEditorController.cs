﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;

public static class LevelEditorController {

    public static void LoadLevelEditor()
    {
        try
        {
            LevelEditorProperties.Setup();
            LoadCustomLevel();
        }
        catch
        {
            NewCustomLevel();
        }
        

        //force leveleditor layout ?
    }

    private static void LoadCustomLevel()
    {
        EditorSceneManager.OpenScene("Assets/CustomLevel/CustomLevel.unity");
    }

    public static void NewCustomLevel()
    {
        string defaultPath = "Assets/Core/Scenes/LevelEditorDefault.unity";
        string destinationPath = "Assets/CustomLevel/CustomLevel.unity";
        File.Copy(defaultPath, destinationPath, true);

        if (!CustomSceneIsInBuildSettings())
        {
            AddCustomLevelToBuildIndex();
        }

        LoadCustomLevel();

        var objs = GameObject.FindGameObjectsWithTag("ToHideInEditor");
        foreach (var obj in objs)
        {
            obj.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy | HideFlags.NotEditable;
        }
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Loaded New Custom Scene");
    }

    private static bool CustomSceneIsInBuildSettings()
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if(scene.path == "Assets/CustomLevel/CustomLevel.unity")
            {
                return true;
            }
        }

        return false;
    }

    private static void AddCustomLevelToBuildIndex()
    {
        var original = EditorBuildSettings.scenes;
        var newSettings = new EditorBuildSettingsScene[original.Length + 1];
        System.Array.Copy(original, newSettings, original.Length);
        var sceneToAdd = new EditorBuildSettingsScene("Assets/CustomLevel/CustomLevel.unity", true);
        newSettings[newSettings.Length - 1] = sceneToAdd;
        EditorBuildSettings.scenes = newSettings;
    }

    public static void ExportLevel()
    {
        CreateVariablesAndEvidencesIntoTextfile();

        string[] filePaths = Directory.GetFiles("Assets\\CustomLevel\\","*",SearchOption.AllDirectories);

        List<string> validPaths = (from a in filePaths where !a.EndsWith(".meta") select a).ToList();

        string[] scenesPaths = (from a in validPaths where a.EndsWith(".unity") select a).ToArray();

        List<string> otherPaths = new List<string>(validPaths);

        foreach (var path in scenesPaths)
        {
            otherPaths.Remove(path);
        }


        foreach (var path in scenesPaths)
        {
            AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant("CustomLevelScenes", "");
        }

        foreach (var path in otherPaths)
        {
            AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant("CustomLevelAssets", "");
        }

        string savePath = Application.persistentDataPath + "/";

        BuildPipeline.BuildAssetBundles(savePath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        foreach (var path in validPaths)
        {
            AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant("","");
        }

        Debug.Log("Bundle Exported to: " + savePath);

    }

    private static void CreateVariablesAndEvidencesIntoTextfile()
    {
        string path = "Assets/CustomLevel/Logic/VarsAndEvidences.txt";

        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(":Variables:");
        foreach (var v in LevelEditorProperties.GetVariables())
        {
            writer.WriteLine((v.Value ? "" : "!") + v.Key + ";");
        }
        writer.WriteLine(":Text Evidences:");

        foreach (var e in LevelEditorProperties.GetTextEvidences())
        {
            writer.WriteLine(e + ";");
        }

        writer.WriteLine("Object Evidences:");

        foreach (var o in LevelEditorProperties.GetObjEvidences())
        {
            writer.WriteLine(o + ";");
        }

        writer.Close();

    }

}
