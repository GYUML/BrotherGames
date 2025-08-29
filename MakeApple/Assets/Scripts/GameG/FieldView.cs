using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameG
{
    public class FieldView : MonoBehaviour
    {
        public GameProcedures procedures;
        public PuzzleInput input;

        TilePuzzle puzzle = new TilePuzzle();

        // GameObject setting
        public GameObject tileBlockPrefab;
        public GameObject selectBoxPrefab;
        public GameObject wallVerticalPrefab;
        public GameObject wallHorizontalPrefab;
        public GameObject flagPrefab;
        public GameObject player;

        public Vector2 tileGap;
        public Vector2 tilePivot;

        public float moveSpeed;
        public float moveError;

        // GameObject state
        int idCounter;
        Dictionary<int, GameObject> tileMaps = new Dictionary<int, GameObject>();
        Dictionary<int, Vector2Int> tilePositions = new Dictionary<int, Vector2Int>();

        int[,] testBoard = new int[,]
        {
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 },
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
        PuzzleData puzzleData;

        private void Awake()
        {
            puzzleData = new PuzzleData();
            puzzleData.board = testBoard;
            puzzleData.wallMaskBoard = testWall;
            puzzleData.itemBoard = itemBoard;
            puzzleData.startPosition = new Vector2Int(0, 0);
            puzzleData.endPosition = new Vector2Int(3, 2);
            puzzleData.needRune = 10;

            tileBlockPrefab.gameObject.SetActive(false);
            puzzle.Init(puzzleData);

            SpawnField(puzzle);
            input.SpawnField(puzzleData);
        }

        public void SpawnField(TilePuzzle puzzle)
        {
            var board = puzzle.GetBoardState();
            var wallBoard = puzzle.GetWallMaskBoard();

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    var instantiated = Instantiate(tileBlockPrefab, tileBlockPrefab.transform.parent);
                    instantiated.transform.position = new Vector2(j * tileGap.x, -i * tileGap.y) + tilePivot;
                    instantiated.gameObject.SetActive(true);
                    instantiated.GetComponent<TileEventListner>().Id = idCounter;

                    tileMaps.Add(idCounter, instantiated);
                    tilePositions.Add(idCounter, new Vector2Int(i, j));
                    idCounter++;
                }
            }

            for (int i = 0; i < wallBoard.GetLength(0); i++)
            {
                for (int j = 0; j < wallBoard.GetLength(1); j++)
                {
                    var pos = new Vector2Int(i, j);

                    if (board.ValidPosition(pos + Vector2Int.up) && puzzle.HasWall(pos, Direction.Up) && puzzle.HasWall(pos + Vector2Int.up, Direction.Down))
                    {
                        var wallPos = Vector3.Lerp(tileMaps[FindTileId(pos)].transform.position, tileMaps[FindTileId(pos + Vector2Int.up)].transform.position, 0.5f);
                        var wall = Instantiate(wallVerticalPrefab);
                        wall.transform.position = wallPos;
                    }
                    if (board.ValidPosition(pos + Vector2Int.down) && puzzle.HasWall(pos, Direction.Down) && puzzle.HasWall(pos + Vector2Int.down, Direction.Up))
                    {
                        // Skip
                    }
                    if (board.ValidPosition(pos + Vector2Int.left) && puzzle.HasWall(pos, Direction.Left) && puzzle.HasWall(pos + Vector2Int.left, Direction.Right))
                    {
                        // Skip
                    }
                    if (board.ValidPosition(pos + Vector2Int.right) && puzzle.HasWall(pos, Direction.Right) && puzzle.HasWall(pos + Vector2Int.right, Direction.Left))
                    {
                        var wallPos = Vector3.Lerp(tileMaps[FindTileId(pos)].transform.position, tileMaps[FindTileId(pos + Vector2Int.right)].transform.position, 0.5f);
                        var wall = Instantiate(wallHorizontalPrefab);
                        wall.transform.position = wallPos;
                    }
                }
            }

            var flag = Instantiate(flagPrefab);
            flag.transform.position = tileMaps[FindTileId(puzzle.GetEndPosition())].transform.position;
            player.transform.position = tileMaps[FindTileId(puzzle.GetNowPosition())].transform.position;
        }

        public void SubmitMove(List<Vector2Int> moveList)
        {
            for (int i = 0; i < moveList.Count - 1; i++)
            {
                var from = moveList[i];
                var to = moveList[i + 1];
                var dir = PuzzleUtil.GetDirection(from, to);

                puzzle.Move(dir);
                MovePlayer(from, to);
            }

            if (puzzle.IsEndGame())
                Debug.Log($"End Game. {puzzle.IsSuccessGame()}");

            Debug.Log(puzzle.GetNowPosition());
        }

        public void UndoMove()
        {
            var log = puzzle.GetPositionLog();

            if (log.Count > 0)
            {
                var prePos = log.Peek();
                puzzle.UndoMove();
                procedures.AddProcedure(UndoMovePlayer(prePos));
            }
        }

        public Vector2Int GetPlayerPosition()
        {
            return puzzle.GetNowPosition();
        }

        public Vector2Int GetTilePosition(int id)
        {
            return tilePositions[id];
        }

        void MovePlayer(Vector2Int prevPos, Vector2Int nowPos)
        {
            var prevId = FindTileId(prevPos);
            var nowId = FindTileId(nowPos);
            procedures.AddProcedure(MovePlayerCo(prevId, nowId));
        }

        IEnumerator MovePlayerCo(int prevTileId, int nowTileId)
        {
            var prevTile = tileMaps[prevTileId];
            var nowTile = tileMaps[nowTileId];

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

        IEnumerator UndoMovePlayer(Vector2Int prePos)
        {
            var preId = FindTileId(prePos);
            var tile = tileMaps[preId];

            tile.GetComponent<TileEventListner>().UndoDrop();
            player.transform.position = tile.transform.position;

            yield return null;
        }

        int FindTileId(Vector2Int tilePosition)
        {
            foreach (var tile in tilePositions)
            {
                if (tile.Value == tilePosition)
                    return tile.Key;
            }

            return -1;
        }
    }
}
