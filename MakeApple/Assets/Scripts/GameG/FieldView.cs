using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameG
{
    public class FieldView : MonoBehaviour
    {
        public GameProcedures procedures;

        TilePuzzle puzzle = new TilePuzzle();
        TilePuzzle virtualPuzzle = new TilePuzzle();

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
        Stack<GameObject> selectBoxPool = new Stack<GameObject>();
        List<GameObject> selectedBoxList = new List<GameObject>();
        Dictionary<int, Vector2Int> tilePositions = new Dictionary<int, Vector2Int>();
        List<Vector2Int> moveList = new List<Vector2Int>();

        bool[,] selectStateBoard;

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

        private void Awake()
        {
            tileBlockPrefab.gameObject.SetActive(false);

            puzzle.Init(testBoard, new Vector2Int(0, 0), new Vector2Int(3, 2), testWall);
            virtualPuzzle.Init(testBoard, new Vector2Int(0, 0), new Vector2Int(3, 2), testWall);

            SpawnField(puzzle);
        }

        private void Update()
        {
            if (puzzle.IsEndGame())
                return;

            if (Input.GetMouseButton(0))
            {
                var touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(touchPosition, Vector2.zero, 10f, LayerMask.GetMask("Tile"));
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Platform"))
                    {
                        var id = hit.transform.GetComponent<TileEventListner>().Id;
                        var nowPos = puzzle.GetNowPosition();
                        var vPos = virtualPuzzle.GetNowPosition();
                        var pos = tilePositions[id];
                        var dir = GetDirection(vPos, pos);

                        // 현재 타일은 선택되지 않은 상태여야 한다.
                        if (selectStateBoard[pos.x, pos.y] == false)
                        {
                            // 현재 캐릭터 위치이거나, 캐릭터 위치가 선택된 상태여야 한다.
                            if (pos == nowPos)
                            {
                                selectStateBoard[pos.x, pos.y] = true;
                                ShowSelectBox(hit.transform.position);
                                moveList.Add(pos);
                            }
                            else if (selectStateBoard[nowPos.x, nowPos.y] == true)
                            {
                                if (virtualPuzzle.IsMovePossible(dir) && !virtualPuzzle.IsEndGame())
                                {
                                    virtualPuzzle.Move(dir);
                                    selectStateBoard[pos.x, pos.y] = true;
                                    ShowSelectBox(hit.transform.position);
                                    moveList.Add(pos);
                                    Debug.Log(virtualPuzzle.GetString());
                                }
                            }
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                
            }
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

            selectStateBoard = new bool[board.GetLength(0), board.GetLength(1)];
            player.transform.position = tileMaps[FindTileId(puzzle.GetNowPosition())].transform.position;
        }

        public void SubmitMove()
        {
            for (int i = 0; i < moveList.Count - 1; i++)
            {
                var from = moveList[i];
                var to = moveList[i + 1];
                var dir = GetDirection(from, to);

                puzzle.Move(dir);
                MovePlayer(from, to);
            }
            moveList.Clear();
            ClearSelectedBoxes();
            selectStateBoard.SetAllFalse();

            if (puzzle.IsEndGame())
                Debug.Log($"End Game. {puzzle.IsSuccessGame()}");
        }

        public void ClearSelect()
        {
            for (int i = 0; i < moveList.Count - 1; i++)
                virtualPuzzle.UndoMove();

            moveList.Clear();
            ClearSelectedBoxes();
            selectStateBoard.SetAllFalse();
        }

        public void UndoMove()
        {
            var log = puzzle.GetPositionLog();
            ClearSelect();

            if (log.Count > 0)
            {
                var prePos = log.Peek();
                puzzle.UndoMove();
                virtualPuzzle.UndoMove();
                procedures.AddProcedure(UndoMovePlayer(prePos));
            }
        }

        void ShowSelectBox(Vector3 position)
        {
            var selectedBox = selectBoxPool.Count > 0 ? selectBoxPool.Pop() : Instantiate(selectBoxPrefab);
            selectedBox.transform.position = position;
            selectedBox.gameObject.SetActive(true);
            selectedBoxList.Add(selectedBox);
        }

        void ClearSelectedBoxes()
        {
            foreach (var selectedBox in selectedBoxList)
            {
                selectedBox.gameObject.SetActive(false);
                selectBoxPool.Push(selectedBox);
            }

            selectedBoxList.Clear();
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
            prevTile.GetComponent<TileEventListner>().DoDrop();
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

        Direction GetDirection(Vector2Int from, Vector2Int to)
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
}
