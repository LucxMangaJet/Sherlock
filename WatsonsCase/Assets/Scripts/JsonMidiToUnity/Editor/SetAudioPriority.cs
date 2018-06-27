using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace JsonMIDItoUnity
{
    public class SetAudioPriority: EditorWindow
    {

        GameObject keyboardParent;

        [MenuItem("JsonMIDI/SetAudioPriority")]
        static void Init()
        {
            SetAudioPriority window = (SetAudioPriority)EditorWindow.GetWindow(typeof(SetAudioPriority));
            window.Show();
        }

        void OnGUI()
        {

            GUILayout.Label("Used to set the prority on the Audio sources from low to high", EditorStyles.boldLabel);
            keyboardParent = (GameObject)EditorGUILayout.ObjectField(keyboardParent, typeof(GameObject), true);


            if (GUILayout.Button("Apply"))
            {
                Apply();
            }
        }


        void Apply()
        {
            if (keyboardParent == null)
                return;

            Transform parentTransform = keyboardParent.transform;


            for (int i = 0; i < parentTransform.childCount; i++)
            {
                Transform g = parentTransform.GetChild(i);

                g.GetComponent<AudioSource>().priority = 200 - i;

            }


        }
    }
}