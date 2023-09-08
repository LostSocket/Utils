using UnityEngine;

namespace Utils.Math
{
    public class Maths
    {
        public static Vector2 RVec2(float minX, float maxX, float minY, float maxY)
        {
            return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
        }
        
        public static Vector3 RVec3(float minX, float maxX, float minY, float maxY, float minZ, float maxZ)
        {
            return new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
        }

        public static Color RCol(bool includeAlpha = false)
        {
            return includeAlpha ? new Color(Random.value, Random.value, Random.value, Random.value) : new Color(Random.value, Random.value, Random.value);
        }
    }
}