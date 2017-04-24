using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RenderHeads.Media.AVProVideo;
using UnityEngine.UI;
using DG.Tweening;

public class VideoButton : MonoBehaviour {

    public DisplayUGUI display;
    VideoController controller;
    public Text videoDescr;
    CanvasGroup blackscreen;

    void Awake()
    {
        foreach (CanvasGroup disp in FindObjectsOfType(typeof(CanvasGroup)))
        {
            if (disp.tag == "VideoScreen")
            {
                display = disp.GetComponentInChildren<DisplayUGUI>();
                blackscreen = disp;
            }
        }
        controller = GetComponent<VideoController>();
        controller.Player = GetComponent<MediaPlayer>();
        controller.Scr1 = display;
        videoDescr = GetComponentInChildren<Text>();
      
    }

    private void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { ButtonClicked(); });
    }

    void ScreenOff()
    {
        blackscreen.DOFade(0, 1);
    }

    public void RemoteClick()
    {
        display._mediaPlayer = GetComponent<MediaPlayer>();
        controller.Play();
        display.isPlayed = true;
        display.raycastTarget = true;
        blackscreen.DOFade(1, 1);
        blackscreen.GetComponentInChildren<CloseButton>().controller = controller;
        blackscreen.interactable = true;
        blackscreen.blocksRaycasts = true;
        MainMenu.GetInstance().GetComponent<AudioSource>().mute = true;
    }

    public void ButtonClicked()
    {
        display._mediaPlayer = GetComponent<MediaPlayer>();
        controller.Play();
        display.isPlayed = true;
        display.raycastTarget = true;
        blackscreen.DOFade(1, 1);
        blackscreen.GetComponentInChildren<CloseButton>().controller = controller;
        blackscreen.interactable = true;
        blackscreen.blocksRaycasts = true;
        MainMenu.GetInstance().GetComponent<AudioSource>().mute = true;
        Sfx.GalleryClick();
        Anim();
    }

    void Anim()
    {
        transform.DOKill();
        transform.DOScale(0.9f, 0.2f);
        transform.DOScale(1f, 0.2f).SetDelay(0.2f);
    }
}
