using System;
using System.Collections.Generic;
using System.Linq;
using Potman.Common.DependencyInjection;
using Potman.Game.Context.Abstractions;
using Potman.Game.Context.Data.Spawn;
using Potman.Game.Scenarios.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Potman.Game.Context.Spawn
{
    public class SpawnPointProvider : IDisposable, ISpawnPointProvider
    {
        private readonly IAbstractFactory abstractFactory;
        private Dictionary<PositionId, SpawnPoint> spawnMap;
        private Dictionary<string, int> spawnIterations;
        private Transform spawnPointsRoot;

        public SpawnPointProvider(IAbstractFactory abstractFactory)
        {
            this.abstractFactory = abstractFactory;
        }

        public void Start(ScenarioConfig config)
        {
            spawnIterations = new Dictionary<string, int>();
            spawnPointsRoot = abstractFactory.CreateUnityObject(config.spawnPoints).transform;
            spawnMap = spawnPointsRoot
                .GetComponentsInChildren<SpawnPoint>(true)
                .ToDictionary(key => key.Id);
        }

        public void End()
        {
            spawnIterations?.Clear();
            spawnIterations = null;
            spawnMap?.Clear();
            spawnMap = null;
        }

        public void Add(SpawnPoint point, bool defaultParent = true)
        {
            if (point != null && spawnMap.TryAdd(point.Id, point) && defaultParent)
                point.SetParent(spawnPointsRoot);
        }

        public void Remove(SpawnPoint point)
        {
            if (point)
                spawnMap.Remove(point.Id);
        }

        public void Remove(PositionId id)
        {
            spawnMap.Remove(id);
        }

        public SpawnPoint Get(PositionId id) => spawnMap[id];

        public SpawnPoint Get(string cfgId, int index, List<PositionId> ids, PositionSelector selector)
        {
            switch (selector)
            {
                case PositionSelector.Index: return Get(ids[Mathf.Clamp(index, 0, ids.Count - 1)]);

                case PositionSelector.RoundRobin:
                    var iter = GetIterations();
                    spawnIterations[cfgId] = (iter + 1) % ids.Count;
                    return Get(ids[iter]);

                case PositionSelector.ReverseRoundRobin:
                    var reverseIter = GetIterations();
                    spawnIterations[cfgId] = (reverseIter - 1 + ids.Count) % ids.Count;
                    return Get(ids[reverseIter]);

                case PositionSelector.PingPong:
                    var pingPongIter = GetIterations();
                    var cycleLength = 2 * ids.Count - 2;
                    var cycleIndex = pingPongIter % cycleLength;

                    var adjustedIndex = cycleIndex < ids.Count 
                        ? cycleIndex
                        : 2 * ids.Count - 2 - cycleIndex;

                    spawnIterations[cfgId] = pingPongIter + 1;
                    return Get(ids[adjustedIndex]);

                case PositionSelector.Random:
                default: return Get(ids[Random.Range(0, ids.Count)]);
            }

            int GetIterations(int defaulValue = 0)
            {
                if (!spawnIterations.TryGetValue(cfgId, out var iteration))
                    spawnIterations[cfgId] = iteration = defaulValue;

                return iteration;
            }
        }

        public void Dispose()
        {
            spawnMap?.Clear();
            spawnMap = null;
        }
    }
}