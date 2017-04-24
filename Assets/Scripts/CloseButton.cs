using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CloseButton : MonoBehaviour
{
    Gallery m_gallery;
    CanvasGroup photoCanvas;
    CanvasGroup videoCanvas;
    public VideoController controller;
    public bool isVideo;

    public void ButtonClicked()
    {
        photoCanvas.DOFade(0, 1);
        photoCanvas.blocksRaycasts = false;
        photoCanvas.interactable = false;
        m_gallery.isOpen = false;
        Sfx.GalleryClick();
        Anim();
        MainMenu.GetInstance().Gallery_Canvas.blocksRaycasts = true;
        m_gallery.GoToZero();
        foreach (RectTransform c in m_gallery.Container.GetComponentInChildren<RectTransform>())
        {
            Destroy(c.gameObject);
        }
    }

    public void ButtonVideoClicked()
    {
        controller.Stop();
        videoCanvas.DOFade(0, 1);
        videoCanvas.interactable = false;
        videoCanvas.blocksRaycasts = false;
        Sfx.GalleryClick();
        Anim();
        MainMenu.GetInstance().GetComponent<AudioSource>().mute = false;
   //     MainMenu.GetInstance().backButton.RemoteClick(); //для 25ти летия видео
    }

    void Anim()
    {
        transform.DOKill();
        transform.DOScale(0.9f, 0.2f);
        transform.DOScale(1f, 0.2f).SetDelay(0.2f);
    }


    private void Awake()
    {
        if (isVideo)
        {
            videoCanvas = GetComponentInParent<CanvasGroup>();
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { ButtonVideoClicked(); });
        }
        else
        {
            photoCanvas = GetComponentInParent<CanvasGroup>();
            m_gallery = GetComponentInParent<Gallery>();
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { ButtonClicked(); });
        }
    }
}
