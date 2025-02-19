using Potman.Common.DependencyInjection;
using Potman.Common.Pools.Abstractions;
using Potman.Game.Units.Abstractions;

namespace Potman.Game.Units
{
    public class UnitViewFactory : IUnitViewFactory
    {
        private readonly IPool<IUnitView> pool;
        private readonly IAbstractFactory abstractFactory;

        public UnitViewFactory(
            IPool<IUnitView> pool,
            IAbstractFactory abstractFactory)
        {
            this.pool = pool;
            this.abstractFactory = abstractFactory;
        }

        public IUnitView Create(IUnitViewModel value)
        {
            var unitId = value.Config.viewPrefab.Id;
            if (pool.TrySpawn(result => result.Id == unitId, out UnitView view, false))
            {
                SetName(view, value);
                view.Init(value);
                view.Spawn();
                return view;
            }

            view = abstractFactory.CreateUnityObject<UnitView>(value.Config.viewPrefab);
            view.Init(value);
            SetName(view, value);
            pool.Add(view, true);
            return view;
        }
        
        private void SetName(UnitView view, IUnitViewModel viewModel)
        {
            view.name = $"{viewModel.Config.viewPrefab.name}_{view.Id}"; // TODO FOR TEST
        }
    }
}