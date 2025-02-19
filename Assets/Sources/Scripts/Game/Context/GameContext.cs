using System;
using Potman.Common.Events;
using Potman.Common.Utilities;
using Potman.Game.Context.Abstractions;
using Potman.Game.Player.Abstractions;
using Potman.Game.Scenarios.Abstractions;
using Potman.Game.Scenarios.Data;
using Potman.Game.Scenarios.Events;
using Potman.Game.Units.Events;
using R3;

namespace Potman.Game.Context
{
    public class GameContext : IGameContext
    {
        private readonly IPlayerViewModelFactory playerViewModelFactory;
        private readonly ISpawnPointProvider spawnPointProvider;
        private int maxTime;
        
        public IPlayerViewModel Player { get; private set; }
        public int Time { get; private set; }
        public int TimeLeft => maxTime - Time;
        public int UnitsMax { get; private set; }
        public int UnitsAlive { get; private set; }
        public int UnitsSpawned  => UnitsAlive + UnitsDied;
        public int UnitsDied { get; private set; }
        public int UnitsLeft => UnitsMax == 0 ? int.MaxValue : UnitsMax - UnitsSpawned;
        private const int TickValueMs = 1;
        private CompositeDisposable disposables;
        private bool isPaused;
        
        public GameContext(
            IPlayerViewModelFactory playerViewModelFactory,
            ISpawnPointProvider spawnPointProvider)
        {
            this.playerViewModelFactory = playerViewModelFactory;
            this.spawnPointProvider = spawnPointProvider;
        }

        public void Pause(bool value)
        {
            if (isPaused == value)
                return;

            isPaused = value;
        }

        public void Start(ScenarioConfig config)
        {
            isPaused = false;
            maxTime = config.gameTime;
            
            UnitsMax = config.maxUnitsAllTheTime;
            disposables?.Dispose();
            disposables = new CompositeDisposable();
            
            Observable.Interval(TimeSpan.FromSeconds(TickValueMs))
                .Skip(TimeSpan.FromMilliseconds(config.delayedStartMs))
                .Subscribe(_ => Update()).AddTo(disposables);
            
            MessageBroker.Receive<SenarioChangedEvent>()
                .Subscribe(evData => Pause(evData.Current.AnyFlags(ScenarioState.Paused)))
                .AddTo(disposables);
            
            MessageBroker.Receive<UnitSpawnedEvent>().Subscribe(_ => UnitsAlive++).AddTo(disposables);
            MessageBroker.Receive<UnitDiedEvent>().Subscribe(_ => UnitsDied++).AddTo(disposables);

            var cfg = config.playerConfig;
            var point = spawnPointProvider.Get(cfg.id, 0, cfg.positionIds, cfg.positionSelector);
            Player = playerViewModelFactory.Create(config.playerConfig, point.Position);
        }

        private void Update()
        {
            if (isPaused)
                return;
            
            Time += TickValueMs;
            if (Time < maxTime)
                return;

            Time = maxTime;
            disposables?.Dispose();
            disposables = null;
            MessageBroker.Publish(new ScenarioTimeEndedEvent());
        }

        public void End()
        {
            isPaused = false;
            disposables?.Dispose();
            disposables = null;
            Player = null;
            Time = 0;
        }
    }
}