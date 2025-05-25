using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameE
{
    public class GameLogic2 : MonoBehaviour
    {
        public HudLayout hudLayout;
        public StateLayout stateLayout;

        public PlayerController playerController;

        public EffectSpawner effectSpawner;
        public DropItemSpawner dropItemSpawner;
        public MonsterSpawner monsterSpawner;

        public Vector2 attackEffectOffset;
        public Vector2 jumpEffectOffset;

        public Vector2 attackRangeSize;
        public Vector2 attackRangeOffset;
        public int maxAttackTargetCount;

        float attackMotionEnd;
        float downJumpTimeLimit;

        HashSet<KeyType> keyPressed = new HashSet<KeyType>();
        HashSet<KeyType> keyJustPressed = new HashSet<KeyType>();

        public EnemyUnit monsterPrefab;
        public float respawnDelay;
        public int maxSpawnCount;

        float respawnTime;

        FieldData fieldData;
        PlayerData playerData;

        private void Start()
        {
            Application.targetFrameRate = 60;
            fieldData = new FieldData(OnSpawnMonster, OnAttackMonster, OnDeadMonster);
            playerData = new PlayerData();

            monsterPrefab.gameObject.SetActive(false);
            AddExp(0);
        }

        private void FixedUpdate()
        {
            if (Time.time > attackMotionEnd)
            {
                if (IsKeyPressed(KeyType.Attack))
                {
                    attackMotionEnd = Time.time + 0.4f;
                    playerController.Attack();
                    StartCoroutine(PlayerAttackCo(playerController.IsLeft()));
                }
                else
                {
                    var inputX = IsKeyPressed(KeyType.MoveLeft) ? -1f : IsKeyPressed(KeyType.MoveRight) ? 1f : 0f;
                    playerController.MoveX(inputX);

                    if (playerController.IsGrounded())
                    {
                        if (IsKeyJustPressed(KeyType.Down))
                        {
                            if (Time.fixedTime < downJumpTimeLimit)
                            {
                                playerController.DownJump();
                                downJumpTimeLimit = 0f;
                            }
                            else
                            {
                                downJumpTimeLimit = Time.fixedTime + 0.2f;
                            }
                        }
                        if (IsKeyPressed(KeyType.JumpLeft))
                            playerController.Jump(-1f);
                        else if (IsKeyPressed(KeyType.JumpRight))
                            playerController.Jump(1f);
                        else if (IsKeyPressed(KeyType.Jump))
                            playerController.Jump(0f);
                    }
                    else
                    {
                        if (IsKeyJustPressed(KeyType.Jump))
                        {
                            if (IsKeyPressed(KeyType.MoveLeft))
                                playerController.DoubleJump(-1f, effectSpawner.ShowJumpEffect);
                            else if (IsKeyPressed(KeyType.MoveRight))
                                playerController.DoubleJump(1f, effectSpawner.ShowJumpEffect);
                            else
                                playerController.DoubleJump(0, effectSpawner.ShowJumpEffect);
                        }
                        else if (IsKeyJustPressed(KeyType.JumpLeft))
                            playerController.DoubleJump(-1f, effectSpawner.ShowJumpEffect);
                        else if (IsKeyJustPressed(KeyType.JumpRight))
                            playerController.DoubleJump(1f, effectSpawner.ShowJumpEffect);
                    }
                }
            }

            keyJustPressed.Clear();
        }

        private void Update()
        {
            if (Time.time > respawnTime)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (fieldData.GetNowMonsterCount() < maxSpawnCount)
                    {
                        fieldData.SpawnMonster(1);
                    }
                }

                respawnTime = Time.time + respawnDelay;
            }
        }

        public void SetKeyDown(KeyType keyType, bool keyDown)
        {
            if (keyDown)
            {
                if (keyType == KeyType.MoveLeft)
                    keyPressed.Remove(KeyType.MoveRight);
                else if (keyType == KeyType.MoveRight)
                    keyPressed.Remove(KeyType.MoveLeft);
                else if (keyType == KeyType.JumpLeft)
                    keyPressed.Remove(KeyType.JumpRight);
                else if (keyType == KeyType.JumpRight)
                    keyPressed.Remove(KeyType.JumpLeft);

                keyPressed.Add(keyType);
                keyJustPressed.Add(keyType);
            }
            else
            {
                keyPressed.Remove(keyType);
            }
        }

        IEnumerator PlayerAttackCo(bool isLeft)
        {
            yield return new WaitForSeconds(0.2f);
            effectSpawner.ShowAttackEffect(playerController.transform.position, isLeft);

            var attackRangePosition = GetAttackRangePosition(playerController.transform.position, attackRangeOffset, isLeft);
            var hits = Physics2D.OverlapBoxAll(attackRangePosition, attackRangeSize, 0f, LayerMask.GetMask("Monster"));
            var attackTargetCounter = 0;
            foreach (var hit in hits)
            {
                if (attackTargetCounter < maxAttackTargetCount && hit.TryGetComponent<EnemyUnit>(out var monster))
                {
                    monster.OnAttacked(playerController.IsLeft());
                    AttackMonster(monster.Id);
                    attackTargetCounter++;
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            var attackRangePosition = GetAttackRangePosition(playerController.transform.position, attackRangeOffset, playerController.IsLeft());
            Gizmos.DrawWireCube(attackRangePosition, attackRangeSize);
        }

        Vector3 GetAttackRangePosition(Vector3 position, Vector2 offset, bool isLeft)
        {
            if (isLeft) offset.x = -offset.x;
            return position + (Vector3)offset;
        }

        bool IsKeyPressed(KeyType keyType)
        {
            return keyPressed.Contains(keyType);
        }

        bool IsKeyJustPressed(KeyType keyType)
        {
            return keyJustPressed.Contains(keyType);
        }

        void AttackMonster(int monsterId)
        {
            var damages = new long[3];
            for (int i = 0; i < damages.Length; i++)
                damages[i] = Random.Range(3, 18);
            fieldData.AttackMonster(monsterId, damages);
        }

        void OnSpawnMonster(int id, long maxHp, long nowHp)
        {
            monsterSpawner.Spawn(id);
        }

        void OnAttackMonster(int id, long maxHp, long nowHp, long[] damages)
        {
            if (monsterSpawner.TryGetMonster(id, out var monster))
            {
                hudLayout.UpdateHealthBar(monster.transform, maxHp, nowHp);
                hudLayout.SpawnDamageText(damages, monster.transform.position);
                effectSpawner.ShowHitEffect(monster.transform.position, damages.Length);
                monster.SetChaseTarget(playerController.transform);
            }
            else
            {
                Debug.LogError($"OnAttackedMonster() Failed to find Monster. id={id}");
            }
        }

        void OnDeadMonster(int id)
        {
            if (monsterSpawner.TryGetMonster(id, out var monster))
            {
                monsterSpawner.Despawn(id);
                dropItemSpawner.DropItem(monster.transform.position, 5, AcquireItem);

                AddExp(50);
            }
            else
            {
                Debug.LogError($"OnDeadMonster() Failed to find Monster. id={id}");
            }
        }

        void AcquireItem()
        {

        }

        void AddExp(long addExp)
        {
            var needExp = GetNeedExp(playerData.level);
            playerData.exp += addExp;

            while (playerData.exp >= needExp)
            {
                playerData.exp -= needExp;
                playerData.level++;
                needExp = GetNeedExp(playerData.level);

                effectSpawner.ShowLevelUpEffect(playerController.transform.position);
            }

            stateLayout.SetExpGuage(needExp, playerData.exp);
            stateLayout.SetLevel(playerData.level);
        }

        long GetNeedExp(int level)
        {
            return level * 100;
        }
    }

    public class FieldData
    {
        List<Monster> monsters = new List<Monster>();

        Action<int, long, long> onSpawnMonster;
        Action<int, long, long, long[]> onAttackMonster;
        Action<int> onDeadMonster;

        int lastId;

        public FieldData(Action<int, long, long> onSpawnMonster, Action<int, long, long, long[]> onAttackMonster, Action<int> onDeadMonster)
        {
            this.onSpawnMonster = onSpawnMonster;
            this.onAttackMonster = onAttackMonster;
            this.onDeadMonster = onDeadMonster;
        }

        public void SpawnMonster(int monsterCode)
        {
            var newMonster = new Monster(lastId++, new Stat() { hp = 100 });
            monsters.Add(newMonster);
            newMonster.Spawn();

            onSpawnMonster?.Invoke(newMonster.id, newMonster.originStat.hp, newMonster.nowStat.hp);
        }

        public void AttackMonster(int monsterId, long[] damages)
        {
            if (TryGetMonster(monsterId, out var monster))
            {
                foreach (var damage in damages)
                    monster.OnAttacked(damage);

                onAttackMonster?.Invoke(monsterId, monster.originStat.hp, monster.nowStat.hp, damages);

                if (!monster.IsAlive())
                {
                    monsters.Remove(monster);
                    onDeadMonster?.Invoke(monsterId);
                }
            }
        }

        public int GetNowMonsterCount()
        {
            return monsters.Count;
        }

        bool TryGetMonster(int id, out Monster monster)
        {
            foreach (var now in monsters)
            {
                if (now.id == id)
                {
                    monster = now;
                    return true;
                }
            }

            monster = null;
            return false;
        }
    }

    public class Monster
    {
        public int id;
        public Stat originStat { get; private set; }
        public Stat nowStat { get; private set; }

        public Monster(int id, Stat originStat)
        {
            this.id = id;
            this.originStat = originStat.Clone();
        }

        public void Spawn()
        {
            nowStat = originStat.Clone();
        }

        public void OnAttacked(long damage)
        {
            nowStat.hp -= damage;
        }

        public bool IsAlive()
        {
            return nowStat.hp > 0;
        }
    }

    public class Stat
    {
        public long hp;

        public Stat Clone()
        {
            return new Stat() { hp = this.hp };
        }
    }
    
    public class PlayerData
    {
        public int level;
        public long exp;

        public PlayerData()
        {
            level = 1;
        }
    }
    
    public enum KeyType
    {
        None,
        MoveLeft,
        MoveRight,
        Down,
        Jump,
        JumpLeft,
        JumpRight,
        Attack
    }
}
