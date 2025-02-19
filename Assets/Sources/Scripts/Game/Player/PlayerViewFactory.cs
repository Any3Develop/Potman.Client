using Potman.Common.DependencyInjection;
using Potman.Game.Player.Abstractions;
using UnityEngine;

namespace Potman.Game.Player
{
    public class PlayerViewFactory : IPlayerViewFactory
    {
        private readonly IAbstractFactory abstractFactory;

        public PlayerViewFactory(IAbstractFactory abstractFactory)
        {
            this.abstractFactory = abstractFactory;
        }

        public IPlayerView Create(IPlayerViewModel viewModel)
        {
            var view = abstractFactory.CreateUnityObject<PlayerView>(viewModel.Config.viewPrefab);
            view.Root.SetParent(viewModel.Root);
            view.Root.localPosition = Vector3.zero;
            view.Root.localRotation = Quaternion.identity;
            view.Init(viewModel);
            return view;
        }
    }
}