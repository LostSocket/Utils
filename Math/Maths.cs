using System.Collections.Generic;
using UnityEngine;

namespace Utils.Math
{
    public static class Maths
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
    
        public static Vector3 V2FromNum(float x)
        {
            return new Vector2(x, x);
        }
        
        public static Vector3 V3FromNum(float x)
        {
            return new Vector3(x, x, x);
        }
   
        
        
    }
    
    public static class ExtensionMethods
    {

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}