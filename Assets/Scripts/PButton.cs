using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class PButton : MonoBehaviour, IPointerClickHandler
{
    Gallery m_gallery;

    public void OnPointerClick(PointerEventData data)
    {
        for (int i = 0; i < m_gallery.m_PButtons.Length; i++)
        {
            if (m_gallery.m_PButtons[i] == this)
            {
                m_gallery.ChangePict(m_gallery.m_PButtons.Length - i - 1);
                m_gallery.CurrentTarget = m_gallery.m_PButtons.Length - i - 1;
            }
        }
        m_gallery.SendMessage("ResetTimer");
        Sfx.GalleryClick();
    }


    // Use this for initialization
    void Start () {
        m_gallery = GetComponentInParent<Gallery>();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
