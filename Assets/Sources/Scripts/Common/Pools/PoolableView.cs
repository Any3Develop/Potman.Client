using System;
using Potman.Common.Pools.Abstractions;
using UnityEngine;

namespace Potman.Common.Pools
{
    public abstract class PoolableView : MonoBehaviour, ISpwanPoolable
    {
        [SerializeField] protected bool activeOnSpawn = true;
        [SerializeField] protected bool activeOnRelease;
        public bool Disposed { get; private set; }

        [field: SerializeField] public Transform Root { get; private set; }
        [field: SerializeField] public GameObject Container { get; private set; }

        public void Release()
        {
            if (DisposedLog())
                return;

            AssertComponents();
            Container.SetActive(activeOnRelease);
            OnReleased();
        }

        public void Spawn()
        {
            if (DisposedLog())
                return;

            AssertComponents();
            Container.SetActive(activeOnSpawn);
            OnSpawned();
        }

        public void Dispose()
        {
            if (DisposedLog())
                return;

            Disposed = true;
            OnDisposed();
        }

        protected void OnDestroy() => Dispose();
        protected virtual void OnSpawned() {}
        protected virtual void OnReleased() {}
        protected virtual void OnDisposed() {}

        protected virtual void OnValidate()
        {
            if (!Container)
                Container = gameObject;

            if (!Root)
                Root = transform;
        }

        protected bool DisposedLog()
        {
            if (!Disposed)
                return false;

            Debug.LogError($"You are trying to use disposed {GetType().Name}.");
            return true;
        }

        private void AssertComponents()
        {
            if (!Container)
                throw new NullReferenceException($"{nameof(Container)} : {nameof(GameObject)} Not set in inspector or was missed while spawn.");

            if (!Root)
                throw new NullReferenceException($"{nameof(Root)} : {nameof(GameObject)} Not set in inspector or was missed while spawn.");
        }
    }
}