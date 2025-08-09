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
    }
}
