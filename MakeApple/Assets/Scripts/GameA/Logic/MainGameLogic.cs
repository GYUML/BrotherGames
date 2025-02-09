using System;
using UnityEngine;

namespace GameALogic
{
    public class MainGameLogic : MonoBehaviour
    {
        int[,] gameBoard;
        Action<int[,]> onUpdateGameBoard;

        public void SetGameBoardCallBack(Action<int[,]> onUpdateGameBoard)
        {
            this.onUpdateGameBoard = onUpdateGameBoard;
        }

        public void GenerateGameBoard(int rowSize, int columnSize)
        {
            gameBoard = new int[rowSize, columnSize];

            for (int i = 0; i < rowSize; i++)
                for (int j = 0; j < columnSize; j++)
                    gameBoard[i, j] = UnityEngine.Random.Range(1, 10);

            onUpdateGameBoard?.Invoke(gameBoard);
        }

        public void DragEnd(Vector2Int startPoint, Vector2Int endPoint)
        {
            if (IsSuccess(startPoint, endPoint))
            {
                var minPoint = new Vector2Int(Mathf.Min(startPoint.x, endPoint.x), Mathf.Min(startPoint.y, endPoint.y));
                var maxPoint = new Vector2Int(Mathf.Max(startPoint.x, endPoint.x), Mathf.Max(startPoint.y, endPoint.y));

                for (int i = minPoint.x; i <= maxPoint.x; i++)
                    for (int j = minPoint.y; j <= maxPoint.y; j++)
                        gameBoard[i, j] = 0;

                onUpdateGameBoard?.Invoke(gameBoard);
            }
        }

        bool IsSuccess(Vector2Int startPoint, Vector2Int endPoint)
        {
            var minPoint = new Vector2Int(Mathf.Min(startPoint.x, endPoint.x), Mathf.Min(startPoint.y, endPoint.y));
            var maxPoint = new Vector2Int(Mathf.Max(startPoint.x, endPoint.x), Mathf.Max(startPoint.y, endPoint.y));
            var sum = 0;

            for (int i = minPoint.x; i <= maxPoint.x; i++)
                for (int j = minPoint.y; j <= maxPoint.y; j++)
                    sum += gameBoard[i, j];

            return sum == 10;
        }
    }
}
