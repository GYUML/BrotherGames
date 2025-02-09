using GameA;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameAUI
{
    public class GameBoardLayout : UILayout
    {
        public GameBoardItem itemPrefab;
        public Button startButton;

        List<GameBoardItem> itemPool = new List<GameBoardItem>();

        int rowSize;
        int columnSize;
        Vector2Int startPoint;
        Vector2Int endPoint;

        private void Start()
        {
            itemPrefab.gameObject.SetActive(false);
        }

        public void SetBoard(int[,] gameBoard)
        {
            this.rowSize = gameBoard.GetLength(0);
            this.columnSize = gameBoard.GetLength(1);

            foreach (var item in itemPool)
                item.gameObject.SetActive(false);

            for (int i = itemPool.Count; i < rowSize * columnSize; i++)
            {
                var instantiated = Instantiate(itemPrefab, itemPrefab.transform.parent);
                itemPool.Add(instantiated);
                instantiated.gameObject.SetActive(false);
            }

            var boardCounter = 0;
            startPoint = new Vector2Int(-1, -1);

            for (int i = 0; i < rowSize; i++)
            {
                for (int j = 0; j < columnSize; j++)
                {
                    var item = itemPool[boardCounter++];
                    var rowIndex = i;
                    var columnIndex = j;
                    item.gameObject.SetActive(true);
                    item.SetNumber(gameBoard[i, j]);
                    item.SetEvent(() => OnItemPointerDown(rowIndex, columnIndex), () => OnItemPointerUp(rowIndex, columnIndex), () => OnItemPointerEnter(rowIndex, columnIndex));
                }
            }
        }

        public void RemoveItem(int row, int column)
        {
            var item = itemPool[row * columnSize + column];
            item.SetNumber(0);
        }

        void SelectItem(int row, int column, bool select)
        {
            var item = itemPool[row * columnSize + column];
            item.Selected(select);
        }

        void SelectItems(Vector2Int start, Vector2Int end)
        {
            UnSelectAllItems();

            var minPoint = new Vector2Int(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y));
            var maxPoint = new Vector2Int(Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y));

            for (int i = minPoint.x; i <= maxPoint.x; i++)
                for (int j = minPoint.y; j <= maxPoint.y; j++)
                    SelectItem(i, j, true);
        }

        void UnSelectAllItems()
        {
            foreach (var item in itemPool)
                item.Selected(false);
        }

        void OnItemPointerDown(int row, int column)
        {
            startPoint = new Vector2Int(row, column);
            SelectItem(row, column, true);
        }

        void OnItemPointerUp(int row, int column)
        {
            if (startPoint.x < 0 || startPoint.y < 0)
            {
                Debug.LogError($"OnItemPointerUp() row={row}, col={column}");
            }
            else
            {
                Managers.MainLogic.DragEnd(startPoint, endPoint);
            }

            startPoint = new Vector2Int(-1, -1);
            UnSelectAllItems();
        }

        void OnItemPointerEnter(int row, int column)
        {
            if (startPoint.x < 0 || startPoint.y < 0)
                return;

            endPoint = new Vector2Int(row, column);
            SelectItems(startPoint, endPoint);
        }
    }
}
