using UnityEngine;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class MediaGallery : MonoBehaviour {

    string folderpath;
    string[] FullFileList;
    GameObject GalleryButton;
    public RectTransform gallery;
    public HorisontalScrollSnaper snaper;

    public void Awake()
    {
        GalleryButton = Resources.Load("Gallery_Container") as GameObject;
    }

    void Start()
    {
        LoadAllMedia();
        StartCoroutine(LoadSprite());
        snaper.SetElements(FullFileList.Length);
    }

    public void LoadAllMedia()
    {
        
        folderpath = Application.streamingAssetsPath + "/Gallery/";
        if (Directory.Exists(folderpath))
        {
            FullFileList = Directory.GetFiles(folderpath, "*.*").Where(str => str.EndsWith(".png")).ToArray();
        }
    }

    public IEnumerator LoadSprite()
    {
        string finalPath;
        WWW localFile;
        Texture texture;
        Sprite sprite;
        foreach (string c in FullFileList)
        {
            finalPath = "file://" + c;
            localFile = new WWW(finalPath);
            yield return localFile;
            texture = localFile.texture;
            sprite = Sprite.Create(texture as Texture2D, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            GameObject newButton = Instantiate(GalleryButton) as GameObject;
            newButton.GetComponent<Image>().sprite = sprite;
            newButton.transform.SetParent(gallery, false);
        }
    }
}
