using System.Collections.Generic;
using Potman.Game.Context.Data.Spawn;
using UnityEngine;

namespace Potman.Game.Effects.Data
{
    [CreateAssetMenu(order = -1, fileName = "EffectSpawnConfig", menuName = "Potman/Effects/EffectSpawnConfig")]
    public class EffectSpawnConfig : SpawnConfigBase
    {
        [Space, Header("Providers")]
        [Tooltip("Target objects for this config, they will spawn when this config conditions will true.")]
        public List<EffectConfig> effects = new();
    }
}