using GameA;
using UnityEngine;
using UnityEngine.UI;

public class LobbyLayout : UILayout
{
    public Button startButton;
    public Button rankingButton;

    private void Start()
    {
        startButton.onClick.AddListener(()=>Managers.Event.MovePage(EventController.Page.MainGame));
    }
}
