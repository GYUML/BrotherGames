using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameE
{
    public class VirtualKeyLayout : MonoBehaviour
    {
        public EventButton upKey;
        public EventButton downKey;
        public EventButton leftKey;
        public EventButton rightKey;
        public EventButton leftJumpKey;
        public EventButton rightJumpKey;
        public EventButton attackKey;

        public GameLogic2 gameLogic;
        public InputManager inputManager;
        public MapContentSpawner mapContentSpawner;

        public bool useKeyBoard;

        private void Start()
        {
#if UNITY_EDITOR
            useKeyBoard = true;
#endif

            leftKey.onDown.AddListener(() => inputManager.SetKeyDown(KeyType.MoveLeft, true));
            leftKey.onUp.AddListener(() => inputManager.SetKeyDown(KeyType.MoveLeft, false));
            rightKey.onDown.AddListener(() => inputManager.SetKeyDown(KeyType.MoveRight, true));
            rightKey.onUp.AddListener(() => inputManager.SetKeyDown(KeyType.MoveRight, false));
            upKey.onDown.AddListener(() =>
            {
                if (mapContentSpawner.IsOnPortal())
                    inputManager.SetKeyDown(KeyType.TakePortal, true);
                else
                    inputManager.SetKeyDown(KeyType.Jump, true);
            });
            upKey.onUp.AddListener(() =>
            {
                inputManager.SetKeyDown(KeyType.TakePortal, false);
                inputManager.SetKeyDown(KeyType.Jump, false);
            });
            downKey.onDown.AddListener(() => inputManager.SetKeyDown(KeyType.Down, true));
            downKey.onUp.AddListener(() => inputManager.SetKeyDown(KeyType.Down, false));
            leftJumpKey.onDown.AddListener(() => inputManager.SetKeyDown(KeyType.JumpLeft, true));
            leftJumpKey.onUp.AddListener(() => inputManager.SetKeyDown(KeyType.JumpLeft, false));
            rightJumpKey.onDown.AddListener(() => inputManager.SetKeyDown(KeyType.JumpRight, true));
            rightJumpKey.onUp.AddListener(() => inputManager.SetKeyDown(KeyType.JumpRight, false));
            attackKey.onDown.AddListener(() => inputManager.SetKeyDown(KeyType.Attack, true));
            attackKey.onUp.AddListener(() => inputManager.SetKeyDown(KeyType.Attack, false));
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (useKeyBoard)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    inputManager.SetKeyDown(KeyType.MoveLeft, true);
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                    inputManager.SetKeyDown(KeyType.MoveLeft, false);
                if (Input.GetKeyDown(KeyCode.RightArrow))
                    inputManager.SetKeyDown(KeyType.MoveRight, true);
                if (Input.GetKeyUp(KeyCode.RightArrow))
                    inputManager.SetKeyDown(KeyType.MoveRight, false);
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                    inputManager.SetKeyDown(KeyType.Jump, true);
                if (Input.GetKeyUp(KeyCode.LeftAlt))
                    inputManager.SetKeyDown(KeyType.Jump, false);
                if (Input.GetKeyDown(KeyCode.LeftShift))
                    inputManager.SetKeyDown(KeyType.Attack, true);
                if (Input.GetKeyUp(KeyCode.LeftShift))
                    inputManager.SetKeyDown(KeyType.Attack, false);
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    inputManager.SetKeyDown(KeyType.TakePortal, true);
                if (Input.GetKeyUp(KeyCode.UpArrow))
                    inputManager.SetKeyDown(KeyType.TakePortal, false);
            }
#endif
        }
    }
}
