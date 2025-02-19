using System;
using UnityEngine;

namespace Potman.Game.Stats.Data
{
    [Serializable]
    public struct StatData
    {
#if UNITY_EDITOR
        /// <summary>
        /// It's for visual in Unity.Editor.Inspector
        /// </summary>
        [HideInInspector] public string name;        
#endif
        
        [Tooltip("Unique identifier of the stat.")]
        public StatType type;
        
        [Tooltip("Value : This value will be the Current component at runtime.")]
        public int value;
        
        [Tooltip("Min : The current component will be clamped to this value.")]
        public int min;
        
        [Tooltip("Max : The current component will be clamped to this value.")]
        public int max;
                
        [Tooltip("Enable to use the Min component.")]
        public bool useMin;
        
        [Tooltip("Enable to use the Max component.")]
        public bool useMax;
        
        [Tooltip("If you need a floating point stat, this value will divide your int by float. For example: Value = 1, Precision = 100, Result = 0.01f")]
        public int precision;
    }
}