using System.Collections.Generic;
using UnityEngine;

namespace Cytus2
{
    public class GameObjectPool<T>
        where T : IGameObjectPoolEntity
    {
        private List<T> _allEntities = new List<T>();
        private Stack<T> _idleEntities = new Stack<T>();
        private GameObject _prefab;

        public GameObjectPool(GameObject prefab)
        {
            _prefab = prefab;
        }

        public T SpawnEntity(Transform parent, bool inWorldSpace)
        {
            T entity;
            if (_idleEntities.Count > 0)
            {
                entity = _idleEntities.Pop();
                entity.gameObject.transform.SetParent(parent, inWorldSpace);
                entity.gameObject.SetActive(true);
                return entity;
            }
            else
            {
                entity = GameObject.Instantiate(_prefab, parent, inWorldSpace).GetComponent<T>();
                _allEntities.Add(entity);
            }
            return entity;
        }

        public void DespawnEntity(T entity)
        {
            entity.gameObject.SetActive(false);
            _idleEntities.Push(entity);
        }

        public void DespawnAllEntities()
        {
            _idleEntities.Clear();
            foreach (var entity in _allEntities)
            {
                entity.Despawn();
            }
        }
    }
}