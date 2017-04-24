using UnityEngine;
using System.IO;
using System.Collections;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using RenderHeads.Media.AVProVideo;

public class VideoScanner : MonoBehaviour {

    string folderpath;
    string[] FullFileList;
    GameObject VideoButton;
    public RectTransform VideoGallery;
    float _timeStepSeconds;
    private const int NumFrames = 8;

    public void Awake()
    {
        VideoButton = Resources.Load("Video_Container") as GameObject;
        LoadAllMedia();
    }

    void Start()
    {
  //      StartCoroutine(LoadAllVideos());
    }

    public IEnumerator LoadAllVideos()
    {
        foreach (VideoController c in VideoGallery.GetComponentsInChildren<VideoController>())
        {
            IMediaInfo info = c.Player.Info;
            yield return info;
            Texture2D _texture = new Texture2D(info.GetVideoWidth(), info.GetVideoHeight(), TextureFormat.ARGB32, false);
            _texture = c.Player.ExtractFrame(_texture, 2, true, 3000);
            yield return _texture;
            c.GetComponent<RawImage>().texture = _texture;
            c.Player.Rewind(true);
        }
     
    }

    public void LoadAllMedia()
    {

        folderpath = Application.streamingAssetsPath + "/Video/";
        if (Directory.Exists(folderpath))
        {
            FullFileList = Directory.GetFiles(folderpath, "*.*").Where(str => str.EndsWith(".mp4") || str.EndsWith(".mov") || str.EndsWith(".m4v") || str.EndsWith(".mkv") || str.EndsWith(".ts") || str.EndsWith(".webm") || str.EndsWith(".mpg") || str.EndsWith(".wmv")).ToArray();
        }
        foreach (string c in FullFileList)
        {
            GameObject newButton = Instantiate(VideoButton) as GameObject;
            newButton.GetComponent<VideoController>().Player.m_VideoPath = c;
            newButton.GetComponentInChildren<Text>().text = Path.GetFileNameWithoutExtension(c);
            newButton.transform.SetParent(VideoGallery, false);
        }
        float y = VideoGallery.GetComponent<RectTransform>().sizeDelta.y;
        VideoButton[] vf = (VideoButton[])FindObjectsOfType(typeof(VideoButton));
        float count1 = vf.Length;
        VideoGallery.GetComponent<RectTransform>().sizeDelta = new Vector2((990 + 160) * (count1 + 1), y);
        if (VideoGallery.GetComponent<RectTransform>().sizeDelta.x < 2310) VideoGallery.GetComponentInParent<ScrollRect>().horizontal = false;
    }
  
}
