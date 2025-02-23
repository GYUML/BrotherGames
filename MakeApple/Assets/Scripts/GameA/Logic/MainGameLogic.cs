using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GameALogic
{
    public class MainGameLogic : MonoBehaviour
    {
        Action<int[,]> onUpdateGameBoard;
        Action<int> onUpdateScore;
        Action<int> onUpdateRemainCount;
        Action<float, float> onSetTimer;
        Action<int> onTimeOver;
        Action<int, int> onAcquire;

        int rowSize;
        int columnSize;
        float maxRemainSeconds;

        int[,] gameBoard;
        int score = 0;
        float remainSeconds;
        int nowStage;

        private void Update()
        {
            if (remainSeconds > 0)
            {
                remainSeconds -= Time.deltaTime;
                if (remainSeconds <= 0)
                {
                    onTimeOver?.Invoke(score);
                }
            }
        }

        public void SetGameBoardCallBack(Action<int[,]> onUpdateGameBoard, Action<int> onUpdateScore, Action<int> onUpdateRemainCount, Action<float, float> onSetTimer, Action<int> onTimeOver, Action<int, int> onAcquire)
        {
            this.onUpdateGameBoard = onUpdateGameBoard;
            this.onUpdateScore = onUpdateScore;
            this.onUpdateRemainCount = onUpdateRemainCount;
            this.onSetTimer = onSetTimer;
            this.onTimeOver = onTimeOver;
            this.onAcquire = onAcquire;
        }

        public void StartGame(int rowSize, int columnSize)
        {
            this.rowSize = rowSize;
            this.columnSize = columnSize;
            nowStage = 1;

            ResetScore();
            GenerateGameBoard(rowSize, columnSize);
            SetRemainTime(100f, 100f);
        }

        void NextStage()
        {
            nowStage++;
            GenerateGameBoard(rowSize, columnSize);
            SetRemainTime(100f, Mathf.Min(maxRemainSeconds, remainSeconds + 50f));
        }

        void GenerateGameBoard(int rowSize, int columnSize)
        {
            gameBoard = new int[rowSize, columnSize];

            for (int i = 0; i < rowSize; i++)
                for (int j = 0; j < columnSize; j++)
                    gameBoard[i, j] = UnityEngine.Random.Range(1, 10);

            onUpdateGameBoard?.Invoke(gameBoard);
            onUpdateRemainCount?.Invoke(GetPossibleCase());
        }

        public void DragEnd(Vector2Int startPoint, Vector2Int endPoint)
        {
            if (IsSuccess(startPoint, endPoint))
            {
                var minPoint = new Vector2Int(Mathf.Min(startPoint.x, endPoint.x), Mathf.Min(startPoint.y, endPoint.y));
                var maxPoint = new Vector2Int(Mathf.Max(startPoint.x, endPoint.x), Mathf.Max(startPoint.y, endPoint.y));

                for (int i = minPoint.x; i <= maxPoint.x; i++)
                    for (int j = minPoint.y; j <= maxPoint.y; j++)
                        if (gameBoard[i, j] != 0)
                        {
                            gameBoard[i, j] = 0;
                            AddScore(1);
                            onAcquire?.Invoke(i, j);
                        }

                onUpdateGameBoard?.Invoke(gameBoard);
                onUpdateRemainCount?.Invoke(GetPossibleCase());
                Debug.Log($"Possible : {GetPossibleCase()}");

                if (GetPossibleCase() <= 0)
                    NextStage();
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
            for (int P0_row = 0; P0_row < rowMax; P0_row++)
            {
                for (int P0_col = 0; P0_col < colMax; P0_col++)
                {

                    int[,] arr = new int[rowMax - P0_row, colMax - P0_col];

                    /*끝점이 되는 좌표*/
                    for (int P1_row = P0_row; P1_row < rowMax; P1_row++)
                    {
                        for (int P1_col = P0_col; P1_col < colMax; P1_col++)
                        {
                            int x = P1_row - P0_row, y = P1_col - P0_col;   //임의 영역의 좌표
                            if (x == 0) arr[0, y] = gameBoard[P1_row, P1_col] + (P1_col == P0_col ? 0 : arr[0, y - 1]);   //영역의 첫째줄은 누적합으로 배열에 저장
                            else
                            {
                                if (y == 0) arr[x, y] = gameBoard[P1_row, P1_col] + arr[x - 1, y];
                                else arr[x, y] = gameBoard[P1_row, P1_col] + arr[x - 1, y] + arr[x, y - 1] - arr[x - 1, y - 1];
                            }
                            if (arr[x, y] == 10)
                            {

                                //첫번째행, 첫번째열, 마지막행, 마지막열 중 하나라도 공백인경우 중복 영역이 되므로 제외
                                //오른쪽이 공백일 경우는 이 코드에서 존재하지 않음
                                if (!(arr[0, y] == 0 || arr[x, 0] == 0 || (y > 0 && arr[x, y] - arr[x, y - 1] == 0) || (x > 0 && arr[x, y] - arr[x - 1, y] == 0)))
                                {
                                    res++;
                                }

                                if (y == 0) P1_row = rowMax;
                                break;
                            }
                            else if (arr[x, y] > 10)
                            {
                                if (y == 0) P1_row = rowMax;
                                break;
                            }
                        }
                    }


                }
            }

            return res;
        }

        void AddScore(int add)
        {
            score += add;
            onUpdateScore?.Invoke(score);
        }

        void ResetScore()
        {
            score = 0;
            onUpdateScore?.Invoke(score);
        }

        void SetRemainTime(float maxTime, float nowTime)
        {
            maxRemainSeconds = maxTime;
            remainSeconds = nowTime;
            onSetTimer?.Invoke(maxTime, nowTime);
        }
    }
}
