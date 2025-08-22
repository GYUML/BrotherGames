using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MakeMapLayout : MonoBehaviour
{
    public TMP_InputField sizeXInput;
    public TMP_InputField sizeYInput;
    public KButton makeButton;
    public KButton saveButton;
    public KButton loadButton;
    
    public KButton tilePrefab;

    Stack<KButton> tilePool = new Stack<KButton>();
    KButton[,] tileBoard;

    int[,] board;

    private void Start()
    {
        makeButton.onClick.AddListener(() =>
        {
            if (int.TryParse(sizeXInput.text, out var sizeX) && int.TryParse(sizeYInput.text, out var sizeY))
                MakeMap(sizeX, sizeY);
        });

        tilePrefab.gameObject.SetActive(false);
    }

    public void MakeMap(int sizeX, int sizeY)
    {
        ClearTileBoard();

        board = new int[sizeX, sizeY];
        tileBoard = new KButton[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                var tile = tilePool.Count > 0 ? tilePool.Pop() : Instantiate(tilePrefab, tilePrefab.transform.parent);
                var pos = new Vector2Int(i, j);
                tile.onClick.AddListener(() => ChangeTile(pos));
                tile.gameObject.SetActive(true);
                tile.GetComponent<RectTransform>().anchoredPosition = new Vector2(120f + j * 110f, -120f + -i * 110f);
                tileBoard[i, j] = tile;
            }
        }
    }

    void ChangeTile(Vector2Int pos)
    {
        board[pos.x, pos.y] = (board[pos.x, pos.y] + 1) % 2;
        tileBoard[pos.x, pos.y].GetComponentInChildren<TMP_Text>().text = board[pos.x, pos.y].ToString();
    }

    void ClearTileBoard()
    {
        if (tileBoard == null)
            return;

        foreach (var tile in tileBoard)
        {
            tile.gameObject.SetActive(false);
            tilePool.Push(tile);
        }
    }
}
