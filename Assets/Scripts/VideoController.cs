using UnityEngine;
using System.Collections;
using DG.Tweening;
using RenderHeads.Media.AVProVideo;

public class VideoController : MonoBehaviour {

    public MediaPlayer Player;
    public DisplayUGUI Scr1;
    // Use this for initialization
    bool isPlay;
	void Start () {

    }

    public void Play()
    {
        Player.Rewind(false);
        Player.Play();
        Scr1.DOFade(1, 1f);
        isPlay = true;
    }

    public void Pause()
    {
        Player.Control.SetVolume(0);
    }

    public void UnPause()
    {
        Player.Control.SetVolume(1);
    }

    public void Stop()
    {
        Scr1.DOFade(0, 1f).OnComplete(Player.Stop);
    }

    void Update()
    {
        if (isPlay)
        {
            if (Player.Control.IsFinished())
            {
                Scr1.DOFade(0, 1f);
                isPlay = false;
                Scr1.raycastTarget = false;
                GetComponent<VideoButton>().SendMessage("ScreenOff");
                MainMenu.GetInstance().backButton.RemoteClick();
            }
        }
    }
}
