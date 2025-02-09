using UnityEngine;
using GameALogic;
using GameAUI;

namespace GameA
{
    public class Managers : MonoBehaviour
    {
        public static Managers Instance;

        public GameBoardLayout gameBoardLayout;
        public MainGameLogic mainGameLogic;

        public static MainGameLogic mainLogic { get { return Instance?.mainGameLogic; } }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            var rowSize = 10;
            var colSize = 7;

            mainGameLogic.SetGameBoardCallBack((gameBoard) => gameBoardLayout.SetBoard(gameBoard));
            mainGameLogic.GenerateGameBoard(rowSize, colSize);
        }
    }
}

