using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;

public static class LevelEditorController {

    public static string PATH_CHARACTERS = "Assets/CustomLevel/Characters/";
    public static string PATH_VARS = "Assets/CustomLevel/Logic/VarsAndEvidences.txt";
    public static string PATH_SCENE = "Assets/CustomLevel/CustomLevel.unity";

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
        EditorSceneManager.OpenScene(PATH_SCENE);
    }

    public static void NewCustomLevel()
    {
        string defaultPath = "Assets/Core/Scenes/LevelEditorDefault.unity";
        File.Copy(defaultPath, PATH_SCENE, true);

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

        foreach (var item in validPaths)
        {
            Debug.Log(item);
        }

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

    public static void ReadVariablesAndEvidencesFromFile()
    {
        Dictionary<string, bool> variables= new Dictionary<string, bool>();
        List<string> textEvidences = new List<string>();
        List<string> objEvidences = new List<string>();

        string s = File.ReadAllText(PATH_VARS);
        string g = "";
        for (int i = 0; i < s.Length; i++)
        {
            char j = s[i];
            if (j.GetHashCode() > 32)
            {
                g += j;
            }
        }

        string[] o = new string[3];
        int indx = -1;
        bool inName = false;
        o[0] = ""; o[1] = ""; o[2] = "";

        for (int i = 0; i < g.Length; i++)
        {
            if (indx > 2)
                break;

            char j = g[i];
            if (j == ':')
            {
                if (inName)
                {
                    indx++;
                    inName = false;
                }
                else
                {
                    inName = true;
                }
            }
            else
            {
                if (!inName)
                    o[indx] += j;
            }
        }

        string[] k = o[0].Split(';');
        //vars
        foreach (string l in k)
        {
            if (l.Length > 2)
            {

                if (l.Contains("!"))
                {
                    variables.Add(l.Replace("!", ""), false);
                }
                else
                {
                    variables.Add(l, true);
                }

            }
        }

        k = o[1].Split(';');
        //textEvidence
        foreach (string u in k)
        {
            if (u.Length > 2)
            {
                textEvidences.Add(u);
                //	Debug.Log (u);
            }
        }

        k = o[2].Split(';');
        //textEvidence
        foreach (string w in k)
        {
            if (w.Length > 2)
            {
                objEvidences.Add(w);
                //	Debug.Log (w);
            }
        }

        LevelEditorProperties.instance.objEvidences = objEvidences;
        LevelEditorProperties.instance.textEvidences = textEvidences;
        LevelEditorProperties.instance.variables = variables;
    }

    

    public static void CreateVariablesAndEvidencesIntoTextfile()
    {

        StreamWriter writer = new StreamWriter(PATH_VARS, false);
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

        writer.WriteLine(":Object Evidences:");

        foreach (var o in LevelEditorProperties.GetObjEvidences())
        {
            writer.WriteLine(o + ";");
        }

        writer.Close();

    }

    

    public static void DeleteCharacterData(string name)
    {
        File.Delete(PATH_CHARACTERS + name + ".json");
    }

    public static List<string> LoadCharacterNames()
    {
        string[] filePaths = Directory.GetFiles("Assets\\CustomLevel\\Characters\\", "*", SearchOption.TopDirectoryOnly);

        List<string> validPaths = (from a in filePaths where !a.EndsWith(".meta") select a).ToList();

        List<string> names = new List<string>() ;

        foreach (var path in validPaths)
        {
            string[] temp = path.Split('\\');
            string temp2 = temp[temp.Length - 1];
            string name = temp2.Remove(temp2.Length - 5);
            names.Add(name);
        }
           
        return names;
    }
}
