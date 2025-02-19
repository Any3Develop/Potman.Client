﻿using System.Collections.Generic;
using System.Linq;
using Potman.Common.Events;
using Potman.Common.Utilities;
using Potman.Game.Scenarios.Abstractions;
using Potman.Game.Scenarios.Data;
using Potman.Game.Scenarios.Events;
using R3;

namespace Potman.Game.Scenarios
{
    public abstract class ScenarioBase : IScenario
    {
        protected readonly ReactiveProperty<ScenarioState> StateInternal = new(ScenarioState.None);
        protected readonly CompositeDisposable Disposables = new();

        public ScenarioState State => StateInternal.CurrentValue;
        public virtual IEnumerable<IScenario> Nested { get; } = Enumerable.Empty<IScenario>();

        public void Start()
        {
            if (!State.AnyFlags(ScenarioState.None))
                return;

            StateInternal.Pairwise()
                .Select(x => new SenarioChangedEvent(x.Current, x.Previous, this))
                .Subscribe(MessageBroker.Publish)
                .AddTo(Disposables);

            StateInternal.AddTo(Disposables);
            StateInternal.Value = ScenarioState.Playing;
            OnStarted();
        }

        public void Pause(bool value)
        {
            if (State.AnyFlags(ScenarioState.None | ScenarioState.Ended)
                || (value && !State.AnyFlags(ScenarioState.Paused))
                || (!value && State.AnyFlags(ScenarioState.Paused)))
                return;

            if (value)
            {
                State.AddFlag(ScenarioState.Paused);
                OnPaused();
                return;
            }

            State.AddFlag(ScenarioState.Paused);
            OnResume();
        }

        public void End()
        {
            if (State.AnyFlags(ScenarioState.None | ScenarioState.Ended))
                return;

            Pause(false);
            StateInternal.Value = State.AddFlag(ScenarioState.Ended);
            OnEnded();
        }

        public void Dispose()
        {
            if (State.AllFlags(ScenarioState.None))
                return;

            Disposables?.Dispose();
            OnDisposed();
        }

        protected virtual void OnStarted() {}
        protected virtual void OnPaused() {}
        protected virtual void OnResume() {}
        protected virtual void OnEnded() {}
        protected virtual void OnDisposed() {}
    }
}