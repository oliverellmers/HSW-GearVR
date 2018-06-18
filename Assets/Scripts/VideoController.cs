using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using VRStandardAssets.Utils;
using DG.Tweening;

public class VideoController : MonoBehaviour {


    public VideoPlayer videoPlayer;
    public Material material;
    public Texture TextureWhileLoading;
    public RenderTexture VideoRenderTexture;
    public UIFader m_UIFader;

    public MeshRenderer previous;
    public MeshRenderer next;

    public Material videoSkybox;
    
    [SerializeField]
    private Texture[] videoTextures;

    [SerializeField]
    private VideoClip[] videoClips;
    
    
    private int videoIndex = 0;
    private Coroutine PlayVideoCoroutine;

    //private bool isInit = false;

    static string TAG = "VideoController";

    void Start () {
        Application.runInBackground = true;
        //StartVideo();

        Texture2D blackTexture = new Texture2D(1, 1);
        blackTexture.SetPixel(0, 0, Color.black);
        blackTexture.Apply();
        TextureWhileLoading = blackTexture;
        material.SetTexture("_MainTex", TextureWhileLoading);

        m_UIFader.OnFadeOutComplete += StartVideo;
        m_UIFader.OnFadeOutComplete += FadeInControls;
        m_UIFader.OnFadeOutComplete += SwitchSkyBox;

        previous.material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 0.0f));
        next.material.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, 0.0f));
        videoSkybox.SetColor("_Tint", new Color(0.0f, 0.0f, 0.0f, 1.0f));
    }

    private void FadeInControls() {
        StartCoroutine(DoFadeControls());
    }

    IEnumerator DoFadeControls() {

        previous.material.DOColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.25f);
        next.material.DOColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), 0.25f);

        yield return new WaitForSeconds(0.25f);

    }

    private void SwitchSkyBox() {
        RenderSettings.skybox = videoSkybox;
        StartCoroutine(DoFadeSkybox());
    }

    IEnumerator DoFadeSkybox() {
        //videoSkybox.DOColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), 2.25f);
        videoSkybox.DOVector(new Vector4(0.5f, 0.5f, 0.5f, 1.0f), "_Tint", 2.0f);
        yield return new WaitForSeconds(0.25f);
    }

    public void StartVideo() {
        /*
        if (!isInit)
        {
            //TextureWhileLoading = videoTextures[videoIndex];
            TextureWhileLoading = videoTextures[0];
        }
        else
        {
            Texture2D blackTexture = new Texture2D(1, 1);
            blackTexture.SetPixel(0, 0, Color.black);
            blackTexture.Apply();
            material.SetTexture("_MainTex", blackTexture);
        }
        */
        Texture2D blackTexture = new Texture2D(1, 1);
        blackTexture.SetPixel(0, 0, Color.black);
        blackTexture.Apply();
        TextureWhileLoading = blackTexture;
        //material.SetTexture("_MainTex", blackTexture);



        VideoClip clip = videoClips[videoIndex];
        
        videoPlayer.clip = clip;

        PlayVideoCoroutine = StartCoroutine(Play360Video());
    }

    IEnumerator Play360Video()
    {
        /*
        if (TextureWhileLoading != null)
            material.SetTexture("_MainTex", TextureWhileLoading);
        else
        {
            Texture2D blackTexture = new Texture2D(1, 1);
            blackTexture.SetPixel(0, 0, Color.black);
            blackTexture.Apply();
            material.SetTexture("_MainTex", blackTexture);
        }
        */

        /*
        if (VRSceneConfig.vrEnabled)
        {
            Debug.Log("Waiting for VR before we prepare video");
            yield return new WaitForSeconds(1.0f);
        }
        */


        videoPlayer.Prepare();
        Debug.Log("Preparing Video");

        while (!videoPlayer.isPrepared)
        {
            //Debug.Log("Preparing Video");
            yield return null;
        }

        Debug.Log("Done Preparing Video");

        // Play the video
        videoPlayer.Play();
        /*
        if (videoPlayer.playOnAwake)
            videoPlayer.Play();
            */

        // Assign the render texture to the skybox material
        material.SetTexture("_MainTex", VideoRenderTexture);

        
    }

    private void OnDestroy()
    {
        StopVideoSafely();
    }

    private void StopVideoSafely() {
        videoPlayer.Stop();
        if (PlayVideoCoroutine!= null) {
            StopCoroutine(PlayVideoCoroutine);
        }
        
        PlayVideoCoroutine = null;
    }



    public void IncrementVideo(bool next) {
        StartCoroutine(IncrementVideoRoutine(next));
    }

    private IEnumerator IncrementVideoRoutine(bool n) {

        if (videoPlayer.isPlaying)
        {

            StopVideoSafely();
        }

        if (n)
        {
            videoIndex++;

            if (videoIndex == videoClips.Length)
            {
                videoIndex = 0;
            }
        }
        else
        {

            videoIndex--;
            if (videoIndex == -1)
            {
                videoIndex = videoClips.Length - 1;
            }
        }

        

        yield return new WaitForEndOfFrame();

        StartVideo();
    }
}
