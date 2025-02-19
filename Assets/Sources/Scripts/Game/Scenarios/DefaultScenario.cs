using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Potman.Game.Common.Attributes;
using Potman.Game.Context.Abstractions;
using Potman.Game.Context.Data.Spawn;
using Potman.Game.Effects.Data;
using Potman.Game.Objects.Data;
using Potman.Game.Scenarios.Abstractions;
using Potman.Game.Scenarios.Data;
using Potman.Game.Stats.Data;
using Potman.Game.Units.Abstractions;
using Potman.Game.Units.Data;
using UnityEngine;

namespace Potman.Game.Scenarios
{
    [Scenario(ScenarioId.Default)]
    public class DefaultScenario : ScenarioBase
    {
        private readonly IGameContext context;
        private readonly IUnitViewModelFactory unitViewModelFactory;
        private readonly ISpawnPointProvider spawnPointProvider;
        private readonly ScenarioConfig config;

        private readonly CancellationTokenSource lifetime;
        private readonly CancellationToken token;

        public DefaultScenario(
            IGameContext context,
            IUnitViewModelFactory unitViewModelFactory,
            ISpawnPointProvider spawnPointProvider,
            ScenarioConfig config)
        {
            this.context = context;
            this.unitViewModelFactory = unitViewModelFactory;
            this.spawnPointProvider = spawnPointProvider;
            this.config = config;
            lifetime = new CancellationTokenSource();
            token = lifetime.Token;
        }

        protected override void OnStarted()
        {
            spawnPointProvider.Start(config);
            context.Start(config);
            StartAsync().Forget();
        }

        protected override void OnEnded()
        {
            spawnPointProvider?.End();
            context?.End();
            if (!lifetime.IsCancellationRequested)
            {
                lifetime?.Cancel();
                lifetime?.Dispose();
            }
        }

        protected override void OnDisposed()
        {
            if (!lifetime.IsCancellationRequested)
            {
                lifetime?.Cancel();
                lifetime?.Dispose();
            }
        }

        protected virtual async UniTask StartAsync()
        {
            try
            {
                var spawnConfigs = config.unitsScenario
                    .Concat<SpawnConfigBase>(config.objectsScenario)
                    .Concat(config.effectsScenario);
                
                foreach (var cfg in spawnConfigs)
                    SetDefault(cfg);
                
                await UniTask.Delay(config.delayedStartMs, cancellationToken: token);

                ExecuteLoopAsync().Forget();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                End();
            }
        }

        private async UniTask ExecuteLoopAsync()
        {
            try
            {
                var hasUnitsCfg = config.unitsScenario.Length > 0;
                var hasObjectsCfg = config.objectsScenario.Length > 0;
                var hasEffectsCfg = config.effectsScenario.Length > 0;
                var levelStat = context.Player.StatsCollection.Get(StatType.BaseLevel);
                var hpStat = context.Player.StatsCollection.Get(StatType.BaseHeath);
                var tickDelay = TimeSpan.FromMilliseconds(1000);
                
                while (Application.isPlaying && !token.IsCancellationRequested)
                {
                    var diedCount = context.UnitsDied;
                    var progress = (diedCount / context.UnitsMax) * 100;
                    var level = levelStat.Current;
                    var time = context.Time;
                    
                    if (hasUnitsCfg)
                        foreach (var cfg in config.unitsScenario)
                            if (ProcessConditions(cfg, level, diedCount, progress, time)) 
                                SpawnUnits(cfg);
                    
                    if (hasObjectsCfg)
                        foreach (var cfg in config.objectsScenario)
                            if (ProcessConditions(cfg, level, diedCount, progress, time)) 
                                SpawnObjects(cfg);
                    
                    if (hasEffectsCfg)
                        foreach (var cfg in config.effectsScenario)
                            if (ProcessConditions(cfg, level, diedCount, progress, time)) 
                                SpawnEffects(cfg);
                    
                    await UniTask.Delay(tickDelay, cancellationToken: token);

                    if (!CheckScenarioEnded(level, hpStat.Current)) 
                        continue;
                    
                    End();
                    break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                End();
            }
        }
        
        private void SpawnUnits(UnitSpawnConfig cfg)
        {
            for (var index = 0; index < cfg.units.Count; index++)
            {
                var unitCfg = cfg.units[index];
                var point = spawnPointProvider.Get(cfg.id, index, cfg.positionIds, cfg.positionSelector);
                unitViewModelFactory.Create(unitCfg, point.Position);
            }
        }
        
        private void SpawnObjects(ObjectSpawnConfig cfg)
        {
            for (var index = 0; index < cfg.objects.Count; index++)
            {
                var objCfg = cfg.objects[index];
                var point = spawnPointProvider.Get(cfg.id, index, cfg.positionIds, cfg.positionSelector);
                // unitViewModelFactory.Create(objCfg, point.Position);
            }
        }
        
        private void SpawnEffects(EffectSpawnConfig cfg)
        {
            for (var index = 0; index < cfg.effects.Count; index++)
            {
                var effectCfg = cfg.effects[index];
                var point = spawnPointProvider.Get(cfg.id, index, cfg.positionIds, cfg.positionSelector);
                // unitViewModelFactory.Create(effectCfg, point.Position);
            }
        }

        protected virtual bool CheckScenarioEnded(int level, int health) // TODO implement more specific conditions
        {
            if (context.UnitsMax > 0 && context.UnitsLeft <= 0 && context.UnitsAlive <= 0)
                return true;

            return health <= 0;
        }

        private static bool ProcessConditions(
            SpawnConfigBase cfg,
            int playerLevel,
            int unitsDied,
            int totalProgress,
            int gameTime)
        {
            if ((cfg.whenUnitsDied.enabled && !cfg.whenUnitsDied.IsConditionTrue(unitsDied))
                || (cfg.whenGameProgress.enabled && !cfg.whenGameProgress.IsConditionTrue(totalProgress))
                || (cfg.whenPlayerLevel.enabled && !cfg.whenPlayerLevel.IsConditionTrue(playerLevel))
                || (cfg.whenGameTime.enabled && !cfg.whenGameTime.IsConditionTrue(gameTime)))
                return false;
                
            cfg.whenUnitsDied.Passed(unitsDied);
            cfg.whenGameProgress.Passed(totalProgress);
            cfg.whenPlayerLevel.Passed(playerLevel);
            cfg.whenGameTime.Passed(gameTime);

            return true;
        }

        private static void SetDefault(SpawnConfigBase cfg)
        {
            cfg.whenUnitsDied.Default();
            cfg.whenGameProgress.Default();
            cfg.whenPlayerLevel.Default();
            cfg.whenGameTime.Default();
        }
    }
}