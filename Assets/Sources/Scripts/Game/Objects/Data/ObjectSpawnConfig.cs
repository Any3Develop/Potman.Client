using System.Collections.Generic;
using Potman.Game.Context.Data.Spawn;
using UnityEngine;

namespace Potman.Game.Objects.Data
{
    [CreateAssetMenu(order = -1, fileName = "ObjectSpawnConfig", menuName = "Potman/Objects/ObjectSpawnConfig")]
    public class ObjectSpawnConfig : SpawnConfigBase
    {
        [Space, Header("Providers")]
        [Tooltip("Target objects for this config, they will spawn when this config conditions will true.")]
        public List<ObjectConfig> objects = new();
    }
}