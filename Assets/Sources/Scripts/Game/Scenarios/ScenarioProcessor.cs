using System;
using System.Linq;
using Potman.Common.Events;
using Potman.Common.Utilities;
using Potman.Game.Scenarios.Abstractions;
using Potman.Game.Scenarios.Data;
using Potman.Game.Scenarios.Events;
using Potman.Lobby.Identity;
using Potman.Lobby.Levels;
using R3;
using UnityEngine;

namespace Potman.Game.Scenarios
{
    public class ScenarioProcessor : IScenarioProcessor, IDisposable
    {
        private readonly IScenarioFactory scenarioFactory;
        private CompositeDisposable disposables;
        public IScenario Scenario { get; private set; }

        public ScenarioProcessor(IScenarioFactory scenarioFactory)
        {
            this.scenarioFactory = scenarioFactory;
            disposables = new CompositeDisposable();
            Observable.NextFrame().Subscribe(_ => Start()).AddTo(disposables);
        }

        private void Start()
        {
            var levelData = User.Redirections.GetArg<LevelData>();
            
            disposables?.Dispose();
            disposables = new CompositeDisposable();
            MessageBroker.Receive<SenarioChangedEvent>()
                .Where(x => x.Scenario == Scenario)
                .Subscribe(OnScenarioChanged)
                .AddTo(disposables);
            
            try
            {
                var cfg = Resources.LoadAll<ScenarioConfig>("Game/Scenarios")
                    .FirstOrDefault(x => x.id == levelData.Id && x.difficulty == levelData.Difficulty);
                
                Scenario = scenarioFactory.Create(cfg);
                Scenario.Start();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void Dispose()
        {
            disposables?.Dispose();
            Scenario?.Dispose();
            disposables = null;
            Scenario = null;
        }

        private void OnScenarioChanged(SenarioChangedEvent evData)
        {
            if (evData.Current.AnyFlags(ScenarioState.Ended | ScenarioState.None))
            {
                Debug.Log($"Scenario : {evData.Scenario} ended!");
            }
        }
    }
}