using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameG
{
    public class MainGameLogic : MonoBehaviour
    {
        public GameProcedures procedures;

        // Data setting
        int[,] board = new int[,]
        {
            { 1, 1, 1},
            { 1, 1, 1},
            { 1, 1, 1},
        };

        Vector2Int startPosition;
        Vector2Int endPosition;

        // Data State
        int[,] nowBoard;
        Vector2Int nowPosition;
        Vector2Int prePosition;
        int remainTile = 0;

        // GameObject setting
        public GameObject tileBlockPrefab;
        public GameObject selectBoxPrefab;
        public GameObject player;

        public float tileGap;
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

        private void Awake()
        {
            tileBlockPrefab.gameObject.SetActive(false);

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    var instantiated = Instantiate(tileBlockPrefab, tileBlockPrefab.transform.parent);
                    instantiated.transform.position = new Vector2(j * tileGap, -i * tileGap) + tilePivot;
                    instantiated.gameObject.SetActive(true);
                    instantiated.GetComponent<TileEventListner>().Id = idCounter;

                    tileMaps.Add(idCounter, instantiated);
                    tilePositions.Add(idCounter, new Vector2Int(i, j));
                    idCounter++;
                }
            }

            nowBoard = board.DeepCopy();
            remainTile = GetRemainTileCount(nowBoard);

            selectStateBoard = new bool[nowBoard.GetLength(0), nowBoard.GetLength(1)];

            MoveToTile(startPosition, true);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                var touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(touchPosition, Vector2.zero, 10f, LayerMask.GetMask("Tile"));
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Platform"))
                    {
                        var id = hit.transform.GetComponent<TileEventListner>().Id;
                        var pos = tilePositions[id];

                        // 현재 타일은 선택되지 않은 상태여야 한다.
                        if (selectStateBoard[pos.x, pos.y] == false)
                        {
                            // 캐릭터 위치이거나, 캐릭터 위치가 선택된 상태여야 한다.
                            if (pos == nowPosition)
                            {
                                selectStateBoard[pos.x, pos.y] = true;
                                ShowSelectBox(hit.transform.position);
                                moveList.Add(pos);
                            }
                            else if (selectStateBoard[nowPosition.x, nowPosition.y] == true)
                            {
                                if (nowBoard[pos.x, pos.y] == 1 && Vector2Int.Distance(moveList.Last(), pos) == 1)
                                {
                                    selectStateBoard[pos.x, pos.y] = true;
                                    ShowSelectBox(hit.transform.position);
                                    moveList.Add(pos);
                                }
                            }
                        }
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                foreach (var pos in moveList)
                    MoveToTile(pos, false);
                moveList.Clear();
                ClearSelectedBoxes();
                selectStateBoard.SetAllFalse();
            }
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

        void MoveToTile(Vector2Int to, bool immediately)
        {
            if (!immediately && nowPosition == to)
                return;
            else if (to.x < 0 || to.x >= nowBoard.GetLength(0)
                || to.y < 0 || to.y >= nowBoard.GetLength(1)
                || nowBoard[to.x, to.y] != 1)
            {
                Debug.LogError($"Failed to move. to={to}");
                return;
            }
            else if ((to - nowPosition).sqrMagnitude > 1)
            {
                Debug.LogError($"Too much move. now={nowPosition}, to={to}");
                return;
            }

            nowBoard[to.x, to.y] = 0;
            remainTile--;
            prePosition = nowPosition;
            nowPosition = to;

            if (immediately)
                MovePlayer(nowPosition);
            else
                MovePlayer(prePosition, nowPosition);

            Debug.Log($"remainTile : {remainTile}");
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

        void MovePlayer(Vector2Int position)
        {
            var tileId = FindTileId(position);
            procedures.AddProcedure(MovePlayer(tileMaps[tileId].transform.position, true));
        }

        void MovePlayer(Vector2Int prevPos, Vector2Int nowPos)
        {
            var prevId = FindTileId(prevPos);
            var nowId = FindTileId(nowPos);
            procedures.AddProcedure(MovePlayer(prevId, nowId));
        }

        IEnumerator MovePlayer(int prevTileId, int nowTileId)
        {
            var prevTile = tileMaps[prevTileId];
            var nowTile = tileMaps[nowTileId];

            yield return MovePlayer(nowTile.transform.position, false);
            prevTile.GetComponent<TileEventListner>().DoDrop();
        }

        IEnumerator MovePlayer(Vector3 position, bool immediately)
        {
            if (!immediately)
            {
                while (Vector3.SqrMagnitude(player.transform.position - position) > moveError)
                {
                    yield return null;
                    player.transform.position = Vector3.MoveTowards(player.transform.position, position, Time.deltaTime * moveSpeed);
                }
            }
            
            player.transform.position = position;
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
