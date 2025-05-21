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

        public bool useKeyBoard;

        private void Start()
        {
#if UNITY_EDITOR
            useKeyBoard = true;
#endif

            leftKey.onDown.AddListener(() => gameLogic.SetKeyDown(KeyType.MoveLeft, true));
            leftKey.onUp.AddListener(() => gameLogic.SetKeyDown(KeyType.MoveLeft, false));
            rightKey.onDown.AddListener(() => gameLogic.SetKeyDown(KeyType.MoveRight, true));
            rightKey.onUp.AddListener(() => gameLogic.SetKeyDown(KeyType.MoveRight, false));
            upKey.onDown.AddListener(() => gameLogic.SetKeyDown(KeyType.Jump, true));
            upKey.onUp.AddListener(() => gameLogic.SetKeyDown(KeyType.Jump, false));
            downKey.onDown.AddListener(() => gameLogic.SetKeyDown(KeyType.Down, true));
            downKey.onUp.AddListener(() => gameLogic.SetKeyDown(KeyType.Down, false));
            leftJumpKey.onDown.AddListener(() => gameLogic.SetKeyDown(KeyType.JumpLeft, true));
            leftJumpKey.onUp.AddListener(() => gameLogic.SetKeyDown(KeyType.JumpLeft, false));
            rightJumpKey.onDown.AddListener(() => gameLogic.SetKeyDown(KeyType.JumpRight, true));
            rightJumpKey.onUp.AddListener(() => gameLogic.SetKeyDown(KeyType.JumpRight, false));
            attackKey.onDown.AddListener(() => gameLogic.SetKeyDown(KeyType.Attack, true));
            attackKey.onUp.AddListener(() => gameLogic.SetKeyDown(KeyType.Attack, false));
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (useKeyBoard)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    gameLogic.SetKeyDown(KeyType.MoveLeft, true);
                if (Input.GetKeyUp(KeyCode.LeftArrow))
                    gameLogic.SetKeyDown(KeyType.MoveLeft, false);
                if (Input.GetKeyDown(KeyCode.RightArrow))
                    gameLogic.SetKeyDown(KeyType.MoveRight, true);
                if (Input.GetKeyUp(KeyCode.RightArrow))
                    gameLogic.SetKeyDown(KeyType.MoveRight, false);
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    if (Input.GetKey(KeyCode.LeftArrow))
                        gameLogic.SetKeyDown(KeyType.JumpLeft, true);
                    else if (Input.GetKey(KeyCode.RightArrow))
                        gameLogic.SetKeyDown(KeyType.JumpRight, true);
                    else
                        gameLogic.SetKeyDown(KeyType.Jump, true);
                }
                if (Input.GetKeyUp(KeyCode.LeftAlt))
                {
                    gameLogic.SetKeyDown(KeyType.Jump, false);
                    gameLogic.SetKeyDown(KeyType.JumpLeft, false);
                    gameLogic.SetKeyDown(KeyType.JumpRight, false);
                }
                if (Input.GetKeyDown(KeyCode.LeftShift))
                    gameLogic.SetKeyDown(KeyType.Attack, true);
                if (Input.GetKeyUp(KeyCode.LeftShift))
                    gameLogic.SetKeyDown(KeyType.Attack, false);
            }
#endif
        }
    }
}
