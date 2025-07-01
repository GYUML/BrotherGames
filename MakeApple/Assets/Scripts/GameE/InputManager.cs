using System.Collections.Generic;
using UnityEngine;

namespace GameE
{
    public class InputManager : MonoBehaviour
    {
        public GameLogic2 gameLogic;
        public PlayerController playerController;
        public EffectSpawner effectSpawner;
        public MapContentSpawner mapContentSpawner;

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
                    playerController.MoveX(inputX);

                    if (playerController.IsGrounded())
                    {
                        if (IsKeyJustPressed(KeyType.Down))
                            playerController.DownJump();

                        if (IsKeyJustPressed(KeyType.TakePortal))
                            mapContentSpawner.TakePortal();

                        if (IsKeyPressed(KeyType.Jump))
                        {
                            if (IsKeyPressed(KeyType.MoveLeft))
                                playerController.Jump(-1f);
                            else if (IsKeyPressed(KeyType.MoveRight))
                                playerController.Jump(1f);
                            else
                                playerController.Jump(0f);
                        }
                        else if (IsKeyPressed(KeyType.JumpLeft))
                            playerController.Jump(-1f);
                        else if (IsKeyPressed(KeyType.JumpRight))
                            playerController.Jump(1f);
                    }
                    else
                    {
                        if (IsKeyJustPressed(KeyType.Jump))
                        {
                            if (IsKeyPressed(KeyType.MoveLeft))
                                playerController.DoubleJump(-1f, effectSpawner.ShowJumpEffect);
                            else if (IsKeyPressed(KeyType.MoveRight))
                                playerController.DoubleJump(1f, effectSpawner.ShowJumpEffect);
                            else
                                playerController.DoubleJump(0f, effectSpawner.ShowJumpEffect);
                        }
                        else if (IsKeyJustPressed(KeyType.JumpLeft))
                            playerController.DoubleJump(-1f, effectSpawner.ShowJumpEffect);
                        else if (IsKeyJustPressed(KeyType.JumpRight))
                            playerController.DoubleJump(1f, effectSpawner.ShowJumpEffect);
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

        public bool IsKeyPressed(KeyType keyType)
        {
            return keyPressed.Contains(keyType);
        }

        public bool IsKeyJustPressed(KeyType keyType)
        {
            return keyJustPressed.Contains(keyType);
        }
    }
}
