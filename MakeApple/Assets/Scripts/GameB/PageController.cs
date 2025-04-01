using GameB;
using UnityEngine;

namespace GameB
{
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

        public void MovePage(Page page)
        {
            SetUI(page);
            SetGameObject(page);
            SetLogic(page);
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
                    Managers.UI.ShowLayout<FallingGameLayout>();
                    Managers.UI.ShowPopup<FallingGameReadyPopup>();
                    break;
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

        void SetLogic(Page page)
        {
            switch (page)
            {
                case Page.FallingGame:
                    Managers.GameLogic.SetGame();
                    break;
            }
        }
    }

}
