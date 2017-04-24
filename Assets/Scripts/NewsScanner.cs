using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class NewsScanner : MonoBehaviour
{
    string folderpath;
    public string[] FullFileList;
    public string[] FileList;
    public GridLayoutGroup newsGreed;
    public GameObject NewsFeedPrefab;

    void Awake()
    {
        Scanner();
    }

    private void Start()
    {
     //   StartCoroutine(LoadNews());
        int count = FullFileList.Length;
        for (int i = 0; i < count; i++)
        {
            GameObject newsObj = (GameObject)Instantiate(NewsFeedPrefab, newsGreed.transform);
            newsObj.GetComponent<NewsFeed>().filename = FullFileList[i];
            //newsObj.GetComponent<NewsFeed>().OpenInfo();
            /*   button.GetComponent<RawImage>().texture = gallery.GetTexture(i);
               button.GetComponent<MediaButton>().filename = gallery.FileList[i];*/
            newsObj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        }
    }

    public void Scanner()
    {
        folderpath = Application.streamingAssetsPath + "/News/";
        if (Directory.Exists(folderpath))
        {
           FullFileList = Directory.GetFiles(folderpath, "*.txt").OrderByDescending(d => new FileInfo(d).CreationTime).
            Where(str => str.EndsWith(".txt")).ToArray();
            FileList = Directory.GetFiles(folderpath).Select(x => Path.GetFileNameWithoutExtension(x)).ToArray();
            
        }
      
    }

    IEnumerator LoadNews()
    {
        int count = FullFileList.Length;
        for (int i = 0; i < count; i++)
        {
            GameObject newsObj = (GameObject)Instantiate(NewsFeedPrefab, newsGreed.transform);
            newsObj.GetComponent<NewsFeed>().filename = FullFileList[i];
            //newsObj.GetComponent<NewsFeed>().OpenInfo();
            /*   button.GetComponent<RawImage>().texture = gallery.GetTexture(i);
               button.GetComponent<MediaButton>().filename = gallery.FileList[i];*/
            newsObj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        }
        yield return null;
    }
}
