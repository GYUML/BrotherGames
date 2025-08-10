
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
        Left= 3,
        Right= 4,
    }

    public class TilePuzzle
    {
        int[,] board;

        Vector2Int startPosition;
        Vector2Int endPosition;

        int remainTileCount;
        Vector2Int nowPosition;
        Stack<Vector2Int> positionLog = new Stack<Vector2Int>();

        public void Init(int[,] board, Vector2Int startPosition, Vector2Int endPosition)
        {
            this.board = board.DeepCopy();
            this.startPosition = startPosition;
            this.endPosition = endPosition;

            Reset();
        }

        public void Reset()
        {
            remainTileCount = GetRemainTileCount(board);
            nowPosition = startPosition;
            positionLog.Clear();
        }

        public void Move(Direction direction)
        {
            var nextPosition = GetNextPosition(direction);

            if (!IsEndGame() && IsMovePossible(nextPosition))
                Move(nextPosition);
        }

        public void RollBack()
        {
            if (positionLog.Count > 0)
            {
                var pos = positionLog.Pop();
                remainTileCount++;
                board[pos.x, pos.y] = 1;
                nowPosition = pos;
            }
        }

        public bool IsMovePossible(Vector2Int to)
        {
            if (to.x < 0 || to.x >= board.GetLength(0) || to.y < 0 || to.y >= board.GetLength(1))
            {
                return false;
            }
            else if (board[to.x, to.y] != 1)
            {
                return false;
            }

            return true;
        }

        public bool IsEndGame()
        {
            bool isEndGame = false;

            if (nowPosition == endPosition)
            {
                isEndGame = true;
            }
            else
            {
                if (!IsMovePossible(GetNextPosition(Direction.Up))
                && !IsMovePossible(GetNextPosition(Direction.Down))
                && !IsMovePossible(GetNextPosition(Direction.Left))
                && !IsMovePossible(GetNextPosition(Direction.Right)))
                    isEndGame = true;
            }

            return isEndGame;
        }

        public bool IsSuccessGame()
        {
            return remainTileCount == 1 && nowPosition == endPosition;
        }

        public int[,] GetBoardState()
        {
            return board;
        }

        public Vector2Int GetNowPosition()
        {
            return nowPosition;
        }

        public int GetRemainCount()
        {
            return remainTileCount;
        }

        int Move(Vector2Int to)
        {
            positionLog.Push(nowPosition);

            board[nowPosition.x, nowPosition.y] = 0;
            remainTileCount--;
            nowPosition = to;

            return 0;
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
                position = nowPosition + new Vector2Int(-1, 0);
            else if (direction == Direction.Down)
                position = nowPosition + new Vector2Int(1, 0);
            else if (direction == Direction.Left)
                position = nowPosition + new Vector2Int(0, -1);
            else if (direction == Direction.Right)
                position = nowPosition + new Vector2Int(0, 1);

            return position;
        }
    }
}
