using UnityEngine;

namespace Potman.Game.Player.Abstractions
{
    public interface IPlayerView
    {
        Transform Root { get; }
        GameObject Container { get; }
        IPlayerViewModel ViewModel { get; }
    }
}