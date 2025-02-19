using System;
using Potman.Common.Pools.Abstractions;
using Potman.Game.Stats.Data;

namespace Potman.Game.Stats.Abstractions
{
    public interface IRuntimeStat : IPoolable
    {
        event Action<IRuntimeStat> OnChanged;
        
        int Precision { get; }
        float CurrentFloat { get; }
        float MinFloat { get; }
        float MaxFloat { get; }
        
        int Current { get; }
        int Min { get; }
        int Max { get; }
        StatType Type { get; }
        
        void Set(int value);
        void Add(int value);
        void Subtract(int value);
        void SetToMax();
    }
}