using GameA;
using GameAUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultPopup : UIPopup
{
    public TMP_Text scoreText;
    public Button homeButton;

    private void Start()
    {
        homeButton.onClick.AddListener(OnClickHome);
    }

    public void SetScore(long score)
    {
        scoreText.text = score.ToFormat();
    }

    void OnClickHome()
    {
        Managers.Event.MovePage(EventController.Page.Lobby);
        Managers.UI.HidePopup<GameResultPopup>();
    }
}
