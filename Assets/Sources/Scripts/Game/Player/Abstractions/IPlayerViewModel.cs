using Potman.Common.DependencyInjection;
using Potman.Game.Player.Data;
using Potman.Game.Stats.Abstractions;
using UnityEngine;

namespace Potman.Game.Player.Abstractions
{
    public interface IPlayerViewModel
    {
        Transform Root { get; }
        GameObject Container { get; }
        PlayerConfig Config { get; }
        IPlayerView View { get; }
        IStatsCollection StatsCollection { get; }
        IPlayerMovement PlayerMovement { get; }
        IPlayerAnimator PlayerAnimator { get; }
        IServiceProvider ServiceProvider { get; }
    }
}