using System.Collections.Generic;
using UnityEngine;

namespace Utils.Pools
{
    public interface IPoolable
    {
        void Setup();
        void Reset();
    }

    public class ObjectPool<T> where T : MonoBehaviour, IPoolable
    {
        [SerializeField]
        private T prefab;

        private Queue<T> objects = new Queue<T>();

        public static ObjectPool<T> Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public T Get()
        {
            if (objects.Count == 0)
                AddObjects(1);
            T objectToReturn = objects.Dequeue();
            objectToReturn.gameObject.SetActive(true);
            objectToReturn.Setup();
            return objectToReturn;
        }

        public void ReturnToPool(T objectToReturn)
        {
            objectToReturn.Reset();
            objectToReturn.gameObject.SetActive(false);
            objects.Enqueue(objectToReturn);
        }

        private void AddObjects(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var newObject = Object.Instantiate(prefab);
                newObject.gameObject.SetActive(false);
                objects.Enqueue(newObject);
            }
        }

        public void SetPrefab(T newPrefab)
        {
            prefab = newPrefab;
        }
    }
}