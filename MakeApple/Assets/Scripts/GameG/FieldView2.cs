using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameG
{
    public class FieldView2 : MonoBehaviour
    {
        public static string BoardFolder = "Assets/Resources/GameG/BoardData";

        public GameProcedures procedures;
        public PuzzleInput2 input;

        // GameObject setting
        public GameObject tileEmptyPrefab;
        public GameObject tileBlockPrefab;
        public GameObject selectBoxPrefab;
        public GameObject wallVerticalPrefab;
        public GameObject wallHorizontalPrefab;
        public GameObject flagPrefab;
        public GameObject player;
        public GameObject runePrefab;

        public GameObject clearStageEffect;

        public Vector2 tileGap;
        public Vector2 tilePivot;

        public float moveSpeed;
        public float moveError;

        // GameObject state
        int idCounter;
        Dictionary<Vector2Int, GameObject> tileMap = new Dictionary<Vector2Int, GameObject>();
        Dictionary<Vector2Int, GameObject> itemMap = new Dictionary<Vector2Int, GameObject>();

        List<GameObject> etcMap = new List<GameObject>();

        BasePuzzleBoard board;

        int nowStage;

        private void Awake()
        {
            tileBlockPrefab.gameObject.SetActive(false);

            nowStage = 0;
            procedures.AddProcedure(NextStageCo());
        }

        void LoadMap(int mapCode)
        {
            if (JsonTool.TryLoad<BoardData>(BoardFolder, $"{mapCode}.json", out var boardData))
            {
                board = GetPuzzleBoard(boardData);

                SpawnField(board);
                board.Initialize();
                board.SetGimmickEvent(GetGimmickEvent());

                input.Initialize(GetPuzzleBoard(boardData));
            }
        }

        BasePuzzleBoard GetPuzzleBoard(BoardData boardData)
        {
            if (boardData == null)
                return null;

            var tiles = new BaseTile[boardData.GetLength(0), boardData.GetLength(1)];

            for (int i = 0; i < boardData.GetLength(0); i++)
            {
                for (int j = 0; j < boardData.GetLength(1); j++)
                {
                    var tileData = boardData.GetTileData(i, j);

                    BaseTile newTile;
                    var tileEnum = tileData.tileEnum;
                    var itemCode = tileData.item;
                    var wallMask = tileData.wallMask;
                    var pos = new Vector2Int(i, j);

                    if (tileEnum == TileEnum.Normal)
                        newTile = new NormalTile(pos, wallMask, itemCode);
                    else if (tileEnum == TileEnum.Disappear)
                        newTile = new DisappearTile(pos, wallMask, itemCode);
                    else if (tileEnum == TileEnum.Portal)
                        newTile = new PortalTile(pos, wallMask, new Vector2Int(3, 1), itemCode);
                    else
                        newTile = new EmptyTile(pos, wallMask);

                    tiles[i, j] = newTile;
                }
            }

            return new BasePuzzleBoard(tiles, boardData.startPos, boardData.endPos);
        }

        Dictionary<GimmickEnum, Action<Vector2Int>> GetGimmickEvent()
        {
            var events = new Dictionary<GimmickEnum, Action<Vector2Int>>();

            events.Add(GimmickEnum.AcquireItem, (pos) => procedures.AddProcedure(AcquireItemCo(pos)));
            events.Add(GimmickEnum.DisappearTile, (pos) => procedures.AddProcedure(DropTileCo(pos)));
            events.Add(GimmickEnum.Teleport, (pos) => procedures.AddProcedure(TeleportPlayerCo(pos)));

            return events;
        }

        public void ClearField()
        {
            clearStageEffect.SetActive(false);

            foreach (var item in tileMap)
                Destroy(item.Value);

            foreach (var item in itemMap)
                Destroy(item.Value);

            foreach (var item in etcMap)
                Destroy(item);

            tileMap.Clear();
            itemMap.Clear();
            etcMap.Clear();
        }

        public void SpawnField(BasePuzzleBoard board)
        {
            // Spawn Tile
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    var pos = new Vector2Int(i, j);

                    if (board.GetTileType(pos) == TileEnum.None)
                    {
                        var instantiated = Instantiate(tileEmptyPrefab, tileEmptyPrefab.transform.parent);
                        instantiated.transform.position = new Vector2(j * tileGap.x, -i * tileGap.y) + tilePivot;
                        instantiated.gameObject.SetActive(true);
                        instantiated.GetComponent<TileEventListner>().Id = idCounter;

                        tileMap.Add(pos, instantiated);
                        idCounter++;
                    }
                    else
                    {
                        var instantiated = Instantiate(tileBlockPrefab, tileBlockPrefab.transform.parent);
                        instantiated.transform.position = new Vector2(j * tileGap.x, -i * tileGap.y) + tilePivot;
                        instantiated.gameObject.SetActive(true);
                        instantiated.GetComponent<TileEventListner>().Id = idCounter;

                        tileMap.Add(pos, instantiated);
                        idCounter++;
                    }
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

                        etcMap.Add(wall);
                    }
                    if (board.IsValidPosition(pos + Vector2Int.right) && board.HasWall(pos, Direction.Right) && board.HasWall(pos + Vector2Int.right, Direction.Left))
                    {
                        var wallPos = Vector3.Lerp(tileMap[pos].transform.position, tileMap[pos + Vector2Int.right].transform.position, 0.5f);
                        var wall = Instantiate(wallHorizontalPrefab);
                        wall.transform.position = wallPos;

                        etcMap.Add(wall);
                    }
                }
            }

            SpawnFieldItem(board);

            var flag = Instantiate(flagPrefab);
            flag.transform.position = tileMap[board.EndPos].transform.position;
            player.transform.position = tileMap[board.PlayerPos].transform.position;

            etcMap.Add(flag);
        }

        void SpawnFieldItem(BasePuzzleBoard board)
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board.HasItem(new Vector2Int(i, j)))
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
            {
                Debug.Log($"End Game. {board.IsSuccessGame()}");
                procedures.AddProcedure(ClearStageCo());
                procedures.AddProcedure(NextStageCo());
            }

            Debug.Log(board.PlayerPos);
        }

        public void UndoMove()
        {

        }

        void MovePlayer(Vector2Int prevPos, Vector2Int nowPos)
        {
            procedures.AddProcedure(MovePlayerCo(prevPos, nowPos));
        }

        IEnumerator MovePlayerCo(Vector2Int prev, Vector2Int now)
        {
            var prevTile = tileMap[prev];
            var nowTile = tileMap[now];

            yield return MovePlayerAniCo(nowTile.transform.position);
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

        IEnumerator TeleportPlayerCo(Vector2Int pos)
        {
            yield return new WaitForSeconds(0.3f);

            var tile = tileMap[pos];
            player.transform.position = tile.transform.position;
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

        IEnumerator ClearStageCo()
        {
            clearStageEffect.gameObject.SetActive(true);

            yield return new WaitForSeconds(5f);
        }

        IEnumerator NextStageCo()
        {
            ClearField();

            yield return new WaitForSeconds(1f);

            nowStage++;
            LoadMap(nowStage);
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

