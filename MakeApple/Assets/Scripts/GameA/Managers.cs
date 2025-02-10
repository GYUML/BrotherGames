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

        public static MainGameLogic MainLogic { get { return Instance?.mainGameLogic; } }
        public static UIManager UI { get { return Instance?.ui; } }
        public static EventController Event { get { return Instance?.eventController; } } 
        private void Awake()
        {
            Instance = this;
        }
    }
}
