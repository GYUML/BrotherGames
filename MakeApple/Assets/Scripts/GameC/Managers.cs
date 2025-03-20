using UnityEngine;

namespace GameC
{
    public class Managers : MonoBehaviour
    {
        public static Managers Instance;

        public UIManager ui;
        public FileManager fileManager;
        public WebController webController;
        public PageController pageController;
        public GameObject logics;

        public static UIManager UI { get { return Instance?.ui; } }
        public static FileManager File { get { return Instance?.fileManager; } }
        public static WebController Web { get { return Instance?.webController; } }
        public static PageController Page { get { return Instance?.pageController; } }
        public static GameObject Logic { get { return Instance?.logics; } }

        private void Awake()
        {
            Instance = this;
        }
    }
}
