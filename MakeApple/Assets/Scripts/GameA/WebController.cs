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

    readonly string SetMyRankingUrl = "https://us-central1-makeapple-afe23.cloudfunctions.net/addmessage";
    readonly string GetRankingUrl = "https://us-central1-makeapple-afe23.cloudfunctions.net/getRankings";

    List<WebCache> cachingList = new List<WebCache>();

    private void Start()
    {
        if (Managers.File.TryLoadData<List<WebCache>>($"{CacheDirectoryPath}/{CacheFileName}", out var data))
            cachingList = data;
    }

    public void SetMyRanking(Action<string> onSuccess, Action onFailed, string uid, int score)
    {
        var url = $"{SetMyRankingUrl}?uid={uid}&score={score}";
        RequestGet(url, onSuccess, onFailed);
    }

    public void GetRankingList(Action<string> onComplete, Action onFailed)
    {
        RequestGet(GetRankingUrl, onComplete, onFailed);
    }

    void RequestGet(string url, Action<string> onComplete, Action onFailed, bool needCaching = false)
    {
        StartCoroutine(RequestGetCo(url, onComplete, onFailed));
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

    IEnumerator RequestGetCo(string url, Action<string> onComplete, Action onFailed, bool needCaching = false)
    {
        Debug.Log($"[WebController] RequestGetCo(). url={url}");

        var www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            onFailed?.Invoke();
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            onComplete?.Invoke(www.downloadHandler.text);
            if (needCaching)
                SaveCacheData(url, www.downloadHandler.text);
        }
    }

    class WebCache
    {
        public string url;
        public string data;
        public DateTime time;
    }
}
