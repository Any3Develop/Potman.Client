using System;
using Potman.Common.SerializableDictionary;
using Unity.Behavior;
using UnityEngine;

namespace Potman.Game.Units.Data
{
    [CreateAssetMenu(order = -1, fileName = "UnitBehaviourSet", menuName = "Potman/Units/UnitBehaviourSet")]
    public class UnitBehaviourSet : ScriptableObject
    {
        [Serializable]
        private class BehaviorsMap : SerializableDictionary<UnitBehaviorType, BehaviorGraph> {}

        [SerializeField] private BehaviorsMap behavioursSet = new();

        public BehaviorGraph Get(UnitBehaviorType value) => behavioursSet[value];
    }
}