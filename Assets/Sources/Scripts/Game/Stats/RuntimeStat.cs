using System;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;

namespace Potman.Game.Stats
{
    public class RuntimeStat : IRuntimeStat
    {
        private StatData stat;
        private ModifiableInt current = new();
        private ModifiableInt min = new();
        private ModifiableInt max = new();
        
        public event Action<IRuntimeStat> OnChanged;
        public StatType Type => stat.type;
        public int Precision => stat.precision;
        public float CurrentFloat => (float) current / Precision;
        public float MinFloat => (float) min / Precision;
        public float MaxFloat => (float) max / Precision;
        public int Current => current;
        public int Min => min;
        public int Max => max;

        public void Init(StatData value)
        {
            stat = value;
            current.Override(CalmpMinMax(stat.value));
            min.Override(stat.min);
            max.Override(stat.max);
        }

        public void Set(int value)
        {
            var newModifier = CalmpMinMax(value - current.Value);
            if (newModifier == current.Modifier)
                return;
            
            current.Set(newModifier);
            OnChanged?.Invoke(this);
        }

        public void Add(int value)
        {
            var virtualTotal = CalmpMinMax(current.Total + value); // clamp a new virtual total
            var newValue = virtualTotal - current.Total; // find how much we will add
            if (newValue == 0) // if any changes won't come
                return;
            
            current.Add(newValue); // a new modifier
            OnChanged?.Invoke(this);
        }

        public void Subtract(int value) => Add(-value);

        public void SetToMax()
        {
            if (current.Modifier == 0)
                return;
            
            current.Set(0);
            OnChanged?.Invoke(this);
        }

        public void Release()
        {
            OnChanged = null;
            current?.Set(0);
            min?.Set(0);
            max?.Set(0);
        }
        
        public void Dispose()
        {
            OnChanged = null;
            current = null;
            min = null;
            max = null;
        }

        private int CalmpMinMax(int curr) 
            => Math.Clamp(curr, stat.useMin ? min : int.MinValue, stat.useMax ? max : int.MaxValue);
    }
}