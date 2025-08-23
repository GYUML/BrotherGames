
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameG
{
    public enum TileType
    {
        None = 0,
        Platform = 1
    }

    public enum Direction
    {
        None = 0,
        Up= 1,
        Down= 2,
        Left= 4,
        Right= 8,
    }

    public class TilePuzzle
    {
        PuzzleData puzzle;

        int remainTileCount;
        Vector2Int nowPosition;
        Stack<Vector2Int> positionLog = new Stack<Vector2Int>();

        public void Init(PuzzleData puzzle)
        {
            this.puzzle = puzzle.DeepCopy();
            Reset();
        }

        public void Reset()
        {
            remainTileCount = GetRemainTileCount(puzzle.board);
            nowPosition = puzzle.startPosition;
            positionLog.Clear();
        }

        public void Move(Direction direction)
        {
            var nextPosition = GetNextPosition(direction);

            if (!IsEndGame() && IsMovePossibleTile(nextPosition))
                Move(nextPosition);
        }

        public void UndoMove()
        {
            if (positionLog.Count > 0)
            {
                var pos = positionLog.Pop();
                remainTileCount++;
                puzzle.board[pos.x, pos.y] = 1;
                nowPosition = pos;
            }
        }

        public bool IsMovePossible(Direction direction)
        {
            return direction != Direction.None && IsMovePossibleTile(GetNextPosition(direction)) && !HasWall(GetNowPosition(), direction);
        }

        public bool IsEndGame()
        {
            bool isEndGame = false;

            if (nowPosition == puzzle.endPosition)
            {
                isEndGame = true;
            }
            else
            {
                if (!IsMovePossible(Direction.Up)
                && !IsMovePossible(Direction.Down)
                && !IsMovePossible(Direction.Left)
                && !IsMovePossible(Direction.Right))
                    isEndGame = true;
            }

            return isEndGame;
        }

        public bool IsSuccessGame()
        {
            return remainTileCount == 1 && nowPosition == puzzle.endPosition;
        }

        public int[,] GetBoardState()
        {
            return puzzle.board;
        }

        public int[,] GetWallMaskBoard()
        {
            return puzzle.wallMaskBoard;
        }

        public Stack<Vector2Int> GetPositionLog()
        {
            return positionLog;
        }

        public Vector2Int GetNowPosition()
        {
            return nowPosition;
        }

        public int GetRemainCount()
        {
            return remainTileCount;
        }

        public bool HasWall(Vector2Int position, Direction direction)
        {
            if (position.x < 0 || position.x >= puzzle.wallMaskBoard.GetLength(0) || position.y < 0 || position.y >= puzzle.wallMaskBoard.GetLength(1))
                return false;

            var result = puzzle.wallMaskBoard[position.x, position.y] & (int)direction;
            return result > 0;
        }

        public Vector2Int GetEndPosition()
        {
            return puzzle.endPosition;
        }

        void Move(Vector2Int to)
        {
            positionLog.Push(nowPosition);

            puzzle.board[nowPosition.x, nowPosition.y] = 0;
            remainTileCount--;
            nowPosition = to;
        }

        int GetRemainTileCount(int[,] board)
        {
            var count = 0;

            foreach (var tile in board)
            {
                if (tile == 1)
                    count++;
            }

            return count;
        }

        Vector2Int GetNextPosition(Direction direction)
        {
            var position = nowPosition;

            if (direction == Direction.Up)
                position = nowPosition + Vector2Int.up;
            else if (direction == Direction.Down)
                position = nowPosition + Vector2Int.down;
            else if (direction == Direction.Left)
                position = nowPosition + Vector2Int.left;
            else if (direction == Direction.Right)
                position = nowPosition + Vector2Int.right;

            return position;
        }

        bool IsMovePossibleTile(Vector2Int to)
        {
            if (to.x < 0 || to.x >= puzzle.board.GetLength(0) || to.y < 0 || to.y >= puzzle.board.GetLength(1))
            {
                return false;
            }
            else if (puzzle.board[to.x, to.y] != 1)
            {
                return false;
            }

            return true;
        }
    }

    public class PuzzleUtil
    {
        public static Direction GetDirection(Vector2Int from, Vector2Int to)
        {
            var vector = to - from;

            if (vector == Vector2Int.up)
                return Direction.Up;
            else if (vector == Vector2Int.down)
                return Direction.Down;
            else if (vector == Vector2Int.left)
                return Direction.Left;
            else if (vector == Vector2Int.right)
                return Direction.Right;

            return Direction.None;
        }
    }

    public class PuzzleData
    {
        public int[,] board;
        public int[,] wallMaskBoard;

        public Vector2Int startPosition;
        public Vector2Int endPosition;
    }
}
