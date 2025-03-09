using UnityEngine;

namespace GameB
{
    public class Managers : MonoBehaviour
    {
        public static Managers Instance;

        public UIManager ui;
        public FileManager fileManager;
        public WebController webController;
        public FallingGameLogic gameLogic;
        public PageController pageController;

        public static UIManager UI { get { return Instance?.ui; } }
        public static FileManager File { get { return Instance?.fileManager; } }
        public static WebController Web { get { return Instance?.webController; } }
        public static FallingGameLogic GameLogic { get { return Instance?.gameLogic; } }
        public static PageController Page { get { return Instance?.pageController; } }

        private void Awake()
        {
            Instance = this;
        }
    }
}
