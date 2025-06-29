using GameE;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameE
{
    public class MapContentSpawner : MonoBehaviour
    {
        public GameLogic2 gameLogic;
        public MonsterSpawner monsterSpawner;

        public List<GameObject> mapList;
        public PlayerController playerController;

        public BaseLever portalPrefab;

        Stack<BaseLever> portalPool = new Stack<BaseLever>();
        List<BaseLever> portalSpawned = new List<BaseLever>();

        int nowMapId;

        public void MoveMap(int mapId, Action onCompleteLoad)
        {
            StartCoroutine(MoveMapCo(mapId, onCompleteLoad));
        }

        IEnumerator MoveMapCo(int mapId, Action onCompleteLoad)
        {
            monsterSpawner.DespawnAll();

            DespawnAllGridMap();
            DespawnAllPortals();
            DisActivePlayer();

            yield return new WaitForFixedUpdate();

            SetGridMap(mapId);
            SetPortals(mapId);

            yield return new WaitForFixedUpdate();

            SetPlayer(mapId, nowMapId);

            onCompleteLoad?.Invoke();

            nowMapId = mapId;
        }

        void DespawnAllGridMap()
        {
            foreach (var map in mapList)
                map.gameObject.SetActive(false);
        }

        void SetGridMap(int mapId)
        {
            mapList[mapId].gameObject.SetActive(true);
        }

        void SetPortals(int mapId)
        {
            if (mapId == 1)
            {
                SpawnPortal(new Vector2(28f, 2f), 2);
            }
            else if (mapId == 2)
            {
                SpawnPortal(new Vector2(-28f, 2f), 1);
            }
        }

        void DisActivePlayer()
        {
            playerController.gameObject.SetActive(false);
        }

        void SetPlayer(int nowMapId, int prevMapId)
        {
            if (nowMapId == 1)
            {
                if (prevMapId == 2)
                    playerController.transform.position = new Vector2(26f, 2f);
                else
                    playerController.transform.position = new Vector2(0f, 2f);
            }
            else if (nowMapId == 2)
            {
                if (prevMapId == 1)
                    playerController.transform.position = new Vector2(-26f, 2f);
                else
                    playerController.transform.position = new Vector2(0f, 2f);
            }

            playerController.Respawn();
            playerController.gameObject.SetActive(true);
        }

        void SpawnPortal(Vector2 position, int moveMapId)
        {
            var portal = portalPool.Count > 0 ? portalPool.Pop() : Instantiate(portalPrefab);
            portalSpawned.Add(portal);

            portal.transform.position = position;
            portal.gameObject.SetActive(true);
            portal.SetTriggerEnter((col) =>
            {
                if (col.CompareTag("Player"))
                {
                    gameLogic.MoveMap(moveMapId);
                }   
            });
        }

        void DespawnAllPortals()
        {
            foreach (var portal in portalSpawned)
            {
                portal.gameObject.SetActive(false);
                portalPool.Push(portal);
            }

            portalSpawned.Clear();
        }
    }
}
