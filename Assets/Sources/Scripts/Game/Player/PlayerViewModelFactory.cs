using Potman.Common.DependencyInjection;
using Potman.Game.Player.Abstractions;
using Potman.Game.Player.Data;
using UnityEngine;

namespace Potman.Game.Player
{
    public class PlayerViewModelFactory : IPlayerViewModelFactory
    {
        private readonly IAbstractFactory abstractFactory;
        private readonly IServiceProvider serviceProvider;

        public PlayerViewModelFactory(IAbstractFactory abstractFactory, IServiceProvider serviceProvider)
        {
            this.abstractFactory = abstractFactory;
            this.serviceProvider = serviceProvider;
        }

        public IPlayerViewModel Create(PlayerConfig config, Vector3 position)
        {
            var player = abstractFactory.CreateUnityObject<PlayerViewModel>(config.viewModelPrefab);
            player.Init(serviceProvider, config, position);
            return player;
        }
    }
}