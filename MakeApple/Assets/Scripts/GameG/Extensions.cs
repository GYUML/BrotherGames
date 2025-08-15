using UnityEngine;

namespace GameG
{
    public static class Extensions
    {
        public static int[,] DeepCopy(this int[,] array)
        {
            var result = new int[array.GetLength(0), array.GetLength(1)];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    result[i, j] = array[i, j];
                }
            }

            return result;
        }

        public static string GetString(this int[,] array)
        {
            var result = "";

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    result += array[i, j] + " ";
                }
                result += "\n";
            }

            return result;
        }

        public static string GetString(this TilePuzzle puzzle)
        {
            var result = "";
            var board = puzzle.GetBoardState();
            var now = puzzle.GetNowPosition();

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (now.x == i && now.y == j)
                        result += "* ";
                    else
                        result += board[i, j] + " ";
                }
                result += "\n";
            }

            return result;
        }

        public static void SetAllZero(this int[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = 0;
                }
            }
        }

        public static void SetAllFalse(this bool[,] array)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = false;
                }
            }
        }

        public static bool ValidPosition(this int[,] array, Vector2Int pos)
        {
            return pos.x >= 0 || pos.x < array.GetLength(0) || pos.y >= 0 || pos.y < array.GetLength(1);
        }
    }
}
