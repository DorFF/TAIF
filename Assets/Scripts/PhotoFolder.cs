using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class PhotoFolder : MonoBehaviour {

    string folderpath;
    public string[] FullFileList;
    public List<Sprite> m_sprites;
    GameObject VideoButton;
    public RectTransform VideoGallery;
    public Gallery m_gallery;
    public CanvasGroup m_photoCanvas;
    public string Header;
    GameObject m_Slide;

    public void Awake()
    {
        foreach (CanvasGroup disp in FindObjectsOfType(typeof(CanvasGroup)))
        {
            if (disp.tag == "PhotoScreen")
            {
                m_photoCanvas = disp; 
                m_gallery = disp.GetComponentInChildren<Gallery>();
            }
        }
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { ButtonClicked(); });
        m_Slide = Resources.Load("Slide_Container") as GameObject;
    }

    public void ButtonClicked()
    {
       
        StartCoroutine(LoadForSlides());
        //       m_gallery.isOpen = true;
        Sfx.GalleryClick();
        Anim();
        MainMenu.GetInstance().Gallery_Canvas.blocksRaycasts = false;
    }

    public IEnumerator LoadForSlides()
    {
        MainMenu.GetInstance().Gallery_Header.text = Header.ToUpper();
        m_gallery.GetComponent<Gallery>().SetElements(m_sprites.Count);
        float y = m_gallery.Container.GetComponent<RectTransform>().sizeDelta.y;
        m_gallery.Container.GetComponent<RectTransform>().sizeDelta = new Vector2(2300 * m_sprites.Count, y);
        yield return StartCoroutine(LoadAllSlides());
        m_photoCanvas.DOFade(1, 0.5f);
        m_photoCanvas.blocksRaycasts = true;
        m_photoCanvas.interactable = true;
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator LoadAllSlides()
    {
        float resize = 0;
        foreach (Sprite c in m_sprites.ToArray())
        {
            if (c.rect.width / c.rect.height > 1) resize = 2300 / c.rect.width;
            if (c.rect.width / c.rect.height < 1) resize = 1533 / c.rect.height;
            GameObject newSlide = Instantiate(m_Slide) as GameObject;
            newSlide.transform.SetParent(m_gallery.Container.transform, false);
            newSlide.transform.FindChild("Slide").GetComponent<Image>().sprite = c;
            newSlide.transform.FindChild("Slide").GetComponent<RectTransform>().sizeDelta = new Vector2(c.rect.width * resize, c.rect.height * resize);
        }
        yield return null;
    }

    void Anim()
    {
        transform.DOKill();
        transform.DOScale(0.9f, 0.2f);
        transform.DOScale(1f, 0.2f).SetDelay(0.2f);
    }


}
