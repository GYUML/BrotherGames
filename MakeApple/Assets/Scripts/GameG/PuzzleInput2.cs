using System.Collections.Generic;
using UnityEngine;

namespace GameG
{
    public class PuzzleInput2 : MonoBehaviour
    {
        public GameObject selectBoxPrefab;
        public FieldView2 fieldView;
        public DragCamera dragLayout;

        BasePuzzleBoard board;
        bool[,] selectStateBoard;

        Stack<GameObject> selectBoxPool = new Stack<GameObject>();
        List<GameObject> selectedBoxList = new List<GameObject>();
        List<Vector2Int> moveList = new List<Vector2Int>();

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (board.IsEndGame())
                    return;

                if (dragLayout.IsDragging)
                    return;

                var touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.Raycast(touchPosition, Vector2.zero, 10f, LayerMask.GetMask("Tile"));
                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("Platform"))
                    {
                        var id = hit.transform.GetComponent<TileEventListner>().Id;
                        var vPos = board.PlayerPos;
                        var pos = fieldView.GetTilePosition(id);
                        var dir = board.GetDirection(vPos, pos);

                        // 현재 타일은 선택되지 않은 상태여야 한다.
                        if (selectStateBoard[pos.x, pos.y] == false)
                        {
                            // 현재 캐릭터 위치이거나, 캐릭터 위치가 선택된 상태여야 한다.
                            if (pos == vPos)
                            {
                                dragLayout.Locked = true;

                                selectStateBoard[pos.x, pos.y] = true;
                                ShowSelectBox(hit.transform.position);
                                moveList.Add(pos);
                            }
                            else if (selectStateBoard[vPos.x, vPos.y] == true)
                            {
                                if (board.IsMovePossible(dir) && !board.IsEndGame())
                                {
                                    board.MoveDirection(dir);
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
                dragLayout.Locked = false;

                fieldView.SubmitMove(moveList);
                ClearSelectedBoxes();
                selectStateBoard.SetAllFalse();
                moveList.Clear();

                Debug.Log($"{board.PlayerPos}");
            }
        }

        public void Initialize(BasePuzzleBoard board)
        {
            this.board = board;
            board.Initialize();
            selectStateBoard = new bool[board.GetLength(0), board.GetLength(1)];
        }

        public void UndoMove()
        {
            // TODO
            //var log = virtualPuzzle.GetPositionLog();

            //if (log.Count > 0)
            //{
            //    virtualPuzzle.UndoMove();
            //    fieldView.UndoMove();
            //}
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
    }
}
