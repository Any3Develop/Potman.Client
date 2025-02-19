using System.Collections.Generic;
using Potman.Game.Context.Data.Spawn;
using Potman.Game.Context.Spawn;
using Potman.Game.Scenarios.Data;

namespace Potman.Game.Context.Abstractions
{
    public interface ISpawnPointProvider
    {
        void Start(ScenarioConfig config);
        void End();

        void Add(SpawnPoint point, bool defaultParent = true);
        void Remove(SpawnPoint point);
        void Remove(PositionId id);
        SpawnPoint Get(PositionId id);
        SpawnPoint Get(string cfgId, int index, List<PositionId> ids, PositionSelector selector);
        void Dispose();
    }
}