using System;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;
using R3;
using UnityEngine;

namespace Potman.Game.Stats.Utils
{
    public static class StatsExtensions
    {
        private class Listener : IDisposable
        {
            private Action<IRuntimeStat> value;
            private IRuntimeStat stat;

            public Listener(Action<IRuntimeStat> value, IRuntimeStat stat)
            {
                this.value = value;
                this.stat = stat;
            }
            
            public void Dispose()
            {
                if (stat != null && value != null)
                    stat.OnChanged -= value;

                stat = null;
                value = null;
            }
        }
        
        public static IDisposable SubscribeInitStat(
            this IStatsCollection statsCollection, 
            StatType statType, 
            Action<IRuntimeStat> onChanged)
        {
            if (!statsCollection.TryGet(statType, out var stat))
            {
                Debug.LogWarning($"{nameof(SubscribeInitStat)} : Stat with type : {statType} doesn't exist.");
                return Disposable.Empty;
            }

            stat.OnChanged += onChanged;
            onChanged?.Invoke(stat);
            return new Listener(onChanged, stat);
        }
    }
}