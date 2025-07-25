using System.Collections.Generic;
using UnityEngine;

public class FieldSpawner : MonoBehaviour
{
    public GameObject mapPrefab;
    public GameObject coinPrefab;
    public GameObject playerFigure;

    public GameObject dice0;
    public GameObject dice1;

    public Vector2Int size;
    public float gap;

    Dictionary<string, List<GameObject>> mapItemDic = new Dictionary<string, List<GameObject>>();
    
    private void Start()
    {
        mapPrefab.gameObject.SetActive(false);
        coinPrefab.gameObject.SetActive(false);

        SpawnMap(size, gap);
        SpawnCoin(size, gap);
    }

    public void SpawnMap(Vector2Int size, float gap)
    {
        for (int i = 0; i < size.y; i++)
            SpawnMapItem(mapPrefab, new Vector3(0f, 0f, i * gap));
        
        for (int i = 0; i < size.x; i++)
            SpawnMapItem(mapPrefab, new Vector3(i * gap, 0f, size.y * gap));
        
        for (int i = size.y; i > 0; i--)
            SpawnMapItem(mapPrefab, new Vector3(size.x * gap, 0f, i * gap));

        for (int i = size.x; i > 0; i--)
            SpawnMapItem(mapPrefab, new Vector3(i * gap, 0f, 0f));
    }

    public void SpawnCoin(Vector2Int size, float gap)
    {
        for (int i = 1; i < size.y; i++)
            SpawnMapItem(coinPrefab, new Vector3(0f, 0f, i * gap));

        for (int i = 1; i < size.x; i++)
            SpawnMapItem(coinPrefab, new Vector3(i * gap, 0f, size.y * gap));

        for (int i = size.y - 1; i > 0; i--)
            SpawnMapItem(coinPrefab, new Vector3(size.x * gap, 0f, i * gap));

        for (int i = size.x - 1; i > 0; i--)
            SpawnMapItem(coinPrefab, new Vector3(i * gap, 0f, 0f));
    }

    public void SpawnMapItem(GameObject prefab, Vector3 position)
    {
        var block = Instantiate(prefab);
        block.gameObject.SetActive(true);
        block.transform.position = position;

        if (!mapItemDic.ContainsKey(prefab.name))
            mapItemDic.Add(prefab.name, new List<GameObject>());

        if (mapItemDic.TryGetValue(prefab.name, out var itemList))
            itemList.Add(block);
    }

    public void RollDice(int number0, int number1)
    {
        dice0.transform.position = new Vector3(11f, 2f, 11f);
        dice1.transform.position = new Vector3(10f, 2f, 11f);
    }

    public void MoveFigure(int position)
    {
        if (mapItemDic.TryGetValue(mapPrefab.name, out var itemList))
        {
            var mapItem = itemList[position];
            playerFigure.transform.position = mapItem.transform.position + new Vector3(0f, 0.5f, 0f);
        }
    }
}
