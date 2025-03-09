using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections.Generic;
using UnityEngine;

namespace GameB
{
    public class TownPage : MonoBehaviour
    {
        public GameObject player;

        public List<GameObject> npcList = new List<GameObject>();
        TweenerCore<Vector3, Vector3, VectorOptions> moveTween;

        private void OnEnable()
        {
            Camera.main.GetComponent<CameraMover>().SetTarget(player.transform);
        }

        private void OnDisable()
        {
            Camera.main.GetComponent<CameraMover>().ResetCamera();
        }

        public void MoveToNpc(int npcIndex, bool left)
        {
            var destination = npcList[npcIndex].transform.position;

            if (left) destination.x = destination.x - 0.5f;
            else destination.x = destination.x + 0.5f;

            if (destination.x < player.transform.position.x)
                player.transform.eulerAngles = Vector3.zero;
            else if (destination.x > player.transform.position.x)
                player.transform.eulerAngles = new Vector3(0f, 180f, 0f);

            if (moveTween != null)
                moveTween.Kill();

            moveTween = player.transform.DOMove(destination, 1f)
                .OnComplete(() =>
                {
                    if (player.transform.position.x < npcList[npcIndex].transform.position.x)
                        player.transform.eulerAngles = new Vector3(0f, 180f, 0f);
                    else
                        player.transform.eulerAngles = Vector3.zero;
                });
        }
    }
}
