using Mono.Cecil.Cil;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameB
{
    public class FallingGameLogic : MonoBehaviour
    {
        public GameObject itemPrefab;
        public GameObject newPrefab;

        public PlayerController player;
        public EndGameScene endGameScene;

        public float fallingSpeed;
        public Vector2 minRange;
        public Vector2 maxRange;
        public float startHeight;

        public double spawnGemDistance;
        public double spawnObstacleDistance;

        List<GameObject> fallingItemList = new List<GameObject>();
        Stack<GameObject> fallingItemPool = new Stack<GameObject>();

        List<GameObject> newItemList = new List<GameObject>();
        Stack<GameObject> newItemPool = new Stack<GameObject>();

        Dictionary<string, Stack<GameObject>> prefabPool = new Dictionary<string, Stack<GameObject>>();

        double nowHeight;
        float nowFallingSpeed;
        float startTime;
        bool playingGame;

        int nowLevel;
        long nowAttack;
        long nowExp;

        double spawnGemCounter;
        double spawnObstacleCounter;
        int prevCoinPosIndex;

        float[] positionSet = new float[5] { -2f, -1f, 0f, 1f, 2f };

        private void Update()
        {
            if (playingGame)
            {
                if (nowHeight < spawnGemCounter)
                {
                    //SpawnItemGem(1, 10);
                    spawnGemCounter = nowHeight - spawnGemDistance;
                }

                if (nowHeight < spawnObstacleCounter)
                {
                    var count = Random.Range(3, 7);
                    //SpawnItemObstacle(2, count);
                    SpawnObstacleItemRandom();
                    spawnObstacleCounter = nowHeight - spawnObstacleDistance;
                }

                if (nowHeight <= 0.25f)
                {
                    var time = Time.time - startTime;
                    var score = startHeight / time * startHeight * nowAttack;

                    EndGame();
                }
                else
                {
                    nowHeight -= nowFallingSpeed * Time.deltaTime;
                    Managers.UI.GetLayout<FallingGameLayout>().SetHeight(nowHeight);
                    Managers.UI.GetLayout<FallingGameLayout>().SetBonusAttack(Managers.Table.GetBonusAttackPer(startHeight, fallingSpeed, Time.time - startTime));
                }
            }
        }

        public void SetGame()
        {
            playingGame = false;
            nowHeight = startHeight;
            nowLevel = 1;

            SetAttack(Managers.Table.GetAttackPerLevel(nowLevel));
            SetNowExp(0);

            spawnGemCounter = startHeight;
            spawnObstacleCounter = startHeight;
        }

        public void StartGame()
        {
            startTime = Time.time;
            playingGame = true;
        }

        public void SetFallingFactor(float factor)
        {
            factor = Mathf.Clamp01(factor);

            nowFallingSpeed = fallingSpeed * (1 + factor);
            foreach (var falling in fallingItemList)
                falling.GetComponent<FallingItem>().SetFallingSpeed(nowFallingSpeed);

            foreach (var falling in newItemList)
                falling.GetComponent<FallingObstacle>().SetFallingSpeed(nowFallingSpeed);

            Managers.UI.GetLayout<FallingGameLayout>().SetSpeed(nowFallingSpeed);
        }

        void SpawnRandomItem(int code)
        {
            var randomPosition = new Vector2(Random.Range(-2f, 2f), -6f);
            SpawnItem(code, randomPosition);
        }

        void SpawnItemGem(int code, int count)
        {
            var randomIndex = Random.Range(-2, 3);
            var nowPosXIndex = Mathf.Clamp(prevCoinPosIndex + randomIndex, 0, positionSet.Length - 1);
            var randomPosition = new Vector2(positionSet[prevCoinPosIndex], -6f);
            var interval = positionSet[nowPosXIndex] - positionSet[prevCoinPosIndex];

            for (int i = 0; i < count; i++)
                SpawnItem(code, new Vector2(randomPosition.x + interval / count * (i + 1), randomPosition.y - i * 0.5f));

            prevCoinPosIndex = nowPosXIndex;
        }

        void SpawnItemObstacle(int code, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var randomIndex = Random.Range(0, positionSet.Length);
                var randomPosition = new Vector2(0f, -6f);

                //while (randomIndex == prevCoinPosIndex)
                //    randomIndex = Random.Range(0, positionSet.Length);
                randomPosition.x = positionSet[randomIndex];

                SpawnItem(code, new Vector2(randomPosition.x, randomPosition.y - i * 0.5f));
            }
        }

        void SpawnItemDuel(int code)
        {
            SpawnItemDuel(code, new Vector2(-1.5f, -6f));
            SpawnItemDuel(code, new Vector2(1.5f, -6f));

        }

        void SpawnItemDuel(int code, Vector2 position)
        {
            var item = Instantiate(itemPrefab);
            item.transform.position = position;
            item.gameObject.SetActive(true);

            item.GetComponent<BoxCollider2D>().size = new Vector2(2.4f, 0.1f);
            item.GetComponent<BoxCollider2D>().isTrigger = true;
            item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            item.GetComponent<FallingItem>().Set(code, 2, minRange, maxRange, ReturnToPool, OnEnterPlayer);
            if (TryGetPrefab("GameB/Prefabs/FallingItems/SelectItem", out var prefab))
            {
                prefab.transform.parent = item.transform;
                prefab.transform.localPosition = Vector2.zero;
            }
        }

        void SpawnItem(int code, Vector2 position)
        {
            if (fallingItemPool.Count == 0)
            {
                var instantiated = Instantiate(itemPrefab);
                instantiated.gameObject.SetActive(false);
                fallingItemPool.Push(instantiated);
                fallingItemList.Add(instantiated);
            }

            var item = fallingItemPool.Pop();
            item.transform.position = position;
            item.gameObject.SetActive(true);

            if (code == 1)
            {
                item.GetComponent<BoxCollider2D>().size = new Vector2(0.2f, 0.2f);
                item.GetComponent<BoxCollider2D>().isTrigger = true;
                item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                item.GetComponent<FallingItem>().Set(code, fallingSpeed, minRange, maxRange, ReturnToPool, OnEnterPlayer);
                if (TryGetPrefab("GameB/Prefabs/FallingItems/Gem", out var prefab))
                {
                    prefab.transform.parent = item.transform;
                    prefab.transform.localPosition = Vector2.zero;
                }
            }
            else if (code == 2)
            {
                item.GetComponent<BoxCollider2D>().size = new Vector2(0.3f, 0.3f);
                item.GetComponent<BoxCollider2D>().isTrigger = false;
                item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                item.GetComponent<Rigidbody2D>().mass = 1f;
                item.GetComponent<FallingItem>().Set(code, fallingSpeed, minRange, maxRange, ReturnToPool, OnEnterPlayer);
                if (TryGetPrefab("GameB/Prefabs/FallingItems/Hammer", out var prefab))
                {
                    prefab.transform.parent = item.transform;
                    prefab.transform.localPosition = Vector2.zero;
                }
            }
            else if (code == 3)
            {
                item.GetComponent<BoxCollider2D>().size = new Vector2(2.4f, 0.1f);
                item.GetComponent<BoxCollider2D>().isTrigger = true;
                item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                item.GetComponent<FallingItem>().Set(code, fallingSpeed / 2f, minRange, maxRange, ReturnToPool, OnEnterPlayer);
                if (TryGetPrefab("GameB/Prefabs/FallingItems/SelectItem", out var prefab))
                {
                    prefab.transform.parent = item.transform;
                    prefab.transform.localPosition = Vector2.zero;
                }
            }
        }

        void SpawnObstacleItemRandom()
        {
            var randomPosition = new Vector2(Random.Range(-2f, 2f), -6f);
            SpawnObstacleItem(randomPosition);
        }

        void SpawnObstacleItem(Vector2 position)
        {
            if (newItemPool.Count == 0)
            {
                var instantiated = Instantiate(newPrefab);
                instantiated.gameObject.SetActive(false);
                newItemPool.Push(instantiated);
                newItemList.Add(instantiated);
            }

            var item = newItemPool.Pop();
            item.transform.position = position;
            item.gameObject.SetActive(true);

            item.GetComponent<FallingObstacle>().Set((success) =>
            {
                if (success)
                {
                    nowCoin++;
                    Managers.UI.GetLayout<FallingGameLayout>().SetCoin(nowCoin);
                }
            });

            if (TryGetPrefab("GameB/Prefabs/FallingItems/Hammer", out var prefab))
            {
                prefab.transform.parent = item.transform;
                prefab.transform.localPosition = Vector2.zero;
            }
        }

        int nowCoin;

        void ReturnToPool(GameObject item)
        {
            var child = item.transform.GetChild(0);
            if (child != null)
            {
                var childKey = child.GetComponent<Poolee>().Key;
                child.transform.SetParent(null);
                ReturnPrefabToPool(childKey, child.gameObject);
            }
            
            fallingItemPool.Push(item);
        }

        void OnEnterPlayer(int code)
        {
            if (code == 1)
            {
                SetNowExp(nowExp + 10);
            }
            //else if (code == 2)
            //    player.OnStun(1f);
        }

        void SetAttack(long attack)
        {
            nowAttack = attack;
            Managers.UI.GetLayout<FallingGameLayout>().SetAttack(attack);
        }

        void SetNowExp(long exp)
        {
            nowExp = exp;

            var maxExp = (nowLevel + 1) * 50;
            while (nowExp >= maxExp)
            {
                nowExp -= maxExp;
                nowLevel++;
                maxExp = (nowLevel + 1) * 10;
                SetAttack(Managers.Table.GetAttackPerLevel(nowLevel));
            }

            Managers.UI.GetLayout<FallingGameLayout>().SetExp(maxExp, nowExp);
        }

        bool TryGetPrefab(string path, out GameObject output)
        {
            if (!prefabPool.ContainsKey(path))
                prefabPool.Add(path, new Stack<GameObject>());

            try
            {
                if (prefabPool[path].Count == 0)
                {
                    var instantiated = Instantiate(Resources.Load<GameObject>(path));
                    instantiated.GetOrAddComponent<Poolee>().OnInstantiated(path);
                    output = instantiated;
                }
                else
                    output = prefabPool[path].Pop();

                output.gameObject.SetActive(true);

                return true;
            }
            catch
            {
                output = default;
                return false;
            }
        }

        void ReturnPrefabToPool(string path, GameObject target)
        {
            if (!prefabPool.ContainsKey(path))
                prefabPool.Add(path, new Stack<GameObject>());

            target.gameObject.SetActive(false);

            prefabPool[path].Push(target);
        }

        void EndGame()
        {
            var damage = (long)(nowAttack * (1 + Managers.Table.GetBonusAttackPer(startHeight, fallingSpeed, Time.time - startTime)));
            playingGame = false;
            ClearAll();
            player.gameObject.SetActive(false);
            endGameScene.gameObject.SetActive(true);
            endGameScene.Play(100, damage);
        }

        void ClearAll()
        {
            fallingItemPool.Clear();

            foreach (var item in fallingItemList)
            {
                fallingItemPool.Push(item);
                item.gameObject.SetActive(false);
            }
            fallingItemList.Clear();
        }
    }
}
