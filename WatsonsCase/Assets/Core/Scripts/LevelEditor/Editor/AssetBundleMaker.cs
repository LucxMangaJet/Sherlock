using UnityEngine;
using UnityEditor;

public class AssetBundleMaker : EditorWindow
{

    Object scene;

    [MenuItem("LevelEditor/AssetBundleMaker")]
    static void Init()
    {
        AssetBundleMaker window = (AssetBundleMaker)EditorWindow.GetWindow(typeof(AssetBundleMaker));
        window.Show();
    }

    void OnGUI()
    {
        
        GUILayout.Label("Select Object to Create AssetBundle", EditorStyles.boldLabel);
        scene = EditorGUILayout.ObjectField(scene, typeof(Object), true);

       
        if(GUILayout.Button("Create Asset Bundles")){
            string assetPath = AssetDatabase.GetAssetPath(scene);
            AssetImporter.GetAtPath(assetPath).SetAssetBundleNameAndVariant("testBundle", "");

            BuildPipeline.BuildAssetBundles("Assets/", BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows64);
            
        }
    }

    
}