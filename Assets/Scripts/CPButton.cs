using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CPButton : MonoBehaviour, IPointerClickHandler
{
    public bool isBack;
    int currentPict;
    Gallery m_gallery;

    public void OnPointerClick(PointerEventData data)
    {
        m_gallery.SendMessage("ResetTimer");
        if (isBack)
        {
          
            currentPict -= 1;
            if (currentPict < 0) currentPict = m_gallery.elements-1;
           
        }
        else {
            currentPict += 1;
            if (currentPict > m_gallery.elements - 1) currentPict = 0;
        }
        m_gallery.CurrentTarget = currentPict;
        m_gallery.ChangePict(currentPict);
        Sfx.GalleryClick();
        Anim();
    }

    void UpdateCurrent()
    {
        currentPict = (int)m_gallery.CurrentTarget;
    }

    void Anim()
    {
        transform.DOKill();
        transform.DOScale(0.9f,0.2f);
        transform.DOScale(1f, 0.2f).SetDelay(0.2f);
    }

    private void Awake()
    {
        m_gallery = GetComponentInParent<Gallery>();
    }

    // Use this for initialization
    void Start()
    {
       
        currentPict = (int)m_gallery.CurrentTarget;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
