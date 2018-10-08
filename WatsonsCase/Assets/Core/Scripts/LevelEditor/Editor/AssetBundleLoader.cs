using UnityEngine;
using UnityEditor;

public class AssetBundleLoader : EditorWindow
{


    //[MenuItem("LevelEditor/AssetBundleLoader")]
    static void Init()
    {
        AssetBundleLoader window = (AssetBundleLoader)EditorWindow.GetWindow(typeof(AssetBundleLoader));
        window.Show();
    }

    void OnGUI()
    {
        
        GUILayout.Label("Select Path to load AssetBundle", EditorStyles.boldLabel);

        if(GUILayout.Button("Load Asset Bundle"))
        {
            string path = EditorUtility.OpenFilePanel("Please Select the Asset Bundle", "", "");
            Debug.Log(path);
            AssetBundle ab =  AssetBundle.LoadFromFile(path);
            foreach (var scene in ab.GetAllScenePaths())
            {
                Debug.Log(scene);
            }
            
        }
    }

    
}