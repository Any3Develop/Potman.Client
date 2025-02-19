using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Game.Common.Data;
using Potman.Game.Context.Data.Spawn;
using Potman.Game.Stats.Data;
using UnityEngine;

namespace Potman.Game.Player.Data
{
    [CreateAssetMenu(order = -1, fileName = "PlayerConfig", menuName = "Potman/Player/PlayerConfig")]
    public class PlayerConfig : ConfigBase
    {
        [Header("Prototypes")]
        public PlayerView viewPrefab;
        public PlayerViewModel viewModelPrefab;
        
        [Space, Header("General")]
        [Tooltip("Provide how to select from the list of PositionIds a next spawn id.")]
        public PositionSelector positionSelector;

        [Tooltip("At what points can player of this config spawn?")]
        public List<PositionId> positionIds = new();
        
        [Header("Common")]
        public StatData[] stats;

#if UNITY_EDITOR
        [ContextMenu(nameof(ResetStats))]
        private void ResetStats()
        {
            stats = new StatData[]
            {
                new() {type = StatType.BaseLevel, value = 1, useMin = true, min = 1, precision = 1},
                new() {type = StatType.BaseHeath, value = 100, useMin = true, min = 0, precision = 1},
                new() {type = StatType.MoveSpeed, value = 250, precision = 10},
                new() {type = StatType.MoveTurnSpeed, value = 500, precision = 10},
                new() {type = StatType.MoveAcceleration, value = 1000, precision = 10},
                new() {type = StatType.MoveDumpingFactor, value = 95, precision = 10},
                new() {type = StatType.AgentHeght, value = 4, precision = 10},
                new() {type = StatType.AgentRadius, value = 25, precision = 10},
            };
            
            OnValidate();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        protected override void Reset()
        {
            base.Reset();
            ResetStats();
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