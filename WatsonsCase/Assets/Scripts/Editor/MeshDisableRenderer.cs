using UnityEngine;
using UnityEditor;

public class MeshDisableRenderer : EditorWindow
{
    bool toEnable = false;
    GameObject objToEnable;

    [MenuItem("Holmes/MeshDisableRenderer")]
    static void Init()
    {
        MeshDisableRenderer window = (MeshDisableRenderer)EditorWindow.GetWindow(typeof(MeshDisableRenderer));
        window.Show();
    }

    void OnGUI()
    {
        
        GUILayout.Label("Used to enable/disable all the Mesh Renderers", EditorStyles.boldLabel);
        objToEnable =(GameObject)EditorGUILayout.ObjectField(objToEnable, typeof(GameObject), true);

        toEnable = GUILayout.Toggle(toEnable,"To Enable?");
       
        if(GUILayout.Button(toEnable?"Enable":"Disable")){
            EnableDisableAll();
        }
    }


    void EnableDisableAll()
    {
        if (objToEnable == null)
            return;

        MeshRenderer[] renderers = objToEnable.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer r in renderers)
        {
            r.enabled = toEnable;
        }
    }
}