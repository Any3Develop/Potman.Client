using Potman.Common.Pools.Abstractions;
using UnityEngine;

namespace Potman.Game.Units.Abstractions
{
    public interface IUnitView : ISpwanPoolable
    {
        string Id { get; }
        IUnitViewModel ViewModel { get; }
        GameObject Container { get; }
        Transform Root { get; }
    }
}