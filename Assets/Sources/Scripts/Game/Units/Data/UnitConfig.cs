using System;
using System.Linq;
using Potman.Game.Common.Attributes;
using Potman.Game.Common.Data;
using Potman.Game.Stats.Data;
using UnityEngine;

namespace Potman.Game.Units.Data
{
    [CreateAssetMenu(order = -1, fileName = "UnitConfig", menuName = "Potman/Units/UnitConfig")]
    public class UnitConfig : ConfigBase
    {
        [Header("Prototypes")]
        public UnitView viewPrefab;
        public UnitViewModel viewModelPrefab;
        
        [Space, Header("Behaviours")]
        public UnitClassType classType;
        public UnitBehaviorType behaviorType;
        public UnitMovementType movementType;
        [NavMeshAreaMask] public int walkableAreas;
        public UnitBehaviourSet behavioursSet;
        
        [Header("Common")]
        public StatData[] stats;

#if UNITY_EDITOR
        [ContextMenu(nameof(ResetStatsForInfantry))]
        private void ResetStatsForInfantry()
        {
            stats = new StatData[]
            {
                new() {type = StatType.BaseLevel, value = 1, precision = 1},
                new() {type = StatType.BaseHeath, value = 100, precision = 1},
                
                new() {type = StatType.MoveSpeed, value = 250, precision = 10},
                new() {type = StatType.MoveTurnSpeed, value = 2500, precision = 10},
                new() {type = StatType.MoveAcceleration, value = 1000, precision = 10},
                
                new() {type = StatType.AgentHeght, value = 3, precision = 10},
                new() {type = StatType.AgentRadius, value = 25, precision = 10},
                new() {type = StatType.AgentPriority, value = 100, precision = 1}
            };
            
            OnValidate();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        [ContextMenu(nameof(ResetStatsForAviation))]
        private void ResetStatsForAviation()
        {
            stats = new StatData[]
            {
                new() {type = StatType.BaseLevel, value = 1, precision = 1},
                new() {type = StatType.BaseHeath, value = 100, precision = 1},
                
                new() {type = StatType.MoveSpeed, value = 250, precision = 10},
                new() {type = StatType.MoveTurnSpeed, value = 2500, precision = 10},
                new() {type = StatType.MoveAcceleration, value = 1000, precision = 10},
                new() {type = StatType.FlyAltitude, value = 30, precision = 1},
                new() {type = StatType.FlyDumping, value = 10, precision = 10},
                new() {type = StatType.FlyTrunDumping, value = 10, precision = 10},
                
                new() {type = StatType.AgentHeght, value = 3, precision = 10},
                new() {type = StatType.AgentRadius, value = 25, precision = 10},
                new() {type = StatType.AgentPriority, value = 20, precision = 1}
            };
            
            OnValidate();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        protected override void Reset()
        {
            base.Reset();
            ResetStatsForInfantry();
            OnValidate();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (stats is {Length: > 0})
            {
                for (var i = 0; i < stats.Length; i++)
                {
                    ref var data = ref stats[i];
                    data.name = data.type.ToString();
                    data.precision = Math.Max(1, data.precision);
                }
            }

            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                stats.ToDictionary(key => key.type, _ => default(bool));
            }
            catch
            {
                Debug.LogError($"There are duplicates of {nameof(StatType)} in the {nameof(stats)} list. This will lead to unexpected behavior.");
            }
        }
#endif
    }
}