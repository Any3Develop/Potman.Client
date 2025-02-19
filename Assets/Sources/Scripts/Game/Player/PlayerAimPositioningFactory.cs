using Potman.Common.DependencyInjection;
using Potman.Game.Player.Abstractions;

namespace Potman.Game.Player
{
    public interface IPlayerAimPositioningFactory
    {
        IAimPositioning Create(IPlayerViewModel playerViewModel);
    }

    public class PlayerAimPositioningFactory : IPlayerAimPositioningFactory
    {
        private readonly IAbstractFactory abstractFactory;

        public PlayerAimPositioningFactory(IAbstractFactory abstractFactory)
        {
            this.abstractFactory = abstractFactory;
        }

        public IAimPositioning Create(IPlayerViewModel playerViewModel)
        {
            var positioning = abstractFactory.Create<AimPositioning>(playerViewModel);
            return positioning;
        }
    }
}