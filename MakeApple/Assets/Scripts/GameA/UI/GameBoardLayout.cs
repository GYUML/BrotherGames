using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameAUI
{
    public class GameBoardLayout : MonoBehaviour
    {
        public GameObject itemPrefab;
        public Button startButton;

        List<GameObject> itemPool = new List<GameObject>();

        int rowSize;
        int columnSize;
        Vector2Int startPoint;

        private void Start()
        {
            itemPrefab.SetActive(false);
        }

        public void SetBoard(int rowSize, int columnSize, int[,] gameBoard)
        {
            this.rowSize = rowSize;
            this.columnSize = columnSize;

            foreach (var item in itemPool)
                item.SetActive(false);

            for (int i = itemPool.Count; i < rowSize * columnSize; i++)
            {
                var instantiated = Instantiate(itemPrefab, itemPrefab.transform.parent);
                itemPool.Add(instantiated);
                instantiated.SetActive(false);
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
                    item.SetActive(true);
                    item.transform.Find("Text").GetComponent<TMP_Text>().text = gameBoard[i, j].ToString();
                    item.GetComponent<Button>().onClick.RemoveAllListeners();
                    item.GetComponent<Button>().onClick.AddListener(() => OnSelectItem(rowIndex, columnIndex));
                }
            }
        }

        public void RemoveItemInBoard(int row, int column)
        {
            var item = itemPool[row * columnSize + column];
            item.transform.Find("Text").GetComponent<TMP_Text>().text = string.Empty;
        }

        void OnSelectItem(int row, int column)
        {
            if (startPoint.x > -1 && startPoint.y > -1)
            {
                // TODO
                // DragEnd(startPoint, new Vector2Int(row, column));
                startPoint = new Vector2Int(-1, -1);
            }
            else
            {
                startPoint = new Vector2Int(row, column);
            }
        }
    }
}
