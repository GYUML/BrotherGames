using GameB;
using UnityEngine;

public class PageController : MonoBehaviour
{
    public enum Page
    {
        None,
        Town,
        FallingGame,
        EndGame
    }

    public GameObject fallingGame;
    public GameObject endGame;
    public GameObject town;

    private void Start()
    {
        MovePage(Page.Town);
    }

    void MovePage(Page page)
    {
        SetUI(page);
        SetGameObject(page);
    }

    void SetUI(Page page)
    {
        Managers.UI.HideAllLayout();

        switch (page)
        {
            case Page.Town:
                Managers.UI.ShowLayout<TownLayout>();
                break;
            case Page.FallingGame:
            case Page.EndGame:
                Managers.UI.ShowLayout<FallingGameLayout>();
                break;
        }
    }

    void SetGameObject(Page page)
    {
        town.SetActive(page == Page.Town);
        fallingGame.SetActive(page == Page.FallingGame);
        endGame.SetActive(page == Page.EndGame);
    }
}
