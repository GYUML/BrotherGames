using UnityEngine;
using GameALogic;

namespace GameA
{
    public class Managers : MonoBehaviour
    {
        public static Managers Instance;

        public MainGameLogic mainGameLogic;
        public AdventureGameLogic adventureGameLogic;
        public UIManager ui;
        public EventController eventController;
        public FileManager fileManager;
        public WebController webController;
        public UserDataController userData;

        public static MainGameLogic MainLogic { get { return Instance?.mainGameLogic; } }
        public static AdventureGameLogic AdventureLogic { get { return Instance?.adventureGameLogic; } }
        public static UIManager UI { get { return Instance?.ui; } }
        public static EventController Event { get { return Instance?.eventController; } } 
        public static FileManager File { get { return Instance?.fileManager; } }
        public static WebController Web { get { return Instance?.webController; } }
        public static UserDataController UserData { get { return Instance?.userData; } }

        private void Awake()
        {
            Instance = this;
        }
    }
}
