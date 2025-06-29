using System;
using UnityEngine;

namespace GameE
{
    public class BaseLever : MonoBehaviour
    {
        Action<Collider2D> onTriggerEnter;
        Action<Collider2D> onTriggerExit;

        public void SetTriggerEnter(Action<Collider2D> onTriggerEnter)
        {
            this.onTriggerEnter = onTriggerEnter;
        }

        public void SetTriggerExit(Action<Collider2D> onTriggerExit)
        {
            this.onTriggerExit = onTriggerExit;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            onTriggerEnter?.Invoke(collision);
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            onTriggerExit?.Invoke(collision);
        }
    }
}
