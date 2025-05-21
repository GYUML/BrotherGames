using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameE
{
    public class DropItemSpawner : MonoBehaviour
    {
        public DroppedItem coinPrefab;

        public float dropPower;
        public float dropRangeX;

        public float acquireMoveTime;
        public float acquireMoveError;

        Stack<DroppedItem> coinPool = new Stack<DroppedItem>();

        public void DropItem(Vector2 position, int count, Action onDestroy)
        {
            position.y += 1f;
            for (int i = 0; i < count; i++)
                DropItem(position, onDestroy);
        }

        void DropItem(Vector2 position, Action onDestroy)
        {
            var item = coinPool.Count > 0 ? coinPool.Pop() : Instantiate(coinPrefab);
            var dropVector = new Vector2(UnityEngine.Random.Range(-dropRangeX, dropRangeX), 1f) * dropPower;
            item.transform.position = position;
            item.gameObject.SetActive(true);
            item.Drop(dropVector, acquireMoveTime, acquireMoveError, onDestroy);
        }
    }
}
