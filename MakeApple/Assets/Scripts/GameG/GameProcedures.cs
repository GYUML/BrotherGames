using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameG
{
    public class GameProcedures : MonoBehaviour
    {
        CoroutinePlayer coroutinePlayer = new CoroutinePlayer();
        Coroutine coroutinePlayerCo;

        private void Start()
        {
            coroutinePlayerCo = StartCoroutine(coroutinePlayer.MainCoroutine());
        }

        private void OnDisable()
        {
            if (coroutinePlayerCo != null)
                StopCoroutine(coroutinePlayerCo);
        }

        public void AddProcedure(IEnumerator enumerator)
        {
            coroutinePlayer.AddCoroutine(enumerator);
        }
    }
}
