using GameAUI;
using UnityEngine;

namespace GameA
{
    public class EventController : MonoBehaviour
    {
        private void Start()
        {
            Managers.MainLogic.SetGameBoardCallBack((gameBoard) => Managers.UI.GetLayout<GameBoardLayout>().SetBoard(gameBoard));

            GenerateNewGame();
        }

        public void DragEnd(Vector2Int startPoint, Vector2Int endPoint)
        {
            Managers.MainLogic.DragEnd(startPoint, endPoint);

            if (Managers.MainLogic.GetPossibleCase() <= 0)
                GenerateNewGame();
        }

        void GenerateNewGame()
        {
            var rowSize = 10;
            var colSize = 7;

            Managers.MainLogic.GenerateGameBoard(rowSize, colSize);
            Managers.UI.GetLayout<GameBoardLayout>().StartTimer(60f);
        }
    }
}
