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
        SpawnCoinItems(size, gap);

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

    public void SpawnCoinItems(Vector2Int size, float gap)
    {
        for (int i = 1; i < size.y; i++)
            SpawnCoinItem(new Vector3(0f, 0f, i * gap));

        for (int i = 1; i < size.x; i++)
            SpawnCoinItem(new Vector3(i * gap, 0f, size.y * gap));

        for (int i = size.y - 1; i > 0; i--)
            SpawnCoinItem(new Vector3(size.x * gap, 0f, i * gap));

        for (int i = size.x - 1; i > 0; i--)
            SpawnCoinItem(new Vector3(i * gap, 0f, 0f));
    }

    public void SpawnCoinItem(Vector3 position)
    {
        var coin = SpawnMapItem(coinPrefab, position);
        var item = coin.GetComponent<DroppedItem>();

        if (item != null)
        {
            item.SpawnItem(0, 0, Vector3.zero, (id, code, dropped) => dropped.gameObject.SetActive(false));
        }
    }

    public GameObject SpawnMapItem(GameObject prefab, Vector3 position)
    {
        var block = Instantiate(prefab);
        block.gameObject.SetActive(true);
        block.transform.position = position;

        if (!mapItemDic.ContainsKey(prefab.name))
            mapItemDic.Add(prefab.name, new List<GameObject>());

        if (mapItemDic.TryGetValue(prefab.name, out var itemList))
            itemList.Add(block);

        return block;
    }

    public void RollDice(int number0, int number1)
    {
        procedureQue.Enqueue(RollDiceCo(number0, number1));
    }

    public void MoveFigure(int from, int to)
    {
        procedureQue.Enqueue(MoveFigureCo(from, to));
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

    public float dicePower = 1f;

    public IEnumerator RollDiceCo(int number0, int number1)
    {
        var dice0Rigid = dice0.GetComponent<Rigidbody>();
        var dice1Rigid = dice1.GetComponent<Rigidbody>();

        dice0.transform.localPosition = new Vector3(0.5f, 0.9f, 0f);
        dice1.transform.localPosition = new Vector3(-0.5f, 0.9f, 0f);

        dice0Rigid.rotation = Quaternion.Euler(new Vector3(70f, 50f, 0f));
        dice1Rigid.rotation = Quaternion.Euler(new Vector3(0f, 50f, 70f));

        dice0Rigid.AddForce(Vector3.up * dicePower, ForceMode.Impulse);
        dice1Rigid.AddForce(Vector3.up * dicePower, ForceMode.Impulse);

        yield return new WaitForSeconds(2f);
    }

    public IEnumerator MoveFigureCo(int from, int to)
    {
        figureAnimator.Play("Move");

        for (int i = from; i < to; i++)
            yield return MoveFigureOneCo(i);

        figureAnimator.Play("Idle");
    }

    IEnumerator MoveFigureOneCo(int from)
    {
        if (mapItemDic.TryGetValue(mapPrefab.name, out var itemList))
        {
            var fromItem = itemList[from];
            var toItem = itemList[from + 1];

            if (fromItem != null && toItem != null)
            {
                var fromPosition = fromItem.transform.position;
                var toPosition = toItem.transform.position;

                fromPosition.y = playerFigure.transform.position.y;
                toPosition.y = playerFigure.transform.position.y;

                playerFigure.transform.position = fromPosition;
                playerFigure.transform.LookAt(toPosition);

                while (Vector3.SqrMagnitude(playerFigure.transform.position - toPosition) > positionError)
                {
                    yield return null;
                    playerFigure.transform.position += (toPosition - playerFigure.transform.position).normalized * figureSpeed * Time.deltaTime;
                }
            }
        }
    }
}
