using System;
using UnityEngine;

namespace Potman.Game.Common.Data
{
    public abstract class ConfigBase : ScriptableObject
    {
        public string id;

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            id = Guid.NewGuid().ToString();
        }

        protected virtual void OnValidate()
        {
            id = Guid.NewGuid().ToString();
        }
#endif
    }
}