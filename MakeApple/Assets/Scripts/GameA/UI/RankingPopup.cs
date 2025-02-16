using GameA;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingPopup : UIPopup
{
    public KButton homeButton;
    public RankingPopupItem rankItemPrefab;

    List<RankingPopupItem> rankItemPool = new List<RankingPopupItem>();

    private void Start()
    {
        rankItemPrefab.gameObject.SetActive(false);
        homeButton.onClick.AddListener(() => Managers.UI.HidePopup<RankingPopup>());
    }

    public void Set(List<RankingData> rankings)
    {
        for (int i = rankItemPool.Count; i < rankings.Count; i++)
        {
            var rankItem = Instantiate(rankItemPrefab, rankItemPrefab.transform.parent);
            rankItemPool.Add(rankItem);
        }

        for (int i = 0; i < rankings.Count; i++)
        {
            var rankingData = rankings[i];
            var rankItem = rankItemPool[i];
            rankItem.Set(i + 1, rankingData.name, rankingData.score);
        }

        for (int i = rankings.Count; i < rankItemPool.Count; i++)
            rankItemPool[i].gameObject.SetActive(false);
    }

    public class RankingData
    {
        public string name;
        public int score;
    }
}
