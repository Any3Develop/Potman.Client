using Potman.Game.Player.Data;
using UnityEngine;

namespace Potman.Game.Player.Abstractions
{
    public interface IPlayerViewModelFactory
    {
        IPlayerViewModel Create(PlayerConfig config, Vector3 position);
    }
}