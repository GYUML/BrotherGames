using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameAUI
{
    public class GameBoardItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
    {
        public Image icon;
        public Image selected;
        public TMP_Text numberText;
        public Action onPointerDown;
        public Action onPointerUp;
        public Action onPointerEnter;

        Sequence selectedTween;

        private void Start()
        {
            Selected(false);

            selectedTween = DOTween.Sequence();
            selectedTween.Append(selected.transform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.2f).SetEase(Ease.OutBack));
            selectedTween.Append(selected.transform.DOScale(new Vector3(1.1f, 0.9f, 1f), 0.2f).SetEase(Ease.OutBack));
            selectedTween.Append(selected.transform.DOScale(new Vector3(1f, 1f, 1f), 0.2f).SetEase(Ease.OutBack));
            selectedTween.SetAutoKill(false);
            selectedTween.Rewind();
        }

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
            icon.gameObject.SetActive(number > 0);
            numberText.text = number > 0 ? number.ToString() : string.Empty;
        }

        public void Selected(bool selected)
        {
            transform.localScale = selected ? Vector3.one * 0.8f : Vector3.one;
            this.selected.gameObject.SetActive(selected);

            if (selected)
                selectedTween.Restart();
        }
    }
}
