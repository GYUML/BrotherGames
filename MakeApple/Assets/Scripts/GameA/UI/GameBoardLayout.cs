using DG.Tweening;
using GameA;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameAUI
{
    public class GameBoardLayout : UILayout
    {
        public GameBoardItem itemPrefab;
        public TMP_Text scoreText;
        public RectTransform timeGuage;
        public RectTransform acquireEffect;
        public GameObject timerIcon;

        List<GameBoardItem> itemPool = new List<GameBoardItem>();
        Stack<RectTransform> effectPool = new Stack<RectTransform>();

        int rowSize;
        int columnSize;
        Vector2Int startPoint;
        Vector2Int endPoint;

        Coroutine timerCo;
        Sequence timerIconSequence;

        private void Start()
        {
            itemPrefab.gameObject.SetActive(false);
            acquireEffect.gameObject.SetActive(false);

            timerIconSequence = DOTween.Sequence();
            timerIconSequence.Append(timerIcon.transform.DORotate(new Vector3(0f, 0f, -45f), 0.1f));
            timerIconSequence.Append(timerIcon.transform.DORotate(new Vector3(0f, 0f, 45f), 0.2f));
            timerIconSequence.Append(timerIcon.transform.DORotate(new Vector3(0f, 0f, 0f), 0.1f));
            timerIconSequence.Insert(0f, timerIcon.transform.DOScale(Vector3.one * 1.3f, 0.2f));
            timerIconSequence.Insert(0.2f, timerIcon.transform.DOScale(Vector3.one, 0.2f));
            timerIconSequence.AppendInterval(0.6f);
            timerIconSequence.SetLoops(-1);
            timerIconSequence.Pause();
        }

        public void SetBoard(int[,] gameBoard)
        {
            this.rowSize = gameBoard.GetLength(0);
            this.columnSize = gameBoard.GetLength(1);

            foreach (var item in itemPool)
                item.gameObject.SetActive(false);

            for (int i = itemPool.Count; i < rowSize * columnSize; i++)
            {
                var instantiated = Instantiate(itemPrefab, itemPrefab.transform.parent);
                itemPool.Add(instantiated);
                instantiated.gameObject.SetActive(false);
            }

            var boardCounter = 0;
            startPoint = new Vector2Int(-1, -1);

            for (int i = 0; i < rowSize; i++)
            {
                for (int j = 0; j < columnSize; j++)
                {
                    var item = itemPool[boardCounter++];
                    var rowIndex = i;
                    var columnIndex = j;
                    item.gameObject.SetActive(true);
                    item.SetNumber(gameBoard[i, j]);
                    item.SetEvent(() => OnItemPointerDown(rowIndex, columnIndex), () => OnItemPointerUp(rowIndex, columnIndex), () => OnItemPointerEnter(rowIndex, columnIndex));
                }
            }
        }

        public void StartTimer(float maxRemainTime, float nowRemainTime)
        {
            if (timerCo != null)
                StopCoroutine(timerCo);
            timerCo = StartCoroutine(TimerCo(maxRemainTime, nowRemainTime));
        }

        public void StopGame()
        {
            timerIconSequence.Rewind();
            UnSelectAllItems();
        }

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();
        }

        public void ShowAcquireEffect(int row, int column)
        {
            var effect = effectPool.Count > 0 ? effectPool.Pop() : Instantiate(acquireEffect, acquireEffect.transform.parent);
            var itemIndex = row * columnSize + column;
            var tweenTime = 1f;

            if (itemIndex < itemPool.Count)
            {
                var startPos = itemPool[itemIndex].transform.position;
                var endPos = scoreText.transform.position;
                var canvasGroup = effect.GetComponent<CanvasGroup>();

                effect.gameObject.SetActive(true);
                effect.transform.position = startPos;
                effect.transform.localScale = Vector3.one;
                effect.DOMove(endPos, tweenTime).OnComplete(() =>
                {
                    effect.gameObject.SetActive(false);
                    effectPool.Push(effect);
                });
                effect.DOScale(Vector2.one * 0.5f, tweenTime);
                canvasGroup.alpha = 1f;
                canvasGroup.DOFade(0f, tweenTime).SetEase(Ease.OutQuad);
            }
        }

        void SelectItem(int row, int column, bool select)
        {
            var item = itemPool[row * columnSize + column];
            item.Selected(select);
        }

        void SelectItems(Vector2Int start, Vector2Int end)
        {
            UnSelectAllItems();

            var minPoint = new Vector2Int(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y));
            var maxPoint = new Vector2Int(Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y));

            for (int i = minPoint.x; i <= maxPoint.x; i++)
                for (int j = minPoint.y; j <= maxPoint.y; j++)
                    SelectItem(i, j, true);
        }

        void UnSelectAllItems()
        {
            foreach (var item in itemPool)
                item.Selected(false);
        }

        IEnumerator TimerCo(float maxRemainTime, float nowRemainTime)
        {
            var timer = nowRemainTime;
            var startedShakeTimeIcon = false;
            timerIconSequence.Rewind();

            while (timer > 0)
            {
                timeGuage.localScale = new Vector2(timer / maxRemainTime, 1f);
                if (!startedShakeTimeIcon && timer < 10f)
                {
                    startedShakeTimeIcon = true;
                    timerIconSequence.Restart();
                }
                yield return null;
                timer -= Time.deltaTime;
            }
        }

        void OnItemPointerDown(int row, int column)
        {
            startPoint = new Vector2Int(row, column);
            SelectItem(row, column, true);
        }

        void OnItemPointerUp(int row, int column)
        {
            if (startPoint.x < 0 || startPoint.y < 0)
            {
                Debug.LogError($"OnItemPointerUp() row={row}, col={column}");
            }
            else
            {
                Managers.Event.DragEnd(startPoint, endPoint);
            }

            startPoint = new Vector2Int(-1, -1);
            UnSelectAllItems();
        }

        void OnItemPointerEnter(int row, int column)
        {
            if (startPoint.x < 0 || startPoint.y < 0)
                return;

            endPoint = new Vector2Int(row, column);
            SelectItems(startPoint, endPoint);
        }
    }
}
