using GameA;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LobbyLayout : UILayout
{
    public KButton startButton;
    public KButton rankingButton;

    private void Start()
    {
        startButton.onClick.AddListener(()=>Managers.Event.MovePage(EventController.Page.MainGame));
        rankingButton.onClick.AddListener(ShowRankingPopup);
    }

    void ShowRankingPopup()
    {
        Managers.UI.ShowPopup<LoadingPopup>();
        Managers.Web.GetRankingList(
            (data) =>
            {
                try
                {
                    Debug.Log($"[LobbyLayout] ShowRankingPopup() data={data}");

                    var rankings = JsonConvert.DeserializeObject<List<RankingPopup.RankingData>>(data);
                    Managers.UI.ShowPopup<RankingPopup>().Set(rankings);
                }
                catch (Exception ex)
                {
                    Debug.Log($"[LobbyLayout] ShowRankingPopup() Failed to parse data. ex = {ex}");
                }
                finally
                {
                    Managers.UI.HidePopup<LoadingPopup>();
                }
            },
            () =>
            {
                Managers.UI.HidePopup<LoadingPopup>();
                Managers.UI.ShowPopup<MessagePopup>().Set("Error", "Failed to get ranking data");
            });
    }
}
