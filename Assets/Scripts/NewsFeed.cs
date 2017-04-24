using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class NewsFeed : MonoBehaviour {

    public RawImage preview;
    public Text date;
    public Text text;
    public string filename;

    private Dictionary<string, string> _info = new Dictionary<string, string>();

    private void Awake()
    {
      
    }

    private void Start()
    {
        OpenNews();
    }

    public void OpenNews()
    {
        if (!File.Exists(filename)) return;

        var strings = File.ReadAllLines(filename);

        foreach (var rawLine in strings)
        {
            var line = rawLine.Trim();

            if (string.IsNullOrEmpty(line)) continue;
            if (line.StartsWith("#")) continue;

            var varName = "";
            var value = "";

            var rawData = line.Split('=');

            if (rawData.Length < 2) continue;

            var varData = rawData[0].Trim().Split(' ');

            if (varData.Length == 1)
            {
                varName = varData[0].Trim().ToLower();
            }
            else if (varData.Length == 2)
            {
                var osType = varData[0].Trim().ToLower();
                varName = varData[1].Trim().ToLower();
            }

            if (_info.ContainsKey(varName)) continue;

            value = rawData[1];

            if (rawData.Length > 2)
            {
                for (var i = 2; i < rawData.Length; i++)
                {
                    value += "=" + rawData[i];
                }
            }

            _info.Add(varName, value.Replace("\\n", "\n").Trim());
        }
        if (strings != null) OpenInfo();
    }

    internal static string Get(object ip)
    {
        throw new NotImplementedException();
    }

    public string Get(string key)
    {
        return Get(key, string.Empty);
    }

    public string Get(string key, string def)
    {
        return _info.ContainsKey(key) ? _info[key] : def;
    }

    public IEnumerator LoadSprite(string absoluteImagePath)
    {
        string finalPath;
        WWW localFile;
        finalPath = "file://" + Application.streamingAssetsPath + "/News/" + absoluteImagePath;
        localFile = new WWW(finalPath);

        yield return localFile;
        preview.texture = localFile.texture;
    }

    public void OpenInfo()
    {
      StartCoroutine(LoadSprite(Get("image")));
      date.text = Get("date");
      text.text = Get("text");
    }
}
