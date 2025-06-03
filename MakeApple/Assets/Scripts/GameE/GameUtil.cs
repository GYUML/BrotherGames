using UnityEngine;
using UnityEngine.Tilemaps;

namespace GameE
{
    public class GameUtil
    {
        // 캐릭터 발밑의 지정된 영역 안에 있는 타일 개수를 세는 함수
        public static int GetTilesUnderneathCount(Tilemap tilemap, Vector3 worldPosition, int length)
        {
            var count = 0;
            var cellPosition = tilemap.WorldToCell(worldPosition);

            for (int y = -length; y <= 0; y++)
            {
                var currentCell = cellPosition + new Vector3Int(0, y, 0);
                if (tilemap.HasTile(currentCell))
                {
                    count++;
                }
            }

            return count;
        }
    }
}
