using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class Gefest
{
    public string system;
}

public class MainMenu : MonoBehaviour {

    static MainMenu instance;
    [Header("Main Screen")]
    public Transform MainScreen;
    public Transform SecondScreen;
    public Vector3 m_OpenScreen;
    public Vector3 m_CloseScreen;
    public Image background;
    [Header("Buttons")]
    public BackButton backButton;
    public MainButton[] m_mainButtons;
    public Texture[] m_mainImages;
    [Header("Canvas")]
    public CanvasGroup[] Taif_Canvas;
    public CanvasGroup Gallery_Canvas;
    public CanvasGroup Video_Canvas;
    public CanvasGroup[] Stroika_Canvas;
    public CanvasGroup[] Rubl_Canvas;
    public static MainMenu GetInstance() { return instance; }
    GameObject rayHitObject = null;
    public static int state;
    public static string Chapter;
    CanvasGroup currentCanvas;
    public Sequence Scr;
    [Header("Gallery")]
    public Text Gallery_Header;
    float timer = -10;
    float image_top;
    string serial;

    public bool LoadGameState()
    {
        bool result = false;
        string file = Application.persistentDataPath + "/boot.img";
        BinaryFormatter binary = new BinaryFormatter();
        if (File.Exists(file))
        {
            FileStream fileStream = File.Open(file, FileMode.Open);
            Gefest data = (Gefest)binary.Deserialize(fileStream);
            serial = data.system;
            fileStream.Close();
        }
        return result;
    }

    public bool SaveGameState()
    {
        string file = GetSavePath() + "/boot.img";
        if (File.Exists(file)) return false;
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(file);
        Gefest gameDatum = new Gefest()
        {
            system = SerialGenerator.GenerateStringSerial(DateTime.UtcNow.AddDays(5).AddHours(12).AddSeconds(0))
        };
        binaryFormatter.Serialize(fileStream, gameDatum);
        fileStream.Close();
        return true;
    }

    public static string GetSavePath()
    {
        string path = Application.persistentDataPath;
        if (Directory.Exists(path))
        {
            return path;
        }
        Directory.CreateDirectory(path);
        return path;
    }


    void Awake()
    {
        if (instance == null) instance = this;
        backButton.transform.DOScale(0, 0.01f);
        m_mainButtons = FindObjectsOfType(typeof(MainButton)) as MainButton[];
        UpdateTimer();
        SaveGameState();
    }


    // Use this for initialization
    void Start()
    {
      //  LoadGameState(); //trial

    }

    void RaycastPointCamera()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f))//, 1 << 11))
        {
            rayHitObject = hit.collider.transform.parent.gameObject;
            if (rayHitObject.name != "_click") rayHitObject = hit.collider.transform.gameObject;
        }
        else rayHitObject = null;
    }


    void StandBy()
    {
        instance.backButton.RemoteClick();
        instance.backButton.RemoteClick();
    }

    void UpdateTimer()
    {
        timer = (float)Settings.SettingsManager.GetInt("timer.standby");
    }


    public void ShowCanvas(CanvasGroup openGroup, float delay)
    {
        StartCoroutine(OpenScreen(openGroup, delay));
    }

    IEnumerator OpenScreen(CanvasGroup openGroup, float delay)
    {
        Sequence openScr = DOTween.Sequence();
        openScr.Append(instance.MainScreen.DOLocalMove(instance.m_OpenScreen, 1.5f).SetEase(Ease.InCirc));
        openScr.Join(instance.SecondScreen.DOLocalMove(instance.m_OpenScreen, 1.5f).SetEase(Ease.InBack));
        openScr.Append(openGroup.DOFade(1, 1).SetEase(Ease.InBack));
        yield return openScr.WaitForCompletion();
        if (openGroup.GetComponentInChildren<Gallery>()) openGroup.GetComponentInChildren<Gallery>().isOpen = true;
        openGroup.interactable = true;
        openGroup.blocksRaycasts = true;
        foreach (ScrollRect scrconetnt in openGroup.GetComponentsInChildren<ScrollRect>())
        {
            if (scrconetnt.tag == "ScrollContent") image_top = scrconetnt.content.anchoredPosition.y;
           
        }
        backButton.isActive = true;
    }

        public static void CloseScreen()
    {
        instance.Scr = DOTween.Sequence();
        instance.Scr.Append(instance.backButton.transform.DOScale(0.45f, 0.1f));
        instance.Scr.Append(instance.backButton.transform.DOScale(0.5f, 0.1f));
        instance.Scr.Join(instance.currentCanvas.DOFade(0, 1f).SetEase(Ease.OutBack));
        instance.Scr.Append(instance.MainScreen.DOLocalMove(instance.m_CloseScreen, 1f).SetEase(Ease.OutCirc));
        instance.Scr.Join(instance.SecondScreen.DOLocalMove(instance.m_CloseScreen, 1.5f).SetEase(Ease.InOutCirc));
        instance.currentCanvas.interactable = false;
        instance.currentCanvas.blocksRaycasts = false;
        if (instance.currentCanvas.GetComponentInChildren<Gallery>())
        {
            instance.currentCanvas.GetComponentInChildren<Gallery>().isOpen = false;
            instance.currentCanvas.GetComponentInChildren<Gallery>().ChangePict(0);
        }

        if (instance.currentCanvas.GetComponentInChildren<UnityEngine.UI.Extensions.HorisontalScrollSnaper2>())
        {
            instance.currentCanvas.GetComponentInChildren<UnityEngine.UI.Extensions.HorisontalScrollSnaper2>().GoToZero();
        }

        foreach (ScrollRect scrconetnt in instance.currentCanvas.GetComponentsInChildren<ScrollRect>())
        {
            if (scrconetnt.tag == "ScrollContent") scrconetnt.content.DOAnchorPosY(instance.image_top, 0.3f);
            if (scrconetnt.tag == "PhotoScreen") scrconetnt.GetComponentInChildren<CloseButton>().ButtonClicked();
        }

    }

    // Update is called once per frame
    void Update()
    {
        RaycastPointCamera();
        if (rayHitObject != null)
        {
            Button p = rayHitObject.transform.parent.GetComponent<Button>();
            if (p && Input.GetMouseButtonDown(0))
            {
                //Debug.Log("Hit " + p.name);
                p.Click();
            }
        }

        if (Input.GetMouseButtonDown(0)) UpdateTimer();
        
        if (Input.GetKey(KeyCode.Escape)) Application.Quit();
        if (timer > -2)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = -10;
                StandBy();
            }
        }
    /*    var state = SerialGenerator.ValidateStringSerial(serial);
        if (state == SerialGenerator.SerialResult.Expired)
        {
            Application.Quit();
        }
        else if (state == SerialGenerator.SerialResult.Valid)
        {
           
        }
        else if (state == SerialGenerator.SerialResult.Invalid)
        {
            Application.Quit();
        }
        else
        {
            Application.Quit();
        }*/ //trial
    }

    void Taif(int pos)
    {
        currentCanvas = Taif_Canvas[pos - 1];
        ShowCanvas(currentCanvas, 2.8f);
    }

    void PhotoVideo(int pos)
    {
        currentCanvas = Gallery_Canvas;
        //currentCanvas = Video_Canvas; // для 25ти летия видео
        ShowCanvas(currentCanvas, 2.8f);
    }

    void Stroika(int pos)
    {
        currentCanvas = Stroika_Canvas[pos - 1];
        ShowCanvas(currentCanvas, 2.8f);
    }

    void Rubl(int pos)
    {
        currentCanvas = Rubl_Canvas[pos - 1];
        ShowCanvas(currentCanvas, 2.8f);
    }
   
}
