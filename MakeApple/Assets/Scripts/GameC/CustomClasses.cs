using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutinePlayer
{
    Queue<IEnumerator> procedureQue = new Queue<IEnumerator>();

    public void AddCoroutine(IEnumerator coroutine)
    {
        procedureQue.Enqueue(coroutine);
    }

    public void ClearCoroutine()
    {
        procedureQue.Clear();
    }

    public IEnumerator MainCoroutine()
    {
        while (true)
        {
            while (procedureQue.Count <= 0)
                yield return null;

            while (procedureQue.Count > 0)
            {
                var procedure = procedureQue.Dequeue();
                yield return procedure;
            }
        }
    }
}
