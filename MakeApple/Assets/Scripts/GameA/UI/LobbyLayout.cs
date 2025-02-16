using GameA;
using UnityEngine;
using UnityEngine.UI;

public class LobbyLayout : UILayout
{
    public KButton startButton;
    public KButton rankingButton;

    private void Start()
    {
        startButton.onClick.AddListener(()=>Managers.Event.MovePage(EventController.Page.MainGame));
        rankingButton.onClick.AddListener(() => Managers.UI.ShowPopup<RankingPopup>());
    }
}
