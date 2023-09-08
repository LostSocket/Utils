using System.Collections.Generic;
using UnityEngine;

namespace Utils.Pools
{
    public class PoolFactory
    {
        private readonly Dictionary<string, object> _pools = new Dictionary<string, object>();

        public ObjectPool<T> GetPool<T>(T prefab) where T : MonoBehaviour, IPoolable
        {
            var type = typeof(T).ToString();
            if (_pools.TryGetValue(type, out var pool)) return pool as ObjectPool<T>;
            var newPool = new ObjectPool<T>();
            newPool.SetPrefab(prefab);
            _pools[type] = newPool;
            return _pools[type] as ObjectPool<T>;
        }
    }
}