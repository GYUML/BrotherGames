using GameALogic;
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
            AdventureLobby = 2,
            AdventureGame = 3
        }

        private void Start()
        {
            Managers.MainLogic.SetGameBoardCallBack(
                (gameBoard) => Managers.UI.GetLayout<GameBoardLayout>().SetBoard(gameBoard),
                (score) => Managers.UI.GetLayout<GameBoardLayout>().SetScore(score),
                (remain) => Managers.UI.GetLayout<GameBoardLayout>().SetRemainCount(remain),
                (maxTime, nowTime) => Managers.UI.GetLayout<GameBoardLayout>().StartTimer(maxTime, nowTime),
                OnEndGame,
                (row, column) => Managers.UI.GetLayout<GameBoardLayout>().ShowAcquireEffect(row, column),
                (targetCombo, nowCombo) => Managers.UI.GetLayout<GameBoardLayout>().SetCombo(targetCombo, nowCombo));

            Managers.AdventureLogic.SetGameBoardCallBack(
                (gameBoard) => Managers.UI.GetLayout<GameBoardAdventureLayout>().SetBoard(gameBoard),
                (targetScore, nowScore) => Managers.UI.GetLayout<GameBoardAdventureLayout>().SetQuestCount(targetScore, nowScore),
                (remain) => Managers.UI.GetLayout<GameBoardAdventureLayout>().SetRemainCount(remain),
                (maxTime, nowTime) => Managers.UI.GetLayout<GameBoardAdventureLayout>().StartTimer(maxTime, nowTime),
                OnAdventureGameEnd,
                (row, column) => Managers.UI.GetLayout<GameBoardAdventureLayout>().ShowAcquireEffect(row, column));

            MovePage(Page.Lobby);
            Application.targetFrameRate = 60;
        }

        public void StartGame()
        {
            var rowSize = 10;
            var colSize = 7;

            Managers.MainLogic.StartGame(rowSize, colSize);
        }

        public void StartAdventureGame()
        {
            var rowSize = 10;
            var colSize = 7;

            Managers.AdventureLogic.StartGame(rowSize, colSize, 10);
        }

        public void DragEnd(Vector2Int startPoint, Vector2Int endPoint)
        {
            Managers.MainLogic.DragEnd(startPoint, endPoint);
        }

        public void DragEndAdventure(Vector2Int startPoint, Vector2Int endPoint)
        {
            Managers.AdventureLogic.DragEnd(startPoint, endPoint);
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
            else if (page == Page.AdventureLobby)
            {
                Managers.UI.ShowLayout<AdventureLobbyLayout>();
            }
            else if (page == Page.AdventureGame)
            {
                Managers.UI.ShowLayout<GameBoardAdventureLayout>();
                StartAdventureGame();
            }
        }

        void OnEndGame(int score)
        {
            Managers.UI.ShowPopup<LoadingPopup>();
            Managers.UI.GetLayout<GameBoardLayout>().StopGame();
            Managers.Web.SetMyRanking(
                (data) =>
                {
                    Managers.UI.HidePopup<LoadingPopup>();
                    Managers.UI.ShowPopup<GameResultPopup>().SetScore(score);
                },
                () =>
                {
                    Managers.UI.HidePopup<LoadingPopup>();
                    Managers.UI.ShowPopup<MessagePopup>().Set("Error", "Failed to register score.");
                    Managers.Event.MovePage(Page.Lobby);
                },
                Managers.UserData.GetUserUID(), score);
        }

        void OnAdventureGameEnd(int targetScore, int nowScore)
        {
            //Managers.UI.ShowPopup<LoadingPopup>();
            Managers.UI.GetLayout<GameBoardAdventureLayout>().StopGame();
            Managers.UI.ShowPopup<MessagePopup>().Set("Result", nowScore >= targetScore ? "Success" : "Failed");
            MovePage(Page.Lobby);
        }
    }
}
