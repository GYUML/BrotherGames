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
    public TMP_Text scoreItemPrefab;

    List<TMP_Text> scoreItemPool = new List<TMP_Text>();

    private void Start()
    {
        scoreItemPrefab.gameObject.SetActive(false);
        homeButton.onClick.AddListener(() => Managers.UI.HidePopup<RankingPopup>());
    }

    public void Set(List<Tuple<string, int>> rankings)
    {
        for (int i = scoreItemPool.Count; i < rankings.Count; i++)
        {
            var scoreItem = Instantiate(scoreItemPrefab, scoreItemPrefab.transform.parent);
            scoreItemPool.Add(scoreItem);
        }

        for (int i = 0; i < rankings.Count; i++)
        {
            var ranking = rankings[i];
            var scoreItem = scoreItemPool[i];
            scoreItem.text = $"{i + 1}. {ranking.Item1} {ranking.Item2.ToFormat()}";
        }

        for (int i = rankings.Count; i < scoreItemPool.Count; i++)
            scoreItemPool[i].gameObject.SetActive(false);
    }
}
