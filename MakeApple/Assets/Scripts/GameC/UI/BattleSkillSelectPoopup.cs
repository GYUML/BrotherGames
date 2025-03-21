using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameB
{
    public class BattleSkillSelectPopup : UIPopup
    {
        public List<KButton> selectButtons = new List<KButton>();
        public List<TMP_Text> selectTexts = new List<TMP_Text>();

        Action<int> onSelect;

        private void Start()
        {
            for (int i = 0; i < selectButtons.Count; i++)
            {
                var indexTmp = i;
                selectButtons[i].onClick.AddListener(() =>
                {
                    onSelect?.Invoke(indexTmp);
                    Managers.UI.HidePopup<BattleSkillSelectPopup>();
                });
            }
        }

        public void SetOnSelect(Action<int> onSelect)
        {
            this.onSelect = onSelect;
        }

        public void Set(List<string> selectItems)
        {
            for (int i = 0; i < selectItems.Count; i++)
            {
                if (i < selectTexts.Count)
                    selectTexts[i].text = selectItems[i];
            }
        }
    }
}

