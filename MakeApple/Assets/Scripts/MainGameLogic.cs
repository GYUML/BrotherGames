using UnityEngine;

namespace GameALogic
{
    public class GameAMainLogic : MonoBehaviour
    {
        public int rowSize;
        public int columnSize;

        int[,] gameBoard;

        public void GenerateGameBoard(int rowSize, int columnSize)
        {
            gameBoard = new int[rowSize, columnSize];

            for (int i = 0; i < rowSize; i++)
            {
                for (int j = 0; j < columnSize; j++)
                {
                    gameBoard[i, j] = Random.Range(1, 10);
                }
            }

            // TODO
            // UpdateUI
            // ShowBoard();
        }

        void DragEnd(Vector2Int startPoint, Vector2Int endPoint)
        {
            if (IsSuccess(startPoint, endPoint))
            {
                var minPoint = new Vector2Int(Mathf.Min(startPoint.x, endPoint.x), Mathf.Min(startPoint.y, endPoint.y));
                var maxPoint = new Vector2Int(Mathf.Max(startPoint.x, endPoint.x), Mathf.Max(startPoint.y, endPoint.y));

                for (int i = minPoint.x; i <= maxPoint.x; i++)
                    for (int j = minPoint.y; j <= maxPoint.y; j++)
                        gameBoard[i, j] = 0;

                // TODO
                // Update UI
                //for (int i = minPoint.x; i <= maxPoint.x; i++)
                //    for (int j = minPoint.y; j <= maxPoint.y; j++)
                //        RemoveItemInBoard(i, j);
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
