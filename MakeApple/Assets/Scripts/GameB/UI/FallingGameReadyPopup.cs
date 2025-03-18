using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameB
{
    public class FallingGameReadyPopup : UIPopup
    {
        public KButton startButton;

        private void Start()
        {
            startButton.onClick.AddListener(OnClickStart);
        }

        void OnClickStart()
        {
            Managers.GameLogic.StartGame();
            Managers.UI.HidePopup<FallingGameReadyPopup>();
        }
    }
}

