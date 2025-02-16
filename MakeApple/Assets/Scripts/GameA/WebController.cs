using GameA;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class WebController : MonoBehaviour
{
    readonly string CacheDirectoryPath = "CacheData";
    readonly string CacheFileName = "WebCache.json";

    readonly string SetMyRankingUrl = "";
    readonly string GetRankingUrl = "";

    List<WebCache> cachingList = new List<WebCache>();

    private void Start()
    {
        if (Managers.File.TryLoadData<List<WebCache>>($"{CacheDirectoryPath}/{CacheFileName}", out var data))
            cachingList = data;
    }

    public void SetMyRanking(Action<string> onComplete, string uid, int score)
    {
        var url = $"{SetMyRankingUrl}?{uid}&{score}";
        RequestGet(SetMyRankingUrl, onComplete);
    }

    public void GetRankingList(Action<string> onComplete)
    {
        RequestGet(GetRankingUrl, onComplete);
    }

    void RequestGet(string url, Action<string> onComplete, bool needCaching = false)
    {
        StartCoroutine(RequestGetCo(url, onComplete));
    }

    void SaveCacheData(string url, string data)
    {
        if (cachingList.Exists(item => item.url == url))
        {
            var cache = cachingList.Find(item => item.url == url);
            cache.data = data;
            cache.time = DateTime.Now;
        }
        else
        {
            cachingList.Add(new WebCache { url = url, data = data, time = DateTime.Now });
        }

        Managers.File.SaveData(CacheDirectoryPath, CacheFileName, cachingList);
    }

    IEnumerator RequestGetCo(string url, Action<string> onComplete, bool needCaching = false)
    {
        var www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            var data = www.downloadHandler.data;
            onComplete?.Invoke(data.ToString());
            if (needCaching)
                SaveCacheData(url, data.ToString());
        }
    }

    class WebCache
    {
        public string url;
        public string data;
        public DateTime time;
    }
}
