using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundHandler : MonoBehaviour {

    public enum ClipEnum
    {
          UIOpenBook,
          UICloseBook,
          UIWritingShort,
          UIWritingMid,
          UIWritingLong,
          UICrossOut,
          UIDrawing,
          UITurningPage,
          AutoPlayingMusic,
          AutoPlayingMusic2,
          DoorOpening
    }

    public enum OutputEnum
    {
        UI,
        Piano
    }

    public AudioClip[] clips_ClosingBook;
    public AudioClip[] clips_WritingShort;
    public AudioClip[] clips_WritingMid;
    public AudioClip[] clips_WritingLong;
    public AudioClip[] clips_Drawing;
    public AudioClip[] clips_CrossOut;
    public AudioClip[] clips_TurningPage;
    public AudioClip[] clips_DoorOpening;
    public AudioClip clips_AutoPlayingMusic, clips_AutoPlayingMusic2;
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
            case ClipEnum.UIWritingShort:
                a = clips_WritingShort[Random.Range(0, clips_WritingShort.Length)];
                break;
            case ClipEnum.UIWritingMid:
                a = clips_WritingMid[Random.Range(0, clips_WritingMid.Length)];
                break;
            case ClipEnum.UIWritingLong:
                a = clips_WritingLong[Random.Range(0, clips_WritingLong.Length)];
                break;
            case ClipEnum.AutoPlayingMusic:
                a = clips_AutoPlayingMusic;
                break;
            case ClipEnum.AutoPlayingMusic2:
                a = clips_AutoPlayingMusic2;
                break;
            case ClipEnum.DoorOpening:
                a = clips_DoorOpening[Random.Range(0, clips_DoorOpening.Length)];
                break;
            default:
                break;
        }

        return a;
    }
}
