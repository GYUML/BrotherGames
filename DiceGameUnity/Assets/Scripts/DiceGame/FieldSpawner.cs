using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class FieldSpawner : MonoBehaviour
{
    public EffectManager effectManager;

    public GameObject mapPrefab;
    public GameObject coinPrefab;
    public GameObject playerFigure;

    public Animator figureAnimator;

    public GameObject dice0;
    public GameObject dice1;

    public List<GameObject> mapItemPrefabList;

    public Vector2Int size;
    public float gap;

    public float figureSpeed;
    public float positionError;
    public float dicePower = 1f;

    Dictionary<string, List<GameObject>> mapItemList = new Dictionary<string, List<GameObject>>();
    Dictionary<int, GameObject> mapItemDic = new Dictionary<int, GameObject>();

    List<Vector3> tilePositions = new List<Vector3>();

    private void Awake()
    {
        mapPrefab.gameObject.SetActive(false);
        coinPrefab.gameObject.SetActive(false);

        // Make Map tile Positions
        for (int i = 0; i < size.y; i++)
            tilePositions.Add(new Vector3(0f, 0f, i * gap));

        for (int i = 0; i < size.x; i++)
            tilePositions.Add(new Vector3(i * gap, 0f, size.y * gap));

        for (int i = size.y; i > 0; i--)
            tilePositions.Add(new Vector3(size.x * gap, 0f, i * gap));

        for (int i = size.x; i > 0; i--)
            tilePositions.Add(new Vector3(i * gap, 0f, 0f));

        foreach (var position in tilePositions)
            SpawnMapItem(mapPrefab, position);
    }

    public void SpawnMapItem(TileType tileType, int tileIndex)
    {
        var position = tilePositions[tileIndex];
        var prefabCode = tileType == TileType.Enemy ? 1 : 0;
        var mapItem = SpawnMapItem(mapItemPrefabList[prefabCode], position + new Vector3(0f, 1f, 0f));
        
        mapItemDic.Add(tileIndex, mapItem);

        if (tileType == TileType.Coin)
        {
            var item = mapItem.GetComponent<DroppedItem>();

            if (item != null)
            {
                item.SpawnItem(0, 0, Vector3.zero, (id, code, dropped) =>
                {
                    dropped.gameObject.SetActive(false);
                    effectManager.ShowEffect(1, dropped.transform.position);
                });
            }
        }
    }

    public GameObject SpawnMapItem(GameObject prefab, Vector3 position)
    {
        var block = Instantiate(prefab);
        block.gameObject.SetActive(true);
        block.transform.position = position;

        if (!mapItemList.ContainsKey(prefab.name))
            mapItemList.Add(prefab.name, new List<GameObject>());

        if (mapItemList.TryGetValue(prefab.name, out var itemList))
            itemList.Add(block);

        return block;
    }

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

        yield return new WaitForSeconds(1f);
    }

    public IEnumerator MoveFigureCo(int from, int to)
    {
        figureAnimator.SetBool("Move", true);

        for (int i = from; i < to; i++)
            yield return MoveFigureOneCo(i);

        figureAnimator.SetBool("Move", false);
    }

    public IEnumerator MoveAndKillCo(int from)
    {
        var fromPosition = tilePositions[from];
        var toPosition = tilePositions[from + 1];

        fromPosition.y = playerFigure.transform.position.y;
        toPosition.y = playerFigure.transform.position.y;

        playerFigure.transform.position = fromPosition;
        playerFigure.transform.LookAt(toPosition);

        var showEffectTime = Time.time + 0.1f;

        while (Vector3.SqrMagnitude(playerFigure.transform.position - toPosition) > positionError)
        {
            if (Time.time > showEffectTime)
            {
                showEffectTime = float.MaxValue;
                effectManager.ShowEffect(3, toPosition);
                effectManager.ShowEffect(2, toPosition);
                mapItemDic[from + 1].gameObject.SetActive(false);
            }
            
            yield return null;
            playerFigure.transform.position += (toPosition - playerFigure.transform.position).normalized * figureSpeed * Time.deltaTime;
        }
    }

    public IEnumerator AttackCo(int targetIndex, bool isDead)
    {
        var targetItem = mapItemDic[targetIndex];
        var targetPosition = targetItem.transform.position;
        figureAnimator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.4f);

        effectManager.ShowEffect(3, targetPosition + new Vector3(0f, 1f, 0f));

        if (isDead)
        {
            effectManager.ShowEffect(2, targetPosition);
            targetItem.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(0.3f);
    }

    IEnumerator MoveFigureOneCo(int from)
    {
        var fromPosition = tilePositions[from];
        var toPosition = tilePositions[from + 1];

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
