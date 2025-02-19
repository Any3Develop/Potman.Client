using Potman.Common.DependencyInjection;
using Potman.Game.Player.Abstractions;

namespace Potman.Game.Player
{
    public class PlayerAnimatorFactory : IPlayerAnimatorFactory
    {
        private readonly IAbstractFactory abstractFactory;
        public PlayerAnimatorFactory(IAbstractFactory abstractFactory)
        {
            this.abstractFactory = abstractFactory;
        }

        public IPlayerAnimator Create(params object[] args)
        {
            return abstractFactory.Create<PlayerAnimator>(args);
        }
    }
}