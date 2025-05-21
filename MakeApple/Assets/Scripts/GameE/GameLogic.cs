using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameE
{
    public class GameLogic : MonoBehaviour
    {
        public GameObject itemPrefab;
        public Transform itemParent;
        public KButton spawnButton;
        public TMP_Text moneyInfo;

        List<GameObject> items = new List<GameObject>();
        List<int> levels = new List<int>();

        double money = 3000;
        double div = 0;

        private void Start()
        {
            spawnButton.onClick.AddListener(SpawnItem);
            StartCoroutine(GetMoneyCo());
        }

        IEnumerator GetMoneyCo()
        {
            while (true)
            {
                yield return new WaitForSeconds(10f);
                money += div;
                UpdateMoneyInfo();
            }
        }

        void SpawnItem()
        {
            if (money < 500)
                return;
            else
                money -= 500;

            var newItem = Instantiate(itemPrefab, itemParent);
            var newId = levels.Count;

            items.Add(newItem);
            newItem.GetComponent<KButton>().onClick.AddListener(() => TryUpgradeLevel(newId));
            newItem.SetActive(true);
            levels.Add(0);

            UpdateMoneyInfo();
        }

        void TryUpgradeLevel(int id)
        {
            var needMoney = Mathf.Pow(2, levels[id]) * 1000;
            var rand = Random.Range(0f, 1f);
            var isSuccess = rand < 0.5f;

            if (money < needMoney)
                return;
            else
                money -= needMoney;

            if (isSuccess)
                levels[id]++;
            else
                levels[id] = 0;

            SetLevel(id, levels[id]);
            UpdateMoneyInfo();
        }

        void SetLevel(int id, int level)
        {
            items[id].transform.Find("Text").GetComponent<TMP_Text>().text = level.ToString();
        }

        void UpdateMoneyInfo()
        {
            div = 0;

            foreach (var level in levels)
            {
                var price = Mathf.Pow(2, level + 1) * 100;
                div += (long)price;
            }

            moneyInfo.text = $"Money : {money}\ndiv : {div}/10s";
        }
    }
}
