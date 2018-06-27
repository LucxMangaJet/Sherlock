using UnityEngine;
using UnityEditor;

public class ToggleMeshGrid : EditorWindow
{
    bool toEnable = false;

    [MenuItem("Holmes/ToggleMeshGrid")]
    static void Init()
    {
        ToggleMeshGrid window = (ToggleMeshGrid)EditorWindow.GetWindow(typeof(ToggleMeshGrid));
        window.Show();
    }

    void OnGUI()
    {
        
        GUILayout.Label("Used to enable/disable all the meshGrid Scripts", EditorStyles.boldLabel);

        toEnable = GUILayout.Toggle(toEnable,"To Enable?");
       
        if(GUILayout.Button(toEnable?"Enable":"Disable")){
            EnableDisableAll();
        }
    }


    void EnableDisableAll()
    {
        Debug.Log("ToggleMeshGrid used");
        Transform[] g= GameObject.FindObjectsOfType(typeof(Transform)) as Transform[];
        Debug.Log(g.Length);

        foreach (Transform s in g)
        {
            matchGrid m = s.GetComponent<matchGrid>();
            if (m != null)
                m.enabled = toEnable;
        }
    }
}