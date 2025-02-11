using GameAUI;
using UnityEngine;

namespace GameA
{
    public class EventController : MonoBehaviour
    {
        private void Start()
        {
            Managers.MainLogic.SetGameBoardCallBack(
                (gameBoard) => Managers.UI.GetLayout<GameBoardLayout>().SetBoard(gameBoard),
                (score) => Managers.UI.GetLayout<GameBoardLayout>().SetScore(score),
                (time) => Managers.UI.GetLayout<GameBoardLayout>().StartTimer(time));

            StartGame();
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
    }
}
