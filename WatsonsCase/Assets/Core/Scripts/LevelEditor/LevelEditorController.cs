using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Linq;

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
        SceneManager.LoadScene("CustomLevel");
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

        string[] filePaths = Directory.GetFiles("Assets\\CustomLevel\\","*",SearchOption.AllDirectories);

        string[] validPaths = (from a in filePaths where !a.EndsWith(".meta") select a).ToArray();

        foreach (var path in validPaths)
        {
            AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant("CustomLevel", "");
        }

        string savePath = EditorUtility.OpenFolderPanel("Export Custom Level", "", "");

        BuildPipeline.BuildAssetBundles(savePath, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);

        foreach (var path in validPaths)
        {
            AssetImporter.GetAtPath(path).SetAssetBundleNameAndVariant("None","");
        }


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
