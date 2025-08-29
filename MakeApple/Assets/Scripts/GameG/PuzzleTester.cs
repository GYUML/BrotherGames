using System;
using TMPro;
using UnityEngine;

namespace GameG
{
    public class PuzzleTester : MonoBehaviour
    {
        public TMP_Text log;

        TilePuzzle puzzle = new TilePuzzle();
        int[,] testPuzzle = new int[,]
        {
            { 1, 1, 1},
            { 1, 1, 1},
            { 1, 1, 1},
        };
        Direction[] directions = new Direction[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

        private void Start()
        {
            //puzzle.Init(testPuzzle, new Vector2Int(0, 0), new Vector2Int(2, 2));
            //Debug.Log(GetAnswerCount(puzzle));
        }

        public int GetAnswerCount(TilePuzzle puzzle)
        {
            var answerCount = 0;
            DFS(puzzle, ref answerCount);
            return answerCount;
        }

        void DFS(TilePuzzle puzzle, ref int answerCount)
        {
            if (puzzle.IsEndGame())
            {
                if (puzzle.IsSuccessGame())
                    answerCount++;
                return;
            }

            foreach (var direction in directions)
            {
                if (puzzle.IsMovePossible(direction))
                {
                    puzzle.Move(direction);
                    DFS(puzzle, ref answerCount);
                    puzzle.UndoMove();
                }
            }
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.UpArrow))
            //{
            //    puzzle.Move(Direction.Up);
            //    Print(puzzle.GetBoardState());
            //}
            //else if (Input.GetKeyDown(KeyCode.DownArrow))
            //{
            //    puzzle.Move(Direction.Down);
            //    Print(puzzle.GetBoardState());
            //}
            //else if (Input.GetKeyDown(KeyCode.LeftArrow))
            //{
            //    puzzle.Move(Direction.Left);
            //    Print(puzzle.GetBoardState());
            //}
            //else if (Input.GetKeyDown(KeyCode.RightArrow))
            //{
            //    puzzle.Move(Direction.Right);
            //    Print(puzzle.GetBoardState());
            //}
            //else if (Input.GetKeyDown(KeyCode.Escape))
            //{
            //    puzzle.UndoMove();
            //    Print(puzzle.GetBoardState());
            //}
        }

        void Print(int[,] board)
        {
            var print = "";
            var nowPos = puzzle.GetNowPosition();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                print += Environment.NewLine;
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (nowPos.x == i && nowPos.y == j)
                        print += "* ";
                    else
                        print += board[i, j] + " ";
                }
            }
            print += $"{Environment.NewLine}";

            if (puzzle.IsEndGame())
                print += $"{Environment.NewLine}GameEnd. success={puzzle.IsSuccessGame()}";

            log.text = print;
        }
    }
}
