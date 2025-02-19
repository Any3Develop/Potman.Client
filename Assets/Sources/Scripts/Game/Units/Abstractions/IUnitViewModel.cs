using Potman.Common.Pools.Abstractions;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Units.Abstractions.Swarming;
using Potman.Game.Units.Data;
using UnityEngine;

namespace Potman.Game.Units.Abstractions
{
    public interface IUnitViewModel : ISpwanPoolable
    {
        Transform Root { get; }
        GameObject Container { get; }
        
        IUnitView View { get; }
        UnitConfig Config { get; }
        IStatsCollection StatsCollection { get; }
        ISwarmAgent SwarmAgent { get; }
        IUnitMovement Movement { get; }
    }
}