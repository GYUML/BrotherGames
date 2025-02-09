using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameAUI
{
    public class GameBoardItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        public TMP_Text numberText;
        public Action onPointerDown;
        public Action onPointerUp;
        public Action onPointerEnter;

        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp?.Invoke();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onPointerEnter?.Invoke();
        }

        public void SetEvent(Action onPointerDown, Action onPointerUp, Action onPointerEnter)
        {
            this.onPointerDown = onPointerDown;
            this.onPointerUp = onPointerUp;
            this.onPointerEnter = onPointerEnter;
        }

        public void SetNumber(int number)
        {
            numberText.text = number > 0 ? number.ToString() : string.Empty;
        }

        public void Selected(bool selected)
        {
            transform.localScale = selected ? Vector3.one * 0.8f : Vector3.one;
        }
    }
}
