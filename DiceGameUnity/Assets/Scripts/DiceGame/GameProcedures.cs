using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProcedures : MonoBehaviour
{
    public FieldSpawner fieldSpawner;
    public GameLayout gameLayout;

    Coroutine mainCoroutine;
    Queue<IEnumerator> procedureQue = new Queue<IEnumerator>();

    int nowPosition = 0;

    private void Start()
    {
        mainCoroutine = StartCoroutine(ProcCo());
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

    public void RollDice(float chargedGuage)
    {
        Debug.Log($"RollDice({chargedGuage})");
        procedureQue.Enqueue(RollDiceCo(chargedGuage));
    }

    IEnumerator RollDiceCo(float chargedGuage)
    {
        var dice0 = Random.Range(1, 7);
        var dice1 = Random.Range(1, 7);
        var diceSum = dice0 + dice1;

        gameLayout.SetChargeButtonEnable(false);
        gameLayout.ShowDiceScreen(true);

        yield return fieldSpawner.RollDiceCo(dice0, dice1);
        gameLayout.ShowDiceScreen(false);

        yield return fieldSpawner.MoveFigureCo(nowPosition, nowPosition + diceSum);
        nowPosition += diceSum;

        gameLayout.SetChargeButtonEnable(true);
    }
}
