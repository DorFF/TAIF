using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class PhotoScanner : MonoBehaviour {

    string folderpath;
    public string[] FullFileList;
    GameObject PhotoButton;
    public RectTransform PhotoGallery;
    public CanvasGroup m_Loading;

    public void Awake()
    {
        PhotoButton = Resources.Load("Photo_Container") as GameObject;
        LoadAllMedia();
    }

    void Start()
    {
        StartCoroutine(LoadAllMedia());
    }

    public IEnumerator LoadAllMedia()
    {
        float resize = 0;
        folderpath = Application.streamingAssetsPath + "/Photos/";
        FullFileList = Directory.GetDirectories(folderpath).OrderByDescending(d => new FileInfo(d).Name).ToArray();
        
        foreach (string c in FullFileList)
        {
            GameObject newButton = Instantiate(PhotoButton) as GameObject;
            newButton.transform.SetParent(PhotoGallery, false);
            if (Directory.Exists(c))
            {
                newButton.GetComponent<PhotoFolder>().FullFileList = Directory.GetFiles(c, "*.*").Where(str => str.EndsWith(".png") || str.EndsWith(".jpg") || str.EndsWith(".bmp")).ToArray();
                yield return StartCoroutine(LoadSprite(newButton.GetComponent<PhotoFolder>().FullFileList, newButton.GetComponent<PhotoFolder>().m_sprites));
            }
            newButton.GetComponentInChildren<Text>().text = c.TrimEnd('/').Split('/').Last();
            newButton.GetComponent<PhotoFolder>().Header = c.TrimEnd('/').Split('/').Last();
            Sprite preview = newButton.GetComponent<PhotoFolder>().m_sprites[0];
            if (preview.rect.width / preview.rect.height > 1) resize = 512 / preview.rect.width;
            if (preview.rect.width / preview.rect.height < 1) resize = 341.3f / preview.rect.height;
            newButton.transform.FindChild("Preview").GetComponent<Image>().sprite = preview;
            newButton.transform.FindChild("Preview").GetComponent<RectTransform>().sizeDelta = new Vector2(preview.rect.width * resize, preview.rect.height * resize);
        }
      
        float x = PhotoGallery.GetComponent<RectTransform>().sizeDelta.x;
        PhotoFolder[] pf = (PhotoFolder[])FindObjectsOfType(typeof(PhotoFolder));
        float count = pf.Length;
        PhotoGallery.GetComponent<RectTransform>().sizeDelta = new Vector2(x, (512 + 90) * (float)System.Math.Ceiling(count / 4));
        if (PhotoGallery.GetComponent<RectTransform>().sizeDelta.y < 830) PhotoGallery.GetComponentInParent<ScrollRect>().vertical = false;
        yield return StartCoroutine(GetComponent<VideoScanner>().LoadAllVideos());
        m_Loading.DOFade(0, 0.5f);
        m_Loading.blocksRaycasts = false;
        yield return new WaitForSeconds(0.4f);
        MainMenu.GetInstance().GetComponent<AudioSource>().mute = false;
    }

    public IEnumerator LoadSprite(string[] FileList, List<Sprite> m_sprites)
    {
        string finalPath;
        WWW localFile;
        Texture texture;
        Sprite sprite;
        foreach (string c in FileList)
        {
            finalPath = "file://" + c;
            localFile = new WWW(finalPath);
            yield return localFile;
            texture = localFile.texture;
            sprite = Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            m_sprites.Add(sprite);
        }
    }


}
