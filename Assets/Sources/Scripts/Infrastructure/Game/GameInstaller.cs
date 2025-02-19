using Potman.Common.Pools;
using Potman.Game.Context;
using Potman.Game.Context.Spawn;
using Potman.Game.Player;
using Potman.Game.Scenarios;
using Potman.Game.Stats;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Units;
using Potman.Game.Units.Abstractions;
using Potman.Game.Units.Abstractions.Swarming;
using Potman.Game.Units.Swarming;
using Potman.Infrastructure.Common.DependencyInjection;
using UnityEngine;
using Zenject;

namespace Potman.Infrastructure.Game
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            InstallFactories();
            InstallPools();
            InstallContext();
            InstallUI();
            InstallGameSettings();
            InstallEntryPoint();
        }

        private void InstallFactories()
        {
            Container
                .BindInterfacesTo<ZenjectAbstractFactory>()
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesTo<ScenarioFactory>()
                .AsSingle()
                .NonLazy();

            Container
                .BindInterfacesTo<UnitViewFactory>()
                .AsSingle();

            Container
                .BindInterfacesTo<UnitViewModelFactory>()
                .AsSingle();

            Container
                .BindInterfacesTo<UnitPathFactory>()
                .AsSingle();

            Container
                .BindInterfacesTo<SwarmAgentFactory>()
                .AsSingle();

            Container
                .BindInterfacesTo<SwarmMasterFactory>()
                .AsSingle();

            Container
                .BindInterfacesTo<UnitMovementFactory>()
                .AsSingle();

            Container
                .BindInterfacesTo<StatFactory>()
                .AsSingle();

            Container
                .BindInterfacesTo<PlayerViewModelFactory>()
                .AsSingle();

            Container
                .BindInterfacesTo<PlayerViewFactory>()
                .AsTransient();

            Container
                .BindInterfacesTo<PlayerMovementFactory>()
                .AsTransient();

            Container
                .BindInterfacesTo<PlayerAnimatorFactory>()
                .AsTransient();

            Container
                .BindInterfacesTo<PlayerAimPositioningFactory>()
                .AsSingle();
        }

        private void InstallPools()
        {
            Container
                .BindInterfacesTo<RuntimePool<IUnitPath>>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<RuntimePool<IUnitView>>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<RuntimePool<IUnitViewModel>>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<RuntimePool<IRuntimeStat>>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<RuntimePool<ISwarmMaster>>()
                .AsSingle();
        }

        private void InstallContext()
        {
            Container
                .BindInterfacesTo<ZenjectServiceProvider>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<StatsCollection>()
                .AsTransient();
            
            Container
                .BindInterfacesTo<DefaultCameraProvider>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<SwarmMatchSystem>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<SpawnPointProvider>()
                .AsSingle();
            
            Container
                .BindInterfacesTo<GameContext>()
                .AsSingle();

        }
        
        private void InstallUI()
        {
            Container
                .BindInterfacesTo<SetupGameUIGroup>()
                .AsSingle()
                .NonLazy();
        }
        
        private void InstallGameSettings()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }

        private void InstallEntryPoint()
        {
            Container
                .BindInterfacesTo<ScenarioProcessor>()
                .AsSingle()
                .NonLazy();
        }
    }
}