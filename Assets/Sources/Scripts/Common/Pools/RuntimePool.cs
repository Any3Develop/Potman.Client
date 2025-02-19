using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Common.Pools.Abstractions;
using UnityEngine;

namespace Potman.Common.Pools
{
    public class RuntimePool<T> : IPool<T> where T : IPoolable
    {
        private readonly List<T> free = new();
        private readonly List<T> active = new();

        public IReadOnlyCollection<T> Active => active;
        public IReadOnlyCollection<T> Free => free;

        public void AddRage(IEnumerable<T> value, bool spawned = false, bool callSpawn = true)
        {
            foreach (var poolable in value)
                Add(poolable, spawned, callSpawn);
        }

        public void Add(T value, bool spawned = false, bool callSpawn = true)
        {
            if (spawned)
            {
                SpawnInternal(value, callSpawn);
                return;
            }

            ReleaseInternal(value);
        }

        public void Release(T value)
        {
            if (!active.Remove(value))
            {
                Debug.LogError($"Can't release {nameof(IPoolable)} who's never been in the pool : [{value}]");
                return;
            }

            ReleaseInternal(value);
        }

        public T Spawn(bool callSpawn = true)
        {
            TrySpawn(out var result, callSpawn);
            return result;
        }

        public bool TrySpawn<TCastable>(Predicate<TCastable> predicate, out TCastable result, bool callSpawn = true) where TCastable : T
        {
            result = free.OfType<TCastable>().FirstOrDefault(predicate.Invoke);
            if (!free.Remove(result))
                return false;

            SpawnInternal(result, callSpawn);
            return true;
        }

        private readonly Type destinationType = typeof(T);
        public bool TrySpawn<TCastable>(Type concreteType, out TCastable result, bool callSpawn = true) where TCastable : T
        {
            result = default;
            if (!destinationType.IsAssignableFrom(concreteType))
                return false;

            result = (TCastable)free.LastOrDefault(x => x.GetType() == concreteType);
            if (!free.Remove(result))
                return false;

            SpawnInternal(result, callSpawn);
            return true;
        }

        public bool TrySpawn<TCastable>(out TCastable result, bool callSpawn = true) where TCastable : T
        {
            result = free.OfType<TCastable>().FirstOrDefault();
            if (!free.Remove(result))
                return false;

            SpawnInternal(result, callSpawn);
            return true;
        }

        public bool TrySpawn(out T result, bool callSpawn = true)
        {
            result = free.FirstOrDefault();
            if (!free.Remove(result))
                return false;

            SpawnInternal(result, callSpawn);
            return TrySpawn<T>(out result, callSpawn);
        }

        public void Clear()
        {
            foreach (var poolable in free.Concat(active).Where(x => x != null))
                poolable.Dispose();

            free.Clear();
            active.Clear();
        }

        private void SpawnInternal<TPollable>(TPollable tObj, bool spawn = true) where TPollable : T
        {
            if (active.Contains(tObj))
            {
                Debug.LogError($"Can't add as spawned {nameof(IPoolable)} twice : [{tObj}]");
                return;
            }
                
            active.Add(tObj);
            
            if (spawn && tObj is ISpwanPoolable spwanPoolable)
                spwanPoolable.Spawn();
        }

        private void ReleaseInternal<TPollable>(TPollable tObj) where TPollable : T
        {
            if (free.Contains(tObj))
            {
                Debug.LogError($"Can't add as free {nameof(IPoolable)} twice : [{tObj}]");
                return;
            }
                
            tObj.Release();
            free.Add(tObj);
        }
    }
}