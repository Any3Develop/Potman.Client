using Potman.Game.Units.Data;
using UnityEngine;

namespace Potman.Game.Units.Abstractions
{
    public interface IUnitViewModelFactory
    {
        IUnitViewModel Create(UnitConfig config, Vector3 position);
    }
}