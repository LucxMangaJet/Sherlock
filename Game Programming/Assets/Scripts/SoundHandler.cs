using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundHandler : MonoBehaviour {

    public enum ClipEnum
    {
          UIOpenBook,
          UICloseBook,
          UIWriting,
          UICrossOut,
          UIDrawing,
          UITurningPage
    }

    public enum OutputEnum
    {
        UI
    }

    public AudioClip[] clips_ClosingBook;
    public AudioClip[] clips_Writing;
    public AudioClip[] clips_Drawing;
    public AudioClip[] clips_CrossOut;
    public AudioClip[] clips_TurningPage;
    public AudioSource[] outputs;

	public void PlayClip(ClipEnum c, OutputEnum o )
    {
        AudioClip a = GetClip(c);
        if (a == null)
            return;
        outputs[(int)o].clip = a;
        outputs[(int)o].Play();

    }

    public void Stop(OutputEnum o)
    {
        outputs[(int)o].Stop();
    }


    AudioClip GetClip(ClipEnum c)
    {
        AudioClip a = null;
        switch (c)
        {
            case ClipEnum.UICloseBook:
                a = clips_ClosingBook[Random.Range(0, clips_ClosingBook.Length)];
                break;
            case ClipEnum.UICrossOut:
                a = clips_CrossOut[Random.Range(0, clips_CrossOut.Length)];
                break;
            case ClipEnum.UIDrawing:
                a = clips_Drawing[Random.Range(0, clips_Drawing.Length)];
                break;
            case ClipEnum.UITurningPage:
                a = clips_TurningPage[Random.Range(0, clips_TurningPage.Length)];
                break;
            case ClipEnum.UIWriting:
                a = clips_Writing[Random.Range(0, clips_Writing.Length)];
                break;
            default:
                break;
        }

        return a;
    }
}
