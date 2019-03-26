using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cytus2
{
    public class GameObjectPool<T>
        where T : IGameObjectPoolEntity
    {
        private Dictionary<string, List<T>> _allEntitiesMap = new Dictionary<string, List<T>>();
        private Dictionary<string, Stack<T>> _idleEntitiesMap = new Dictionary<string, Stack<T>>();
        private Dictionary<string, GameObject> _prefabMap = new Dictionary<string, GameObject>();

        public bool HasEntityPrefab(string name)
        {
            return _prefabMap.ContainsKey(name);
        }

        public void AddEntityPrefab(string name, GameObject prefab)
        {
            _prefabMap[name] = prefab ?? throw new Exception("Invalid prefab");
            _allEntitiesMap[name] = new List<T>();
            _idleEntitiesMap[name] = new Stack<T>();
        }

        public void AddEntityPrefab(GameObject prefab)
        {
            AddEntityPrefab("Default", prefab);
        }

        public T SpawnEntity(string name, Transform parent, bool inWorldSpace = true)
        {
            Stack<T> idleEntities = _idleEntitiesMap[name];
            T entity;
            if (idleEntities.Count > 0)
            {
                entity = idleEntities.Pop();
                entity.gameObject.transform.SetParent(parent, inWorldSpace);
                entity.gameObject.SetActive(true);
                return entity;
            }
            else
            {
                entity = GameObject.Instantiate(_prefabMap[name], parent, inWorldSpace).GetComponent<T>();
                _allEntitiesMap[name].Add(entity);
            }
            return entity;
        }

        public T SpawnEntity(Transform parent, bool inWorldSpace = true)
        {
            return SpawnEntity("Default", parent, inWorldSpace);
        }

        public void DespawnEntity(string name, T entity)
        {
            entity.gameObject.SetActive(false);
            _idleEntitiesMap[name].Push(entity);
        }

        public void DespawnEntity(T entity)
        {
            DespawnEntity("Default", entity);
        }

        public void DespawnAllEntities()
        {
            foreach (var idleEntities in _idleEntitiesMap.Values)
            {
                idleEntities.Clear();
            }
            foreach (var allEntities in _allEntitiesMap.Values)
            {
                foreach (var entity in allEntities)
                {
                    entity.Despawn();
                }
            }
        }

        public void ReleaseAllEntities()
        {
            foreach (var idleEntities in _idleEntitiesMap.Values)
            {
                idleEntities.Clear();
            }
            foreach (var allEntities in _allEntitiesMap.Values)
            {
                allEntities.Clear();
            }
        }

        public void Clear()
        {
            _idleEntitiesMap.Clear();
            _allEntitiesMap.Clear();
            _prefabMap.Clear();
        }
    }
}