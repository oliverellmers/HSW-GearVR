using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class AudioController : MonoBehaviour {

    public UIFader m_UIFader;
    public AudioSource m_AudioSource;

    // Use this for initialization
    void Start () {
        m_UIFader.OnFadeOutComplete += StartAudio;
    }

    private void StartAudio() {
        m_AudioSource.Play();
    }
}
