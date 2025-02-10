using System;
using System.Collections;
using System.Collections.Generic;
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
            Debug.Log(GetPossibleCase());
        }

        bool IsSuccess(Vector2Int startPoint, Vector2Int endPoint)
        {
            var minPoint = new Vector2Int(Mathf.Min(startPoint.x, endPoint.x), Mathf.Min(startPoint.y, endPoint.y));
            var maxPoint = new Vector2Int(Mathf.Max(startPoint.x, endPoint.x), Mathf.Max(startPoint.y, endPoint.y));
            var sum = 0;

            for (int i = minPoint.x; i <= maxPoint.x; i++)
                for (int j = minPoint.y; j <= maxPoint.y; j++)
                    sum += gameBoard[i, j];

            //for (int i = minPoint.x; i <= maxPoint.x; i++)
            //    for (int j = minPoint.y; j <= maxPoint.y; j++)
            //        Debug.Log(gameBoard[i, j]);

            return sum == 10;
        }


        public int GetPossibleCase()
        {
            int res = 0;
            int rowMax = gameBoard.GetLength(0), colMax = gameBoard.GetLength(1);

            /*시작점이 되는 좌표*/
            for (int i = 0; i < rowMax; i++)
            {
                for(int j = 0; j < colMax; j++)
                {

                    int[] arr = new int[colMax - j];

                    /*끝점이 되는 좌표*/
                    for (int k = i; k < rowMax; k++)
                    {
                        for (int l = j; l < colMax; l++)
                        {
                            if (k == i) arr[l - j] = gameBoard[k, l] + (l == j ? 0 : arr[l - j - 1]);   //영역의 첫째줄은 누적합으로 배열에 저장
                            else
                            {
                                //if (l - j + 1 < arr.Length) arr[l - j + 1] -= arr[l - j];   //중첩된 부분 제거

                                if (l == j) arr[l - j] += gameBoard[k, l];
                                else arr[l - j] = arr[l - j - 1] + arr[l - j] + gameBoard[k, l];
                            }

                            if (arr[l - j] == 10)
                            {
                                res++;
                                //k = rowMax;     //이중 for문 탈출
                                break;          //이중 for문 탈출
                            }
                            else if (arr[l - j] > 10)
                            {
                                if (l == j) k = rowMax;
                                break;
                            }
                        }
                        for (int l = colMax - j - 1; l > 0; l--)
                        {
                            arr[l] -= arr[l - 1];
                        }
                    }


                }
            }

            return res;
        }
    }
}
