using Potman.Common.DependencyInjection;
using Potman.Game.Player.Abstractions;

namespace Potman.Game.Player
{
    public class PlayerMovementFactory : IPlayerMovementFactory
    {
        private readonly IAbstractFactory abstractFactory;
        public PlayerMovementFactory(IAbstractFactory abstractFactory)
        {
            this.abstractFactory = abstractFactory;
        }

        public IPlayerMovement Create(params object[] args)
        {
            return abstractFactory.Create<PlayerMovement>(args);
        }
    }
}