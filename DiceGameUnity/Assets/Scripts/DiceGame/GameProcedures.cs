using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    None = 0,
    Coin = 1,
    Enemy = 2,
}

public class GameProcedures : MonoBehaviour
{
    public FieldSpawner fieldSpawner;
    public GameLayout gameLayout;
    public GameObject boardField;

    public BattleLayout battleLayout;
    public BattleField battleField;

    public Vector2Int mapSize;
    public float tileGap;

    Coroutine mainCoroutine;
    Queue<IEnumerator> procedureQue = new Queue<IEnumerator>();

    int nowPosition = 0;
    TileType[] tiles = new TileType[44];

    private void Start()
    {
        mainCoroutine = StartCoroutine(ProcCo());

        for (int i = 0; i < tiles.Length; i++)
        {
            if (i % 11 == 0)
                continue;

            if (Random.Range(0f, 1f) < 0.2f)
                tiles[i] = TileType.Enemy;
            else
                tiles[i] = TileType.Coin;

            fieldSpawner.SpawnMapItem(tiles[i], i);
        }

        MoveField(0);
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
            else
            {
                yield return null;
            }
        }
    }

    public void MoveField(int index)
    {
        procedureQue.Enqueue(MoveFieldCo(index));
    }

    public void RollDice(float chargedGuage)
    {
        Debug.Log($"RollDice({chargedGuage})");
        procedureQue.Enqueue(RollDiceCo(chargedGuage));
    }

    public void RollDiceAttack(float chargedGuage)
    {
        Debug.Log($"RollDice({chargedGuage})");
        procedureQue.Enqueue(RollDiceAttackCo(chargedGuage));
    }

    IEnumerator MoveFieldCo(int index)
    {
        boardField.gameObject.SetActive(index == 0);
        gameLayout.gameObject.SetActive(index == 0);
        battleField.gameObject.SetActive(index == 1);
        battleLayout.gameObject.SetActive(index == 1);
        yield break;
    }

    IEnumerator RollDiceCo(float chargedGuage)
    {
        var dice0 = Random.Range(1, 7);
        var dice1 = Random.Range(1, 7);
        var diceSum = dice0 + dice1;

        Debug.Log(diceSum);

        gameLayout.SetChargeButtonEnable(false);
        gameLayout.ShowDiceScreen(true);

        yield return fieldSpawner.RollDiceCo(dice0, dice1);
        gameLayout.ShowDiceScreen(false);

        for (int i = 0; i < diceSum; i++)
        {
            if (tiles[nowPosition + 1] == TileType.Enemy)
                yield return fieldSpawner.MoveAndKillCo(nowPosition);
            else
                yield return fieldSpawner.MoveFigureCo(nowPosition, nowPosition + 1);
            nowPosition++;
        }

        gameLayout.SetChargeButtonEnable(true);

        //MoveField(1);
    }

    IEnumerator RollDiceAttackCo(float chargedGuage)
    {
        var dice0 = Random.Range(1, 7);
        var dice1 = Random.Range(1, 7);
        var diceSum = dice0 + dice1;

        battleLayout.SetChargeButtonEnable(false);
        battleLayout.ShowDiceScreen(true);

        yield return fieldSpawner.RollDiceCo(dice0, dice1);
        battleLayout.ShowDiceScreen(false);

        yield return battleField.AttackEnemyCo(0);
        nowPosition += diceSum;

        battleLayout.SetChargeButtonEnable(true);
    }
}
