using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JsonMIDItoUnity
{
    /// <summary>
    /// 
    /// </summary>
    //[RequireComponent(typeof(AudioSource))]
    public class KeyboardKey : MonoBehaviour
    {

        // Imporovements: Add ienumerator animation for pressing;

        public int midiKey;
        public string note;
        public bool playSound;

        AudioSource source;
        private void Start()
        {
            source = GetComponent<AudioSource>();
        }

        public IEnumerator Play(float duration, float strength)
        {
            if (playSound)
                source.volume = strength;

            Press();

            float t = 0;
            while (t < duration)
            {
                if (playSound&& t/duration > 0.7)
                {
                    source.volume = strength * (1-t/duration) * 3.333f;
                }
                yield return null;
                t += Time.deltaTime;
            }
      
            Relese();

        }

        void Press()
        {
            // Debug.Log("Pressing Note:" + note);
            transform.localEulerAngles = new Vector3(3, 0, 0);
            if (playSound)
                source.Play();
        }

        void Relese()
        {
            // Debug.Log("Relesing Note:" + note);
            transform.localEulerAngles = new Vector3(0, 0, 0);
            if (playSound)
            {
                source.Stop();
                source.volume = 1;
            }
              
        }


    }
}