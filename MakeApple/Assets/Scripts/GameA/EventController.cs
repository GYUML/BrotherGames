using GameAUI;
using UnityEngine;

namespace GameA
{
    public class EventController : MonoBehaviour
    {
        public enum Page
        {
            Lobby = 0,
            MainGame = 1,
        }

        private void Start()
        {
            Managers.MainLogic.SetGameBoardCallBack(
                (gameBoard) => Managers.UI.GetLayout<GameBoardLayout>().SetBoard(gameBoard),
                (score) => Managers.UI.GetLayout<GameBoardLayout>().SetScore(score),
                (time) => Managers.UI.GetLayout<GameBoardLayout>().StartTimer(time));

            MovePage(Page.Lobby);
        }

        public void StartGame()
        {
            var rowSize = 10;
            var colSize = 7;

            Managers.MainLogic.StartGame(rowSize, colSize);
        }

        public void DragEnd(Vector2Int startPoint, Vector2Int endPoint)
        {
            Managers.MainLogic.DragEnd(startPoint, endPoint);
        }

        public void MovePage(Page page)
        {
            Managers.UI.HideAllLayout();

            if (page == Page.Lobby)
                Managers.UI.ShowLayout<LobbyLayout>();
            else if (page == Page.MainGame)
            {
                Managers.UI.ShowLayout<GameBoardLayout>();
                StartGame();
            }
        }
    }
}
