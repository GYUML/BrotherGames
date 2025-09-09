using GameG;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameG
{
    public class MakeMapLayout : MonoBehaviour
    {
        public int SizeX = 5;
        public int SizeY = 4;

        public KButton createButton;
        public KButton saveButton;
        public KButton loadButton;
        public GridLayoutGroup layoutGroup;
        public MakeMapLayoutTile tilePrefab;

        public TMP_Dropdown editModeDropdown;
        public TMP_Dropdown tileTypeDropdown;

        public Toggle wallMaskUp;
        public Toggle wallMaskDown;
        public Toggle wallMaskLeft;
        public Toggle wallMaskRight;

        Stack<MakeMapLayoutTile> tilePool = new Stack<MakeMapLayoutTile>();
        MakeMapLayoutTile[,] tileBoard;
        BoardData boardData;

        Vector2Int nowSelectedTile;

        enum EditMode
        {
            None,
            TileType,
            WallMask
        }

        private void Start()
        {
            tilePrefab.gameObject.SetActive(false);

            createButton.onClick.AddListener(() => CreateMap(SizeX, SizeY));
        }

        private void OnEnable()
        {
            // Edit Mode 드롭다운
            var editModes = Enum.GetNames(typeof(EditMode));
            editModeDropdown.AddOptions(editModes.ToList());

            // 타일 종류 드롭다운
            var tileTypes = Enum.GetNames(typeof(TileEnum));
            tileTypeDropdown.AddOptions(tileTypes.ToList());
        }

        void CreateMap(int sizeX, int sizeY)
        {
            boardData = new BoardData(sizeX, sizeY);
            tileBoard = new MakeMapLayoutTile[sizeX, sizeY];
            layoutGroup.constraintCount = sizeX;

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    var tile = tilePool.Count > 0 ? tilePool.Pop() : Instantiate(tilePrefab, tilePrefab.transform.parent);
                    var pos = new Vector2Int(i, j);

                    tile.gameObject.SetActive(true);
                    //tile.GetComponent<RectTransform>().anchoredPosition = new Vector2(120f + j * 110f, -120f + -i * 110f);
                    tile.SetClickEvent(() => OnSelectTile(pos.x, pos.y));

                    tileBoard[pos.x, pos.y] = tile;
                }
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
            tileBoard[x, y].SetText(tileEnum.ToString());
        }

        void OnSelectWallMask(int x, int y)
        {
            wallMaskUp.isOn = boardData.GetTileData(x, y).HasWall(Direction.Up);
            wallMaskDown.isOn = boardData.GetTileData(x, y).HasWall(Direction.Down);
            wallMaskLeft.isOn = boardData.GetTileData(x, y).HasWall(Direction.Left);
            wallMaskRight.isOn = boardData.GetTileData(x, y).HasWall(Direction.Right);
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
        }
    }
}
