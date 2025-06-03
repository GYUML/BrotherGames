using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameE
{
    public class InputLogic : MonoBehaviour
    {
        HashSet<KeyType> keyPressed = new HashSet<KeyType>();
        HashSet<KeyType> keyJustPressed = new HashSet<KeyType>();

        Action onFixedUpdate;
        Action onUpdate;

        private void FixedUpdate()
        {
            onFixedUpdate?.Invoke();

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
