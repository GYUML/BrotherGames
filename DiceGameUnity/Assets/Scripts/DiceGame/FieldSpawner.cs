using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSpawner : MonoBehaviour
{
    public GameObject mapPrefab;
    public GameObject coinPrefab;
    public GameObject playerFigure;

    public Animator figureAnimator;

    public GameObject dice0;
    public GameObject dice1;

    public Vector2Int size;
    public float gap;

    public float figureSpeed;
    public float positionError;

    Dictionary<string, List<GameObject>> mapItemDic = new Dictionary<string, List<GameObject>>();
    Queue<IEnumerator> procedureQue = new Queue<IEnumerator>();

    private void Start()
    {
        mapPrefab.gameObject.SetActive(false);
        coinPrefab.gameObject.SetActive(false);

        SpawnMap(size, gap);
        SpawnCoin(size, gap);

        StartCoroutine(ProcCo());
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
        //dice0.transform.position = new Vector3(11f, 2f, 11f);
        //dice1.transform.position = new Vector3(10f, 2f, 11f);
        procedureQue.Enqueue(RollDiceCo(number0, number1));
    }

    public void MoveFigure(int from, int to)
    {
        //if (mapItemDic.TryGetValue(mapPrefab.name, out var itemList))
        //{
        //    var mapItem = itemList[position];
        //    playerFigure.transform.position = mapItem.transform.position + new Vector3(0f, 0.5f, 0f);
        //}
        for (int i = from; i < to; i++)
            procedureQue.Enqueue(MoveFigureCo(i, i + 1));
    }

    IEnumerator ProcCo()
    {
        while (true)
        {
            if (procedureQue.Count > 0)
            {
                var nowProc = procedureQue.Dequeue();
                yield return nowProc;
            }

            yield return null;
        }
    }

    IEnumerator RollDiceCo(int number0, int number1)
    {
        dice0.transform.localPosition = new Vector3(0.5f, 2f, 0f);
        dice1.transform.localPosition = new Vector3(-0.5f, 2f, 0f);

        yield return new WaitForSeconds(1f);
    }

    IEnumerator MoveFigureCo(int from, int to)
    {
        figureAnimator.Play("Move");

        if (mapItemDic.TryGetValue(mapPrefab.name, out var itemList))
        {
            var fromItem = itemList[from];
            var toItem = itemList[to];

            if (fromItem != null && toItem != null)
            {
                var fromPosition = fromItem.transform.position;
                var toPosition = toItem.transform.position;

                fromPosition.y = playerFigure.transform.position.y;
                toPosition.y = playerFigure.transform.position.y;

                playerFigure.transform.position = fromPosition;

                while (Vector3.SqrMagnitude(playerFigure.transform.position - toPosition) > positionError)
                {
                    yield return null;
                    playerFigure.transform.position += (toPosition - playerFigure.transform.position).normalized * figureSpeed * Time.deltaTime;
                }
            }
        }

        figureAnimator.Play("Idle");
    }
}
