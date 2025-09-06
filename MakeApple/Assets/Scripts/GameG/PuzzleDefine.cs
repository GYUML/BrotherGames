using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameG
{
    public interface IGimmick
    {
        public void OnEnter(BasePuzzleBoard board, BaseTile tile);
        public void OnLeave(BasePuzzleBoard board, BaseTile tile);
    }

    public abstract class BaseGimmick : IGimmick
    {
        public abstract void OnEnter(BasePuzzleBoard board, BaseTile tile);

        public abstract void OnLeave(BasePuzzleBoard board, BaseTile tile);
    }

    public class DisappearGimmick : BaseGimmick
    {
        public override void OnEnter(BasePuzzleBoard board, BaseTile tile) { }

        public override void OnLeave(BasePuzzleBoard board, BaseTile tile)
        {
            tile.IsWalkable = false;
            board.OnGimmickEvent(GimmickEnum.DisappearTile, tile.Pos);
        }
    }

    public class RuneItemGimmick : BaseGimmick
    {
        public override void OnEnter(BasePuzzleBoard board, BaseTile tile)
        {
            board.AcquireRune++;
            board.OnGimmickEvent(GimmickEnum.RuneItem, tile.Pos);
        }

        public override void OnLeave(BasePuzzleBoard board, BaseTile tile) { }
    }

    public enum GimmickEnum
    {
        None,
        RuneItem,
        DisappearTile,
    }

    public enum TileEnum
    {
        None,
        Normal,
        Disappear
    }

    public abstract class BaseTile
    {
        public bool IsWalkable { get; set; }
        public Vector2Int Pos { get; protected set; }

        protected List<IGimmick> gimmicks = new List<IGimmick>();
        protected int wallMask;

        public abstract void OnEnter(BasePuzzleBoard board);
        public abstract void OnLeave(BasePuzzleBoard board);

        public BaseTile(Vector2Int pos, int wallMask)
        {
            Pos = pos;
            this.wallMask = wallMask;
        }

        public void Enter(BasePuzzleBoard board)
        {
            foreach (var gimmick in gimmicks)
                gimmick.OnEnter(board, this);

            OnEnter(board);
        }

        public void Leave(BasePuzzleBoard board)
        {
            foreach (var gimmick in gimmicks)
                gimmick.OnLeave(board, this);

            OnLeave(board);
        }

        public bool HasWall(Direction direction)
        {
            return (wallMask & (int)direction) > 0;
        }
    }

    public class NormalTile : BaseTile
    {
        public NormalTile(Vector2Int pos, int wallMask, bool hasRune) : base(pos, wallMask)
        {
            IsWalkable = true;

            if (hasRune)
                gimmicks.Add(new RuneItemGimmick());
        }

        public override void OnEnter(BasePuzzleBoard board) { }

        public override void OnLeave(BasePuzzleBoard board) { }
    }

    public class EmptyTile : BaseTile
    {
        public EmptyTile(Vector2Int pos, int wallMask) : base(pos, wallMask)
        {
            IsWalkable = false;
        }

        public override void OnEnter(BasePuzzleBoard board) { }

        public override void OnLeave(BasePuzzleBoard board) { }
    }

    public class DisappearTile : NormalTile
    {
        public DisappearTile(Vector2Int pos, int wallMask, bool hasRune) : base(pos, wallMask, hasRune)
        {
            gimmicks.Add(new DisappearGimmick());
        }

        public override void OnEnter(BasePuzzleBoard board) { }

        public override void OnLeave(BasePuzzleBoard board) { }
    }

    public class BasePuzzleBoard
    {
        public Vector2Int StartPos { get; private set; }
        public Vector2Int EndPos { get; private set; }
        public Vector2Int PlayerPos { get; private set; }
        public int AcquireRune { get; set; }

        BaseTile[,] board;

        Dictionary<GimmickEnum, Action<Vector2Int>> gimmickEvent;

        public BasePuzzleBoard(BaseTile[,] board, Vector2Int startPos, Vector2Int endPos)
        {
            this.board = board;
            StartPos = startPos;
            EndPos = endPos;
        }

        public void Initialize()
        {
            if (IsValidPosition(StartPos))
            {
                PlayerPos = StartPos;
                var tile = board[StartPos.x, StartPos.y];
                tile.Enter(this);
            }
        }

        public void MoveDirection(Direction direction)
        {
            var nextPos = PlayerPos + GetVector(direction);

            if (!IsEndGame() && IsMovePossible(direction))
                MovePoint(nextPos);
        }

        public void MovePoint(Vector2Int pos)
        {
            if (IsValidPosition(pos))
            {
                var nextTile = board[pos.x, pos.y];
                var prevTile = board[PlayerPos.x, PlayerPos.y];

                if (nextTile.IsWalkable)
                {
                    prevTile.Leave(this);
                    PlayerPos = pos;
                    nextTile.Enter(this);
                }
            }
        }

        public bool IsValidPosition(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < board.GetLength(0) && pos.y >= 0 && pos.y < board.GetLength(1);
        }

        public bool HasWall(Vector2Int pos, Direction direction)
        {
            return IsValidPosition(pos) && board[pos.x, pos.y].HasWall(direction);
        }

        public bool IsMovePossible(Direction direction)
        {
            var nextPos = PlayerPos + GetVector(direction);

            if (IsValidPosition(nextPos))
            {
                var prevTile = board[PlayerPos.x, PlayerPos.y];
                var nextTile = board[nextPos.x, nextPos.y];

                return !prevTile.HasWall(direction) && nextTile.IsWalkable;
            }

            return false;
        }

        public bool IsEndGame()
        {
            if (PlayerPos == EndPos)
                return true;
            else if (!IsMovePossible(Direction.Up)
                && !IsMovePossible(Direction.Down)
                && !IsMovePossible(Direction.Left)
                && !IsMovePossible(Direction.Right))
                return true;

            return false;
        }

        public bool IsSuccessGame()
        {
            return true;
        }

        public int GetLength(int dimension)
        {
            return board.GetLength(dimension);
        }

        public Vector2Int GetVector(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:return Vector2Int.up;
                case Direction.Down:return Vector2Int.down;
                case Direction.Left:return Vector2Int.left;
                case Direction.Right:return Vector2Int.right;
                default: return Vector2Int.zero;
            }
        }

        public Direction GetDirection(Vector2Int from, Vector2Int to)
        {
            var vector = to - from;

            foreach (var value in Enum.GetValues(typeof(Direction)))
            {
                var direction = (Direction)value;
                if (GetVector(direction) == vector)
                    return direction;
            }
            
            return Direction.None;
        }

        public void SetGimmickEvent(Dictionary<GimmickEnum, Action<Vector2Int>> gimmickEvent)
        {
            this.gimmickEvent = gimmickEvent;
        }

        public void OnGimmickEvent(GimmickEnum gimmick, Vector2Int pos)
        {
            if (gimmickEvent != null && gimmickEvent.ContainsKey(gimmick))
                gimmickEvent[gimmick]?.Invoke(pos);
        }
    }
}
