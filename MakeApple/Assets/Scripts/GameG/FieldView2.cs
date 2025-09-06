using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameG
{
    public class FieldView2 : MonoBehaviour
    {
        public GameProcedures procedures;
        public PuzzleInput2 input;

        // GameObject setting
        public GameObject tileBlockPrefab;
        public GameObject selectBoxPrefab;
        public GameObject wallVerticalPrefab;
        public GameObject wallHorizontalPrefab;
        public GameObject flagPrefab;
        public GameObject player;
        public GameObject runePrefab;

        public Vector2 tileGap;
        public Vector2 tilePivot;

        public float moveSpeed;
        public float moveError;

        // GameObject state
        int idCounter;
        Dictionary<Vector2Int, GameObject> tileMap = new Dictionary<Vector2Int, GameObject>();
        Dictionary<Vector2Int, GameObject> itemMap = new Dictionary<Vector2Int, GameObject>();

        int[,] testBoard = new int[,]
        {
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 2, 2, 1 },
            { 1, 1, 1 },
        };
        int[,] testWall = new int[,]
        {
            { 0, 8, 0 },
            { 0, 4, 0 },
            { 0, 0, 0 },
            { 0, 1, 2 },
        };
        int[,] itemBoard = new int[,]
        {
            { 0, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 0 },
        };

        BasePuzzleBoard board;

        private void Awake()
        {
            tileBlockPrefab.gameObject.SetActive(false);

            board = LoadBoardData();
            SpawnField(board);
            board.Initialize();
            board.SetGimmickEvent(GetGimmickEvent());

            input.Initialize(LoadBoardData());
        }

        BasePuzzleBoard LoadBoardData()
        {
            BaseTile[,] tiles = new BaseTile[4, 3];

            for (int i = 0; i < testBoard.GetLength(0); i++)
            {
                for (int j = 0; j < testBoard.GetLength(1); j++)
                {
                    BaseTile newTile;
                    var tileEnum = (TileEnum)testBoard[i, j];
                    var hasRune = itemBoard[i, j] == 1;

                    if (tileEnum == TileEnum.Normal)
                        newTile = new NormalTile(new Vector2Int(i, j), testWall[i, j], hasRune);
                    else if (testBoard[i, j] == (int)TileEnum.Disappear)
                        newTile = new DisappearTile(new Vector2Int(i, j), testWall[i, j], hasRune);
                    else
                        newTile = new EmptyTile(new Vector2Int(i, j), testWall[i, j]);

                    tiles[i, j] = newTile;
                }
            }

            return new BasePuzzleBoard(tiles, new Vector2Int(0, 0), new Vector2Int(3, 2));
        }

        Dictionary<GimmickEnum, Action<Vector2Int>> GetGimmickEvent()
        {
            var events = new Dictionary<GimmickEnum, Action<Vector2Int>>();

            events.Add(GimmickEnum.RuneItem, (pos) => procedures.AddProcedure(AcquireItemCo(pos)));
            events.Add(GimmickEnum.DisappearTile, (pos) => procedures.AddProcedure(DropTileCo(pos)));
            return events;
        }

        public void SpawnField(BasePuzzleBoard board)
        {
            // Spawn Tile
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    var instantiated = Instantiate(tileBlockPrefab, tileBlockPrefab.transform.parent);
                    instantiated.transform.position = new Vector2(j * tileGap.x, -i * tileGap.y) + tilePivot;
                    instantiated.gameObject.SetActive(true);
                    instantiated.GetComponent<TileEventListner>().Id = idCounter;

                    tileMap.Add(new Vector2Int(i, j), instantiated);
                    idCounter++;
                }
            }

            // Spawn Wall
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    var pos = new Vector2Int(i, j);

                    if (board.IsValidPosition(pos + Vector2Int.up) && board.HasWall(pos, Direction.Up) && board.HasWall(pos + Vector2Int.up, Direction.Down))
                    {
                        var wallPos = Vector3.Lerp(tileMap[pos].transform.position, tileMap[pos + Vector2Int.up].transform.position, 0.5f);
                        var wall = Instantiate(wallVerticalPrefab);
                        wall.transform.position = wallPos;
                    }
                    if (board.IsValidPosition(pos + Vector2Int.right) && board.HasWall(pos, Direction.Right) && board.HasWall(pos + Vector2Int.right, Direction.Left))
                    {
                        var wallPos = Vector3.Lerp(tileMap[pos].transform.position, tileMap[pos + Vector2Int.right].transform.position, 0.5f);
                        var wall = Instantiate(wallHorizontalPrefab);
                        wall.transform.position = wallPos;
                    }
                }
            }

            SpawnFieldItem(board);

            var flag = Instantiate(flagPrefab);
            flag.transform.position = tileMap[board.EndPos].transform.position;
            player.transform.position = tileMap[board.PlayerPos].transform.position;
        }

        void SpawnFieldItem(BasePuzzleBoard board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (itemBoard[i, j] == 1)
                    {
                        var pos = new Vector2Int(i, j);
                        var rune = Instantiate(runePrefab);
                        rune.transform.position = tileMap[pos].transform.position;
                        itemMap.Add(pos, rune);
                    }
                }
            }
        }

        public void SubmitMove(List<Vector2Int> moveList)
        {
            for (int i = 0; i < moveList.Count - 1; i++)
            {
                var from = moveList[i];
                var to = moveList[i + 1];
                var dir = PuzzleUtil.GetDirection(from, to);

                MovePlayer(from, to);
                board.MoveDirection(dir);
            }

            if (board.IsEndGame())
                Debug.Log($"End Game. {board.IsSuccessGame()}");

            Debug.Log(board.PlayerPos);
        }

        public void UndoMove()
        {

        }

        void MovePlayer(Vector2Int prevPos, Vector2Int nowPos)
        {
            procedures.AddProcedure(MovePlayerCo(prevPos, nowPos));
            //if (puzzle.GetItemType(nowPos) == 1)
            //    procedures.AddProcedure(AcquireItemCo(nowPos));
            //if (puzzle.GetTileType(prevPos) == TileType.DropPlatform)
            //    procedures.AddProcedure(DropTileCo(prevPos));
        }

        IEnumerator MovePlayerCo(Vector2Int prev, Vector2Int now)
        {
            var prevTile = tileMap[prev];
            var nowTile = tileMap[now];

            yield return MovePlayerAniCo(nowTile.transform.position);

            //prevTile.GetComponent<TileEventListner>().DoDrop();
        }

        IEnumerator MovePlayerAniCo(Vector3 position)
        {
            while (Vector3.SqrMagnitude(player.transform.position - position) > moveError)
            {
                yield return null;
                player.transform.position = Vector3.MoveTowards(player.transform.position, position, Time.deltaTime * moveSpeed);
            }

            player.transform.position = position;
        }

        IEnumerator AcquireItemCo(Vector2Int pos)
        {
            if (itemMap.ContainsKey(pos))
            {
                itemMap[pos].gameObject.SetActive(false);
            }

            yield break;
        }

        IEnumerator DropTileCo(Vector2Int pos)
        {
            var tile = tileMap[pos];
            tile.GetComponent<TileEventListner>().DoDrop();

            yield break;
        }

        public Vector2Int GetTilePosition(int id)
        {
            foreach (var tile in tileMap)
            {
                if (tile.Value.GetComponent<TileEventListner>().Id == id)
                    return tile.Key;
            }

            return Vector2Int.zero;
        }
    }
}

