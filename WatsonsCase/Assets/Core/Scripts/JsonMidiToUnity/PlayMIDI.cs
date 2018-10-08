using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JsonMIDItoUnity
{
    public class PlayMIDI : MonoBehaviour
    {

        [SerializeField] TextAsset jsonFile;
        [SerializeField] MIDI midi;
        [SerializeField] GameObject keyboard;
        KeyboardKey[] keys;
        [SerializeField] float timeSinceStart = 0;
        const int keyboardKeyStartKey =21;
        public bool shouldPlay = false;
        void Start()
        {
            JsonUtility.FromJsonOverwrite(jsonFile.text, midi);

            Debug.Log("MIDItoUnity: Loaded "+ midi.tracks.Length +" Tracks");
        }

        public void Play()
        {

            keys = keyboard.GetComponentsInChildren<KeyboardKey>();
            Debug.Log("MIDItoUnity: Playing song with " + keys.Length+" keys.");
            StartCoroutine(CountTime());
            shouldPlay = true;

            foreach (MIDITrack track in midi.tracks)
            {
                foreach (MIDINote note in track.notes)
                {
                    float extraTime = midi.startTime + track.startTime;
                    // Debug.Log("Buffering:" + note.midi);
                    StartCoroutine(PlayNote(note,extraTime));
                }
            }
        }

        IEnumerator CountTime()
        {
            timeSinceStart = 0;
            while (true)
            {
                yield return null;
                timeSinceStart += Time.deltaTime;
            }
                
        }

        IEnumerator PlayNote(MIDINote m,float extraTime)
        {
            while (timeSinceStart < m.time + extraTime)
                yield return null;
            int indx = m.midi - keyboardKeyStartKey;
            if (indx >= 0 && indx <= keys.Length && shouldPlay)
              StartCoroutine(keys[indx].Play(m.duration,m.velocity));
        }
    }
    

    [Serializable]
    public class MIDI
    {
        public MIDIHeader header;

        public float startTime;
        public float duration;

        public MIDITrack[] tracks;


    }

    [Serializable]
    public class MIDIHeader
    {
        public string name;
        public float bpm;
        public float[] timeSignature;
        public float PPQ;
    }

    [Serializable]
    public class MIDITrack
    {
        public float id;
        public string name;
        public MIDINote[] notes;

        public float startTime;
        public float duration;

        //MIDIControlChanges controlChanges;

        public bool isPercussion;
        public float channelNumber;
        public float instrumentNumber;
        public string instrumentFamily;
        public string instrument;

    }

    [Serializable]
    public class MIDINote
    {
        public int midi;
        public float time;
        public string note;
        public float velocity;
        public float duration;
    }

   
}