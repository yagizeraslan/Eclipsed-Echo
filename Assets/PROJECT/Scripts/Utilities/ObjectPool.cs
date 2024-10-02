using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.EclipsedEcho
{
    public class ObjectPool<T> where T : Component
    {
        private List<T> poolList = new List<T>(); // Store all pooled objects
        private GameObject prefab;
        private Transform parent;

        private int currentIndex = 0; // Track the index of the current object being used

        // Constructor to initialize the pool with a prefab and parent transform (optional)
        public ObjectPool(GameObject prefab, int initialPoolSize, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;

            for (int i = 0; i < initialPoolSize; i++)
            {
                T instance = CreateNewObject();
                instance.gameObject.SetActive(false);
                poolList.Add(instance); // Add to pool list
            }
        }

        // Get an object from the pool
        public T GetObjectFromPool()
        {
            if (currentIndex >= poolList.Count)
            {
                Debug.LogWarning("Pool size exceeded, instantiating new object.");
                T newObj = CreateNewObject();
                poolList.Add(newObj);
                return newObj;
            }

            T obj = poolList[currentIndex];
            obj.gameObject.SetActive(true);
            currentIndex++;
            return obj;
        }

        // Return an object back to the pool (just deactivate it)
        public void ReturnObjectToPool(T obj)
        {
            obj.gameObject.SetActive(false);
        }

        // Create a new object and add it to the pool
        private T CreateNewObject()
        {
            GameObject newObj = Object.Instantiate(prefab, parent);
            return newObj.GetComponent<T>();
        }

        // Reset the pool index for a new game so it starts from the first object
        public void ResetPoolIndex()
        {
            currentIndex = 0;
        }

        // Optional: Clear the pool entirely
        public void ClearPool()
        {
            foreach (var obj in poolList)
            {
                Object.Destroy(obj.gameObject);
            }
            poolList.Clear();
            currentIndex = 0;
        }
    }
}
