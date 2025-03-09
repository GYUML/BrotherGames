using UnityEngine;

namespace GameB
{
    public class TownLayout : UILayout
    {
        public KButton shopButton;
        public KButton bagButton;
        public KButton battleButton;
        public KButton skillButton;
        public KButton researchButton;

        public GameObject shopTab;
        public GameObject bagTab;
        public GameObject battleTab;
        public GameObject skillTab;
        public GameObject researchTab;

        public KButton battleTabStartButton;

        public TownPage townPage;

        private void Start()
        {
            shopButton.onClick.AddListener(OnClickShop);
            bagButton.onClick.AddListener(OnClickBag);
            battleButton.onClick.AddListener(OnClickBattle);

            OnClickBattle();
        }

        void OnClickShop()
        {
            HideAllTab();
            shopTab.SetActive(true);
            townPage.MoveToNpc(0, false);
        }

        void OnClickBag()
        {
            HideAllTab();
            bagTab.SetActive(true);
            townPage.MoveToNpc(1, true);
        }

        void OnClickBattle()
        {
            HideAllTab();
            battleTab.SetActive(true);
            townPage.MoveToNpc(2, true);
        }

        void HideAllTab()
        {
            shopTab.SetActive(false);
            bagTab.SetActive(false);
            battleTab.SetActive(false);
            skillTab.SetActive(false);
            researchTab.SetActive(false);
        }
    }
}
