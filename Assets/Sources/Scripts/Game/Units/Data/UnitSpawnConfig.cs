using System.Collections.Generic;
using Potman.Game.Context.Data.Spawn;
using UnityEngine;

namespace Potman.Game.Units.Data
{
    [CreateAssetMenu(order = -1, fileName = "UnitSpawnConfig", menuName = "Potman/Units/UnitSpawnConfig")]
    public class UnitSpawnConfig : SpawnConfigBase
    {
        [Space, Header("Providers")]
        [Tooltip("Target units for this config, they will spawn when this config conditions will true.")]
        public List<UnitConfig> units = new();

    }
}