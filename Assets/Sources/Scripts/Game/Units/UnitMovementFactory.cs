using Potman.Common.DependencyInjection;
using Potman.Game.Units.Abstractions;

namespace Potman.Game.Units
{
    public class UnitMovementFactory : IUnitMovementFactory
    {
        private readonly IAbstractFactory abstractFactory;

        public UnitMovementFactory(IAbstractFactory abstractFactory)
        {
            this.abstractFactory = abstractFactory;
        }

        public IUnitMovement Create(params object[] args)
        {
            return abstractFactory.Create<UnitNavmeshMovement>(args);
        }
    }
}