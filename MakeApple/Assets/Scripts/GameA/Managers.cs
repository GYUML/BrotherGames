using UnityEngine;
using GameALogic;
using GameAUI;

namespace GameA
{
    public class Managers : MonoBehaviour
    {
        public static Managers Instance;

        public MainGameLogic mainGameLogic;
        public UIManager ui;

        public static MainGameLogic MainLogic { get { return Instance?.mainGameLogic; } }
        public static UIManager UI {  get { return Instance?.ui; } }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            var rowSize = 10;
            var colSize = 7;

            mainGameLogic.SetGameBoardCallBack((gameBoard) => ui.GetLayout<GameBoardLayout>().SetBoard(gameBoard));
            mainGameLogic.GenerateGameBoard(rowSize, colSize);
        }
    }
}

