using System;

namespace Potman.Game.Units.Abstractions.Swarming
{
    public interface ISwarmMatchSystem : IDisposable
    {
        float MatchDistance { get; }
        int MaxGroupSize { get; set; }
        void Start(ISwarmAgent agent);
        void Stop(ISwarmAgent agent);
    }
}