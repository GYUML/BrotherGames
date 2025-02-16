using UnityEngine;
using GameALogic;

namespace GameA
{
    public class Managers : MonoBehaviour
    {
        public static Managers Instance;

        public MainGameLogic mainGameLogic;
        public UIManager ui;
        public EventController eventController;
        public FileManager fileManager;
        public WebController webController;

        public static MainGameLogic MainLogic { get { return Instance?.mainGameLogic; } }
        public static UIManager UI { get { return Instance?.ui; } }
        public static EventController Event { get { return Instance?.eventController; } } 
        public static FileManager File { get { return Instance?.fileManager; } }
        public static WebController Web { get { return Instance?.webController; } }

        private void Awake()
        {
            Instance = this;
        }
    }
}
