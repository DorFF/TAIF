using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainButton : Button {
    public Color standartColor = new Color();
    public int animationType = 1;
    [Header("Position")]
    public Vector3 standartPosion;
    public Vector3 openPosition;
    public Vector3 fixPos;
    public float openScale = 0.5f;
    [Header("Speed")]
    public float speedRotation;
    public bool rightDirection;
    GameObject Gear;
    [Header("States")]
    public Texture[] Images;
    public string State2Func;
    public int buttonNumber;
    public bool select;
    Sequence spring;
    Tween m_light;
    Sequence openState;
    float direct;
    // Use this for initialization
    void Awake () {
        standartColor = GetComponent<Renderer>().materials[1].color;
        foreach (Transform gear in transform.GetComponentsInChildren<Transform>())
        {
            if (gear.tag == "Gear") Gear = gear.gameObject;
        }
        if (rightDirection) direct = 1; else direct = -1;
    }

    void Rotate()
    {
        Gear.transform.Rotate(0, 0, direct * speedRotation);
    }

    private void Update()
    {
        // if (!isOpen) 
        Rotate();
    }

    public override void Click()
    {
        switch (MainMenu.state)
        {
            case 0: { State1Open(); break; }
            case 1: { State2Open(); break; }
        }
    }

  

    #region 1stState

    IEnumerator Open1stState()
    {
        spring = DOTween.Sequence();
        Sequence change = DOTween.Sequence();
        spring.Append(transform.DOScale(1.1f, 0.2f));
        spring.Append(transform.DOScale(1f, 0.2f));
        MainMenu.GetInstance().background.DOFade(1, 0.3f);
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < MainMenu.GetInstance().m_mainButtons.Length; i++)
        {
       //     MainMenu.GetInstance().m_mainButtons[i].transform.DOShakePosition(1, 1, 10, 0.5f);
            MainMenu.GetInstance().m_mainButtons[i].GetComponent<Renderer>().materials[1].SetTexture("_MainTex", Images[i]);
        }
        yield return new WaitForSeconds(0.2f);
        MainMenu.GetInstance().background.DOFade(0, 0.3f);
        yield return new WaitForSeconds(0.1f);
        change = DOTween.Sequence();
        change.Append(MainMenu.GetInstance().backButton.transform.DOScale( 0.55f, 0.2f));
        change.Append(MainMenu.GetInstance().backButton.transform.DOScale(0.5f, 0.2f));
        yield return null;
    }

    public void State1Open()
    {
        MainMenu.state = 1;
        MainMenu.Chapter = State2Func;
        select = true;
        if (State2Func == "PhotoVideo") State2Open();
        else StartCoroutine(Open1stState());
        Sfx.GearClick();
    }

    IEnumerator Close1stState()
    {
        Sequence change = DOTween.Sequence();
        change.Append(MainMenu.GetInstance().backButton.transform.DOScale(0.55f, 0.2f));
        change.Append(MainMenu.GetInstance().backButton.transform.DOScale(0f, 0.5f));
        yield return new WaitForSeconds(0.1f);
        if (MainMenu.Chapter != "PhotoVideo")
        {
            MainMenu.GetInstance().background.DOFade(1, 0.3f);
            yield return new WaitForSeconds(0.3f);

            for (int i = 0; i < MainMenu.GetInstance().m_mainButtons.Length; i++)
            {
                MainMenu.GetInstance().m_mainButtons[i].GetComponent<Renderer>().materials[1].SetTexture("_MainTex", MainMenu.GetInstance().m_mainImages[i]);
            }
            yield return new WaitForSeconds(0.2f);
            MainMenu.GetInstance().background.DOFade(0, 0.3f);
        }
        MainMenu.state = 0;
        select = false;
    }

    public void State1Close()
    {
        StartCoroutine(Close1stState());
    }

    #endregion

    #region 2ndState

    IEnumerator Open1stGear()
    {
        MainMenu.GetInstance().backButton.isActive = false;
        Sequence change = DOTween.Sequence();
        openState = DOTween.Sequence();
        Sequence flight = DOTween.Sequence();
        transform.DOKill();
        Gear.transform.DOKill();
        change.Append(transform.GetComponentInChildren<Light>().DOIntensity(6, 0.5f));

        change.Append(MainMenu.GetInstance().m_mainButtons[3].transform.DOBlendableLocalMoveBy(new Vector3(-0.72f, -0.6f, 0), 0.3f));
        change.Append(MainMenu.GetInstance().m_mainButtons[2].transform.DOBlendableLocalMoveBy(new Vector3(-0.72f, 0f, -0.21f), 0.3f));

        // поднятие 2х шестеренок

        change.Append(MainMenu.GetInstance().m_mainButtons[0].transform.DOLocalMoveZ(-0.99f, 0.3f));
        change.Append(MainMenu.GetInstance().m_mainButtons[0].transform.DORotate(new Vector3(40f, 0f, 0f), 1f).SetEase(Ease.InBack));
        // выезд главной
        change.Append(MainMenu.GetInstance().m_mainButtons[2].transform.DOBlendableLocalMoveBy(new Vector3(0.72f, 0f, 0.21f), 0.3f));
        change.Append(MainMenu.GetInstance().m_mainButtons[3].transform.DOBlendableLocalMoveBy(new Vector3(0.72f, 0.6f, 0), 0.3f));
        // закрытие 2х шестеренок
        yield return change.WaitForCompletion();
        MainMenu.GetInstance().SendMessage(MainMenu.Chapter, buttonNumber);
        flight.Append(MainMenu.GetInstance().m_mainButtons[0].transform.DOLocalMove(openPosition, 0.5f))
        .Join(MainMenu.GetInstance().m_mainButtons[0].transform.DORotate(Vector3.zero, 0.5f))
        .Join(MainMenu.GetInstance().m_mainButtons[0].transform.DOScale(openScale, 0.5f));
        yield return flight.WaitForCompletion();
        isOpen = true;
        //Gear.transform.DOLocalRotate(new Vector3(0, 0, direct * 480), 20, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo);
        m_light = (transform.GetComponentInChildren<Light>().DOIntensity(5, 2f).SetLoops(-1, LoopType.Yoyo));
    }

    IEnumerator Close1stGear()
    {
        m_light.Kill();
        Gear.transform.DOKill();
        isOpen = false;
        Sequence change = DOTween.Sequence();
        MainMenu.CloseScreen();
        transform.GetComponentInChildren<Light>().DOIntensity(6, 0.5f);
        yield return MainMenu.GetInstance().Scr.WaitForCompletion();

        change.Append(MainMenu.GetInstance().m_mainButtons[0].transform.DOLocalJump(new Vector3(3.73f, 0.76f, -0.99f), 0.5f, 1, 0.3f))
        .Join(MainMenu.GetInstance().m_mainButtons[0].transform.DOScale(1, 0.5f));
        yield return new WaitForSeconds(0.5f);

        MainMenu.GetInstance().m_mainButtons[3].transform.DOBlendableLocalMoveBy(new Vector3(-0.72f, -0.6f, 0), 0.3f);
        yield return new WaitForSeconds(0.3f);
        MainMenu.GetInstance().m_mainButtons[2].transform.DOBlendableLocalMoveBy(new Vector3(-0.72f, 0f, -0.21f), 0.3f);
        yield return new WaitForSeconds(0.3f);
        MainMenu.GetInstance().m_mainButtons[0].transform.DOLocalMoveZ(0.5f, 0.3f);
        yield return new WaitForSeconds(0.3f);
        // заезд главной

        MainMenu.GetInstance().m_mainButtons[2].transform.DOBlendableLocalMoveBy(new Vector3(0.72f, 0f, 0.21f), 0.3f);
        yield return new WaitForSeconds(0.3f);
        MainMenu.GetInstance().m_mainButtons[3].transform.DOBlendableLocalMoveBy(new Vector3(0.72f, 0.6f, 0), 0.3f);
        yield return new WaitForSeconds(0.3f);
        // закрытие 2х шестеренок
        transform.GetComponentInChildren<Light>().DOIntensity(0, 0.5f);
        MainMenu.state = 1;
    }

    IEnumerator Open2ndGear()
    {
        MainMenu.GetInstance().backButton.isActive = false;
        Sequence change = DOTween.Sequence();
        openState = DOTween.Sequence();
        Sequence flight = DOTween.Sequence();
        transform.DOKill();
        Gear.transform.DOKill();
        if (MainMenu.Chapter == "PhotoVideo")
        {
            change.Append(MainMenu.GetInstance().backButton.transform.DOScale(0.55f, 0.2f));
            change.Append(MainMenu.GetInstance().backButton.transform.DOScale(0.5f, 0.2f));
        }

        change.Append(transform.GetComponentInChildren<Light>().DOIntensity(6, 0.5f));


        change.Append(MainMenu.GetInstance().m_mainButtons[3].transform.DOLocalMove(new Vector3(0.48f, -0.52f, 0.3f), 0.3f));
        change.Append(MainMenu.GetInstance().m_mainButtons[1].transform.DOLocalMove(new Vector3(-2.1f, -1.43f, 0f), 0.3f));
        change.Append(MainMenu.GetInstance().m_mainButtons[3].transform.DOLocalMove(MainMenu.GetInstance().m_mainButtons[3].standartPosion, 0.3f));
        yield return change.WaitForCompletion();

        flight.Append(MainMenu.GetInstance().m_mainButtons[1].transform.DOLocalMove(openPosition, 0.5f))
        .Insert(0f, MainMenu.GetInstance().m_mainButtons[1].transform.DOScale(openScale, 0.5f));
        yield return flight.WaitForCompletion();
        isOpen = true;
        MainMenu.GetInstance().SendMessage(MainMenu.Chapter, buttonNumber);

        //   Gear.transform.DOLocalRotate(new Vector3(0, 0, direct * 480), 16, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo);
        m_light = (transform.GetComponentInChildren<Light>().DOIntensity(5, 2f).SetLoops(-1, LoopType.Yoyo));
      //для 25ти летия ролик
   /*     if (MainMenu.Chapter == "PhotoVideo")
        {
            yield return new WaitForSeconds(2.5f);
            foreach (VideoButton btn in FindObjectsOfType(typeof(VideoButton)))
            {
                btn.RemoteClick();
            }
        }*/
    }

    IEnumerator Close2ndGear()
    {
        foreach (CanvasGroup disp in FindObjectsOfType(typeof(CanvasGroup)))
        {
            if (disp.tag == "PhotoScreen")
            {
                disp.DOFade(0, 1);
                disp.blocksRaycasts = false;
                disp.interactable = false;
                foreach (RectTransform c in disp.GetComponentInChildren<Gallery>().Container.GetComponentInChildren<RectTransform>())
                {
                    Destroy(c.gameObject);
                }
            }
        }
        m_light.Kill();
        Gear.transform.DOKill();
        isOpen = false;
        Sequence change = DOTween.Sequence();
        MainMenu.CloseScreen();
        transform.GetComponentInChildren<Light>().DOIntensity(6, 0.5f);
        yield return MainMenu.GetInstance().Scr.WaitForCompletion();
        
        MainMenu.GetInstance().m_mainButtons[3].transform.DOLocalMove(new Vector3(0.48f, -0.52f, 0.3f), 0.3f);
        yield return new WaitForSeconds(0.3f);
        change.Append(MainMenu.GetInstance().m_mainButtons[1].transform.DOLocalMove(standartPosion, 0.5f))
        .Insert(0f, MainMenu.GetInstance().m_mainButtons[1].transform.DOScale(1, 0.5f));
        yield return new WaitForSeconds(0.5f);
        MainMenu.GetInstance().m_mainButtons[3].transform.DOLocalMove(MainMenu.GetInstance().m_mainButtons[3].standartPosion, 0.3f);
        yield return new WaitForSeconds(0.3f);
        transform.GetComponentInChildren<Light>().DOIntensity(0, 0.5f);
        MainMenu.state = 1;
        if (MainMenu.Chapter == "PhotoVideo")
        {
            yield return StartCoroutine(Close1stState());
        }
    }

    IEnumerator Open3thGear()
    {
        MainMenu.GetInstance().backButton.isActive = false;
        Sequence change = DOTween.Sequence();
        openState = DOTween.Sequence();
        Sequence flight = DOTween.Sequence();

        transform.DOKill();
        Gear.transform.DOKill();
        transform.GetComponentInChildren<Light>().DOIntensity(6, 0.5f);

        change.Append(MainMenu.GetInstance().m_mainButtons[3].transform.DOLocalMove(new Vector3(0.1f, -0.96f, 0.3f), 0.3f));
        change.Append(MainMenu.GetInstance().m_mainButtons[2].transform.DOLocalMove(new Vector3(1f, 1.61f, -0.51f), 0.3f));
        change.Insert(0.5f, MainMenu.GetInstance().m_mainButtons[3].transform.DOLocalMove(MainMenu.GetInstance().m_mainButtons[3].standartPosion, 0.3f));
        yield return change.WaitForCompletion();
        // закрытие 2х шестеренок
        MainMenu.GetInstance().SendMessage(MainMenu.Chapter, buttonNumber);
        flight.Append(MainMenu.GetInstance().m_mainButtons[2].transform.DOLocalMove(openPosition, 0.5f))
        .Insert(0f, MainMenu.GetInstance().m_mainButtons[2].transform.DORotate(Vector3.zero, 0.5f))
        .Insert(0f, MainMenu.GetInstance().m_mainButtons[2].transform.DOScale(openScale, 0.5f));
        yield return flight.WaitForCompletion();
        isOpen = true;
     //   Gear.transform.DOLocalRotate(new Vector3(0, 0, direct * 480), 16, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo);
        m_light = (transform.GetComponentInChildren<Light>().DOIntensity(5, 2f).SetLoops(-1, LoopType.Yoyo)).SetDelay(3.1f);
        yield return null;
    }

    IEnumerator Close3thGear()
    {
        m_light.Kill();
        Gear.transform.DOKill();
        isOpen = false;
        Sequence change = DOTween.Sequence();
        MainMenu.CloseScreen();
        transform.GetComponentInChildren<Light>().DOIntensity(6, 0.5f);
        yield return MainMenu.GetInstance().Scr.WaitForCompletion();


        MainMenu.GetInstance().m_mainButtons[3].transform.DOLocalMove(new Vector3(0.1f, -0.96f, 0.3f), 0.3f);
        yield return new WaitForSeconds(0.3f);

        change.Append(MainMenu.GetInstance().m_mainButtons[2].transform.transform.DOLocalMove(new Vector3(1f, 1.61f, -0.51f), 0.3f));
        change.Insert(0.3f, MainMenu.GetInstance().m_mainButtons[2].transform.DOScale(1, 0.5f));
        yield return new WaitForSeconds(0.5f);

        MainMenu.GetInstance().m_mainButtons[2].transform.DOLocalMove(MainMenu.GetInstance().m_mainButtons[2].standartPosion, 0.3f);
        yield return new WaitForSeconds(0.3f);
        MainMenu.GetInstance().m_mainButtons[3].transform.DOLocalMove(MainMenu.GetInstance().m_mainButtons[3].standartPosion, 0.3f);
        yield return new WaitForSeconds(0.3f);
        transform.GetComponentInChildren<Light>().DOIntensity(0, 0.5f);
        MainMenu.state = 1;
    }

    IEnumerator Open4thGear()
    {
        MainMenu.GetInstance().backButton.isActive = false;
        Sequence change = DOTween.Sequence();
      
        transform.DOKill();
        Gear.transform.DOKill();
        transform.GetComponentInChildren<Light>().DOIntensity(6, 0.5f);
        MainMenu.GetInstance().SendMessage(MainMenu.Chapter, buttonNumber);
        MainMenu.GetInstance().m_mainButtons[3].transform.DOLocalMove(new Vector3(-0.72f, 0.26f, -0.9f), 0.3f);
        
        yield return new WaitForSeconds(0.5f);
        change.Append(MainMenu.GetInstance().m_mainButtons[3].transform.DOLocalMove((openPosition), 0.5f))
        .Insert(0.8f, MainMenu.GetInstance().m_mainButtons[3].transform.DOScale(openScale, 0.5f));
        yield return new WaitForSeconds(0.3f);
        // закрытие 2х шестеренок
        
        isOpen = true;
    //    Gear.transform.DOLocalRotate(new Vector3(0, 0, direct * 480), 12f, RotateMode.Fast).SetLoops(-1, LoopType.Yoyo);
        m_light = (transform.GetComponentInChildren<Light>().DOIntensity(5, 2f).SetLoops(-1, LoopType.Yoyo));
        yield return null;
    }

    IEnumerator Close4thGear()
    {
        m_light.Kill();
        Gear.transform.DOKill();
        isOpen = false;
        Sequence change = DOTween.Sequence();
        MainMenu.CloseScreen();
        transform.GetComponentInChildren<Light>().DOIntensity(6, 0.5f);
        yield return MainMenu.GetInstance().Scr.WaitForCompletion();

        change.Append(MainMenu.GetInstance().m_mainButtons[3].transform.DOLocalMove(new Vector3(-0.72f, 0.26f, -0.9f), 0.3f));
        change.Insert(0.3f, MainMenu.GetInstance().m_mainButtons[3].transform.DOScale(1, 0.5f));
        yield return new WaitForSeconds(0.5f);
        MainMenu.GetInstance().m_mainButtons[3].transform.DOLocalMove(standartPosion, 0.3f);
        yield return new WaitForSeconds(0.3f);
        transform.GetComponentInChildren<Light>().DOIntensity(0, 0.5f);
        MainMenu.state = 1;
    }

    public void State2Open() {
        MainMenu.state = 2;
        switch (animationType)
        {
            case 1:
                {
                    StopAllCoroutines();
                    StartCoroutine(Open1stGear());
                    break; }
            case 2:
                {
                    StopAllCoroutines();
                    StartCoroutine(Open2ndGear());
                    break;
                }
            case 3:
                {
                    StopAllCoroutines();
                    StartCoroutine(Open3thGear());
                    break;
                }
            case 4:
                {
                    StopAllCoroutines();
                    StartCoroutine(Open4thGear());
                    break;
                }
        }
        Sfx.GearClick();
    }

    public void State2Close() {
        switch (animationType)
        {
            case 1:
                {
                    StopAllCoroutines();
                    StartCoroutine(Close1stGear());
                    break; }
            case 2:
                {
                    StopAllCoroutines();
                    StartCoroutine(Close2ndGear());
                    break; }
            case 3:
                {
                    StopAllCoroutines();
                    StartCoroutine(Close3thGear());
                    break; }
            case 4:
                {
                    StopAllCoroutines();
                    StartCoroutine(Close4thGear());
                    break; }
        }
    }
    #endregion
}
