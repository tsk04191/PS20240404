using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum ButtonClick
    {
        On,
        Off,
        Fail
    }


    [Header("Speaker")]
    public AudioSource adoBGM;
    public AudioSource adoUI;

    [Header("Clips")]
    [SerializeField] private AudioClip clpBGMStart;

    [Space(10.0f)]
    [SerializeField] private AudioClip clpClickOn;
    [SerializeField] private AudioClip clpClickOff;
    [SerializeField] private AudioClip clpClickFail;

    public void PlayClick(ButtonClick c)
    {
        switch (c)
        {
            case ButtonClick.On:
                adoUI.clip = clpClickOn;
                break;
            case ButtonClick.Off:
                adoUI.clip = clpClickOff;
                break;
            case ButtonClick.Fail:
                adoUI.clip = clpClickFail;
                break;
        }

        adoUI.Play();
    }

    public void PlayBGM()
    {
        adoBGM.clip = clpBGMStart;

        adoBGM.Play();
    }
}
