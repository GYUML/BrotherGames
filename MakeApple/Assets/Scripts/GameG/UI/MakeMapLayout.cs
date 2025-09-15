using GameG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameG
{
    public class MakeMapLayout : MonoBehaviour
    {
        public static string BoardFolder = "Assets/Resources/GameG/BoardData";

        public int SizeX = 5;
        public int SizeY = 4;

        public KButton createButton;
        public KButton saveButton;
        public KButton loadButton;
        public GridLayoutGroup layoutGroup;
        public MakeMapLayoutTile tilePrefab;

        public TMP_Dropdown editModeDropdown;
        public TMP_Dropdown tileTypeDropdown;

        public List<GameObject> editTabs;

        // ManageMap
        public TMP_InputField mapNameInput;

        // Config
        public TMP_InputField startXInput;
        public TMP_InputField startYInput;
        public TMP_InputField endXInput;
        public TMP_InputField endYInput;
        public KButton configApplyButton;

        // WallMask
        public Toggle wallMaskUp;
        public Toggle wallMaskDown;
        public Toggle wallMaskLeft;
        public Toggle wallMaskRight;
        public KButton applayButton;

        Stack<MakeMapLayoutTile> tilePool = new Stack<MakeMapLayoutTile>();
        MakeMapLayoutTile[,] tileBoard;
        BoardData boardData;

        Vector2Int nowSelectedTile;

        enum EditMode
        {
            ManageMap,
            Config,
            TileType,
            WallMask
        }

        private void Start()
        {
            tilePrefab.gameObject.SetActive(false);

            // 데이터 관리
            createButton.onClick.AddListener(() => CreateMap(SizeX, SizeY));
            saveButton.onClick.AddListener(SaveMap);
            loadButton.onClick.AddListener(LoadMap);

            // Edit Mode 드롭다운
            var editModes = Enum.GetNames(typeof(EditMode));
            editModeDropdown.AddOptions(editModes.ToList());
            editModeDropdown.onValueChanged.AddListener(ChangeTab);

            // 설정
            configApplyButton.onClick.AddListener(EditConfig);

            // 타일 종류 드롭다운
            var tileTypes = Enum.GetNames(typeof(TileEnum));
            tileTypeDropdown.AddOptions(tileTypes.ToList());

            // 벽
            applayButton.onClick.AddListener(EditWallMask);

            ChangeTab(0);
        }

        void CreateMap(int sizeX, int sizeY)
        {
            boardData = new BoardData(sizeX, sizeY);
            CreateTileView();
            UpdateTileView();
        }

        void CreateTileView()
        {
            // Clear
            if (tileBoard != null)
            {
                for (int i = 0; i < tileBoard.GetLength(0); i++)
                {
                    for (int j = 0; j < tileBoard.GetLength(1); j++)
                    {
                        var tile = tileBoard[i, j];
                        tile.gameObject.SetActive(false);
                        tilePool.Push(tile);
                    }
                }
            }
            
            // Create New
            var sizeX = boardData.GetLength(0);
            var sizeY = boardData.GetLength(1);

            tileBoard = new MakeMapLayoutTile[sizeX, sizeY];
            layoutGroup.constraintCount = sizeX;

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    var tile = tilePool.Count > 0 ? tilePool.Pop() : Instantiate(tilePrefab, tilePrefab.transform.parent);
                    var pos = new Vector2Int(i, j);

                    tile.gameObject.SetActive(true);
                    tile.SetClickEvent(() => OnSelectTile(pos.x, pos.y));

                    tileBoard[pos.x, pos.y] = tile;
                }
            }
        }

        void UpdateTileView()
        {
            var sizeX = boardData.GetLength(0);
            var sizeY = boardData.GetLength(1);

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    var pos = new Vector2Int(i, j);
                    var tileData = boardData.GetTileData(pos.x, pos.y);
                    var tileView = tileBoard[pos.x, pos.y];

                    tileView.SetText(((int)tileData.tileEnum).ToString());
                    tileView.SetWallMaskText(tileData.wallMask);
                }
            }
        }

        void SaveMap()
        {
            if (mapNameInput.text != "")
                JsonTool.Save(boardData, BoardFolder, $"{mapNameInput.text}.json");
            else
                Debug.LogError($"Not valid file name. {mapNameInput.text}");
        }

        void LoadMap()
        {
            if (mapNameInput.text != "")
            {
                if (JsonTool.TryLoad<BoardData>(BoardFolder, $"{mapNameInput.text}.json", out var output))
                {
                    boardData = output;
                    CreateTileView();
                    UpdateTileView();
                }
                else
                {
                    Debug.LogError($"Failed to load the file. {mapNameInput.text}");
                }
            }
            else
                Debug.LogError($"Not valid file name. {mapNameInput.text}");
        }

        void ChangeTab(int editMode)
        {
            for (int i = 0; i < Enum.GetNames(typeof(EditMode)).Length; i++)
            {
                editTabs[i].SetActive(i == editMode);
            }
        }

        void OnSelectTile(int x, int y)
        {
            nowSelectedTile = new Vector2Int(x, y);

            if (editModeDropdown.value == (int)EditMode.TileType)
                OnSelectTileType(x, y);
            if (editModeDropdown.value == (int)EditMode.WallMask)
                OnSelectWallMask(x, y);
        }

        void OnSelectTileType(int x, int y)
        {
            var tileEnum = tileTypeDropdown.value;
            boardData.GetTileData(x, y).tileEnum = (TileEnum)tileEnum;
            //tileBoard[x, y].SetText(tileEnum.ToString());
            UpdateTileView();
        }

        void OnSelectWallMask(int x, int y)
        {
            wallMaskUp.isOn = boardData.GetTileData(x, y).HasWall(Direction.Up);
            wallMaskDown.isOn = boardData.GetTileData(x, y).HasWall(Direction.Down);
            wallMaskLeft.isOn = boardData.GetTileData(x, y).HasWall(Direction.Left);
            wallMaskRight.isOn = boardData.GetTileData(x, y).HasWall(Direction.Right);
        }

        void EditConfig()
        {
            int.TryParse(startXInput.text, out var startX);
            int.TryParse(startYInput.text, out var startY);
            int.TryParse(endXInput.text, out var endX);
            int.TryParse(endYInput.text, out var endY);

            BoardDataMaker.SetStartPos(boardData, startX, startY);
            BoardDataMaker.SetEndPos(boardData, endX, endY);
        }

        void EditWallMask()
        {
            if (wallMaskUp.isOn)
                BoardDataMaker.AddTileWall(boardData, nowSelectedTile.x, nowSelectedTile.y, Direction.Up);
            else
                BoardDataMaker.RemoveTileWall(boardData, nowSelectedTile.x, nowSelectedTile.y, Direction.Up);

            if (wallMaskDown.isOn)
                BoardDataMaker.AddTileWall(boardData, nowSelectedTile.x, nowSelectedTile.y, Direction.Down);
            else
                BoardDataMaker.RemoveTileWall(boardData, nowSelectedTile.x, nowSelectedTile.y, Direction.Down);

            if (wallMaskLeft.isOn)
                BoardDataMaker.AddTileWall(boardData, nowSelectedTile.x, nowSelectedTile.y, Direction.Left);
            else
                BoardDataMaker.RemoveTileWall(boardData, nowSelectedTile.x, nowSelectedTile.y, Direction.Left);

            if (wallMaskRight.isOn)
                BoardDataMaker.AddTileWall(boardData, nowSelectedTile.x, nowSelectedTile.y, Direction.Right);
            else
                BoardDataMaker.RemoveTileWall(boardData, nowSelectedTile.x, nowSelectedTile.y, Direction.Right);

            //tileBoard[nowSelectedTile.x, nowSelectedTile.y].SetWallMaskText(boardData.GetTileData(nowSelectedTile.x, nowSelectedTile.y).wallMask);
            UpdateTileView();
        }
    }
}
