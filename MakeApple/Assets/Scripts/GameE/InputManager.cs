using System.Collections.Generic;
using UnityEngine;


namespace GameE
{
    public class InputManager : MonoBehaviour
    {
        public GameLogic2 gameLogic;

        HashSet<KeyType> keyPressed = new HashSet<KeyType>();
        HashSet<KeyType> keyJustPressed = new HashSet<KeyType>();

        private void FixedUpdate()
        {
            if (gameLogic.IsPlayerAttackMotionEnd())
            {
                if (IsKeyPressed(KeyType.Attack))
                {
                    gameLogic.PlayerAttack();
                }
                else
                {
                    var inputX = IsKeyPressed(KeyType.MoveLeft) ? -1f : IsKeyPressed(KeyType.MoveRight) ? 1f : 0f;
                    gameLogic.PlayerMove(inputX);

                    if (gameLogic.IsPlayerGrounded())
                    {
                        if (IsKeyJustPressed(KeyType.Down))
                            gameLogic.PlayerDownJump();

                        if (IsKeyPressed(KeyType.Jump))
                        {
                            if (IsKeyPressed(KeyType.MoveLeft))
                                gameLogic.PlayerJump(-1f);
                            else if (IsKeyPressed(KeyType.MoveRight))
                                gameLogic.PlayerJump(1f);
                            else
                                gameLogic.PlayerJump(0f);
                        }
                        else if (IsKeyPressed(KeyType.JumpLeft))
                            gameLogic.PlayerJump(-1f);
                        else if (IsKeyPressed(KeyType.JumpRight))
                            gameLogic.PlayerJump(1f);
                    }
                    else
                    {
                        if (IsKeyJustPressed(KeyType.Jump))
                        {
                            if (IsKeyPressed(KeyType.MoveLeft))
                                gameLogic.PlayerDoubleJump(-1f);
                            else if (IsKeyPressed(KeyType.MoveRight))
                                gameLogic.PlayerDoubleJump(1f);
                            else
                                gameLogic.PlayerDoubleJump(0f);
                        }
                        else if (IsKeyJustPressed(KeyType.JumpLeft))
                            gameLogic.PlayerDoubleJump(-1f);
                        else if (IsKeyJustPressed(KeyType.JumpRight))
                            gameLogic.PlayerDoubleJump(1f);
                    }
                }
            }

            keyJustPressed.Clear();
        }

        public void SetKeyDown(KeyType keyType, bool keyDown)
        {
            if (keyDown)
            {
                if (keyType == KeyType.MoveLeft)
                    keyPressed.Remove(KeyType.MoveRight);
                else if (keyType == KeyType.MoveRight)
                    keyPressed.Remove(KeyType.MoveLeft);
                else if (keyType == KeyType.JumpLeft)
                    keyPressed.Remove(KeyType.JumpRight);
                else if (keyType == KeyType.JumpRight)
                    keyPressed.Remove(KeyType.JumpLeft);

                keyPressed.Add(keyType);
                keyJustPressed.Add(keyType);
            }
            else
            {
                keyPressed.Remove(keyType);
            }
        }

        bool IsKeyPressed(KeyType keyType)
        {
            return keyPressed.Contains(keyType);
        }

        bool IsKeyJustPressed(KeyType keyType)
        {
            return keyJustPressed.Contains(keyType);
        }
    }
}
