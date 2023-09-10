using System.Collections.Generic;

namespace Utils.Common
{
    class LRUCache<K, V>
    {
        private readonly int _capacity;
        private readonly Dictionary<K, LinkedListNode<CacheItem>> _cache;
        private readonly LinkedList<CacheItem> _lruList;

        public LRUCache(int capacity)
        {
            _capacity = capacity;
            _cache = new Dictionary<K, LinkedListNode<CacheItem>>(capacity);
            _lruList = new LinkedList<CacheItem>();
        }

        public bool Contains(K key)
        {
            return _cache.ContainsKey(key);
        }

        public void Add(K key, V value)
        {
            if (_cache.Count >= _capacity)
            {
                RemoveLast();
            }
            var cacheItem = new CacheItem { Key = key, Value = value };
            var node = new LinkedListNode<CacheItem>(cacheItem);
            _lruList.AddLast(node);
            _cache.Add(key, node);
        }

        public bool TryGet(K key, out V value)
        {
            if (_cache.TryGetValue(key, out var node))
            {
                var result = node.Value.Value;
                _lruList.Remove(node);
                _lruList.AddLast(node);
                value = result;
                return true;
            }
            value = default(V);
            return false;
        }

        private void RemoveLast()
        {
            var node = _lruList.First;
            _lruList.RemoveFirst();
            _cache.Remove(node.Value.Key);
        }

        private class CacheItem
        {
            public K Key { get; set; }
            public V Value { get; set; }
        }
    }
}