using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace JsonMIDItoUnity
{
    public class SetAudioFilesFromHierarchy : EditorWindow
    {

        GameObject keyboardParent;

        [MenuItem("JsonMIDI/SetAudioFilesFromHierarchy")]
        static void Init()
        {
            SetAudioFilesFromHierarchy window = (SetAudioFilesFromHierarchy)EditorWindow.GetWindow(typeof(SetAudioFilesFromHierarchy));
            window.Show();
        }

        void OnGUI()
        {

            GUILayout.Label("Used to add the sound files to the Keys", EditorStyles.boldLabel);
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
            Object[] load = Resources.LoadAll("PianoNotes", typeof(AudioClip));
            AudioClip[] clips = new AudioClip[load.Length];
            for (int i = 0; i < load.Length; i++)
            {
                clips[i] = (AudioClip)load[i];
            }

            for (int i = 0; i < clips.Length; i++)
            {
                Transform g = parentTransform.GetChild(i);
                g.GetComponent<AudioSource>().clip = clips[i];

            }

            for (int i = 0; i < parentTransform.childCount; i++)
            {
                GameObject g = parentTransform.GetChild(i).gameObject;

                g.GetComponent<AudioSource>().priority = 200 - i;

            }


        }
    }
}