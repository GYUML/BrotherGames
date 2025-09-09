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

    public class AcquireItemGimmick : BaseGimmick
    {
        public override void OnEnter(BasePuzzleBoard board, BaseTile tile)
        {
            board.AcquireRune++;
            board.OnGimmickEvent(GimmickEnum.AcquireItem, tile.Pos);
        }

        public override void OnLeave(BasePuzzleBoard board, BaseTile tile) { }
    }

    public class TeleportGimmick : BaseGimmick
    {
        Vector2Int teleportPos;

        public TeleportGimmick(Vector2Int teleportPos)
        {
            this.teleportPos = teleportPos;
        }

        public override void OnEnter(BasePuzzleBoard board, BaseTile tile)
        {
            board.MovePoint(teleportPos);
            board.OnGimmickEvent(GimmickEnum.Teleport, teleportPos);
        }

        public override void OnLeave(BasePuzzleBoard board, BaseTile tile) { }
    }

    public enum GimmickEnum
    {
        None,
        AcquireItem,
        DisappearTile,
        Teleport,
    }

    public enum TileEnum
    {
        None,
        Normal,
        Disappear,
        Portal
    }

    public abstract class BaseTile
    {
        public bool IsWalkable { get; set; }
        public Vector2Int Pos { get; protected set; }
        public int Item;

        protected List<IGimmick> gimmicks = new List<IGimmick>();
        protected int wallMask;
        
        public abstract void OnEnter(BasePuzzleBoard board);
        public abstract void OnLeave(BasePuzzleBoard board);

        public BaseTile(Vector2Int pos, int wallMask, int item = 0)
        {
            Pos = pos;
            this.wallMask = wallMask;
            Item = item;

            if (item != 0)
                gimmicks.Add(new AcquireItemGimmick());
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
        public NormalTile(Vector2Int pos, int wallMask, int item) : base(pos, wallMask, item)
        {
            IsWalkable = true;
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
        public DisappearTile(Vector2Int pos, int wallMask, int item) : base(pos, wallMask, item)
        {
            gimmicks.Add(new DisappearGimmick());
        }

        public override void OnEnter(BasePuzzleBoard board) { }

        public override void OnLeave(BasePuzzleBoard board) { }
    }

    public class PortalTile : BaseTile
    {
        public PortalTile(Vector2Int pos, int wallMask, Vector2Int teleportPos, int item) : base(pos, wallMask, item)
        {
            IsWalkable = true;
            gimmicks.Add(new TeleportGimmick(teleportPos));
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

        public bool HasItem(Vector2Int pos)
        {
            return board[pos.x, pos.y].Item != 0;
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

    public class TileData
    {
        public TileEnum tileEnum;
        public int wallMask;
        public int item;

        public TileData(TileEnum tileEnum, int wallMask, int item)
        {
            this.tileEnum = tileEnum;
            this.wallMask = wallMask;
            this.item = item;
        }

        public bool HasWall(Direction direction)
        {
            return (wallMask & (int)direction) > 0;
        }
    }

    public class BoardData
    {
        public int code;
        public TileData[,] tiles;
        public Vector2Int startPos;
        public Vector2Int endPos;

        public BoardData(int sizeX, int sizeY)
        {
            tiles = new TileData[sizeX, sizeY];
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j] = new TileData(TileEnum.None, 0, 0);
                }
            }
        }

        public int GetLength(int dimension)
        {
            if (tiles == null)
                return 0;

            return tiles.GetLength(dimension);
        }

        public TileData GetTileData(int x, int y)
        {
            if (tiles == null)
                return null;

            return tiles[x, y];
        }
    }

    public class BoardDataMaker
    {
        public static void CreateBoard(BoardData boardData, int sizeX, int sizeY)
        {
            boardData.tiles = new TileData[sizeX, sizeY];

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    boardData.tiles[i, j] = new TileData(TileEnum.None, 0, 0);
                }
            }
        }

        public static void SetStartPos(BoardData boardData, int x, int y)
        {
            boardData.startPos = new Vector2Int(x, y);
        }

        public static void SetEndPos(BoardData boardData, int x, int y)
        {
            boardData.endPos = new Vector2Int(x, y);
        }

        public static void SetTile(BoardData boardData, int x, int y, TileEnum tileEnum)
        {
            boardData.tiles[x, y].tileEnum = tileEnum;
        }

        public static void AddTileWall(BoardData boardData, int x, int y, Direction dir)
        {
            boardData.tiles[x, y].wallMask |= (int)dir;
        }

        public static void RemoveTileWall(BoardData boardData, int x, int y, Direction dir)
        {
            var reverseDir = ~dir;
            boardData.tiles[x, y].wallMask &= (int)reverseDir;
        }

        public static void SetItem(BoardData boardData, int x, int y, int itemCode)
        {
            boardData.tiles[x, y].item = itemCode;
        }
    }
}
