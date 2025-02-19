using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Potman.Common.Events;
using Potman.Common.Pools;
using Potman.Common.Pools.Abstractions;
using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;
using Potman.Game.Units.Abstractions;
using Potman.Game.Units.Abstractions.Swarming;
using Potman.Game.Units.Data;
using Potman.Game.Units.Events;
using R3;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using IServiceProvider = Potman.Common.DependencyInjection.IServiceProvider;
using Random = UnityEngine.Random;

namespace Potman.Game.Units
{
    public class UnitViewModel : PoolableView, IUnitViewModel
    {
        [field:SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
        [field:SerializeField] public BehaviorGraphAgent BehaviorGraphAgent { get; private set; }
        [field:SerializeField] public Collider Collider { get; private set; }
        
        private IDisposable disposables;
        
        #region Dependencies
        public IUnitView View { get; private set; }
        public UnitConfig Config { get; private set; }
        public IStatsCollection StatsCollection { get; private set; }
        public ISwarmAgent SwarmAgent { get; private set; }
        public IUnitMovement Movement { get; private set; }
        
        public IServiceProvider ServiceProvider { get; private set; }
        public IUnitViewFactory ViewFactory { get; private set; }
        public IStatFactory StatFactory { get; private set; }
        public ISwarmMatchSystem SwarmMatchSystem { get; private set; }
        public IPool<IUnitView> ViewPool { get; private set; }
        public IPool<IUnitViewModel> ViewModelPool { get; private set; } // TODO TEST
        public IPool<ISwarmMaster> MasterPool { get; private set; } // TODO TEST

        #endregion

        public void Construct(IServiceProvider serviceProvider)
        {
            if (DisposedLog())
                return;

            ServiceProvider = serviceProvider;
            StatsCollection = serviceProvider.GetRequiredService<IStatsCollection>();
            ViewFactory = serviceProvider.GetRequiredService<IUnitViewFactory>();
            StatFactory = serviceProvider.GetRequiredService<IStatFactory>();
            ViewPool = serviceProvider.GetRequiredService<IPool<IUnitView>>();
            ViewModelPool = serviceProvider.GetRequiredService<IPool<IUnitViewModel>>();
            MasterPool = serviceProvider.GetRequiredService<IPool<ISwarmMaster>>();
            SwarmMatchSystem = serviceProvider.GetRequiredService<ISwarmMatchSystem>();
            SwarmAgent = serviceProvider.GetRequiredService<ISwarmAgentFactory>().Create(this);
            Movement = serviceProvider.GetRequiredService<IUnitMovementFactory>().Create(NavMeshAgent, this);
        }

        public void Init(UnitConfig config, Vector3 position)
        {
            if (DisposedLog())
                return;

            Config = config;
            StatsCollection.AddRange(config.stats.Select(StatFactory.Create));
            View = ViewFactory.Create(this);
            using var eventBuilder = Disposable.CreateBuilder();
            
            if (BehaviorGraphAgent)
            {
                BehaviorGraphAgent.End();
                BehaviorGraphAgent.Graph = Instantiate(Config.behavioursSet.Get(config.behaviorType));
            }
            
            { // TODO FOR TEST
                if (StatsCollection.TryGet(StatType.MoveSpeed, out var moveSpeed))
                    moveSpeed.Set(Random.Range(moveSpeed.Current, moveSpeed.Current * 2));

                if (StatsCollection.TryGet(StatType.FlyAltitude, out var altitude))
                    altitude.Set((int) Random.Range(altitude.CurrentFloat, altitude.CurrentFloat / 5f) * altitude.Precision);

                eventBuilder.Add(Observable.EveryUpdate().Subscribe(_ =>
                {
                    if (disposables != null && NavMeshAgent.hasPath && Movement.RemainingDistance <= 5)
                    {
                        ViewModelPool.Release(this);
                        MessageBroker.Publish(new UnitDiedEvent());
                    }
                }));
            }

            disposables = eventBuilder.Build();
            OnInit(position);
        }

        protected virtual void OnInit(Vector3 position)
        {
            Movement.Move(position);
            View.Root.SetParent(Root);
            View.Root.localPosition = Vector3.zero;
            View.Root.localRotation = Quaternion.identity;
        }

        protected override void OnSpawned()
        {
            // SwarmAgent.Enable(true); // TODO FOR TEST
            SwarmAgent.Enable(false); // TODO FOR TEST
            Movement.Enable(true);
            
            MessageBroker.Publish(new UnitSpawnedEvent());
            
            if (BehaviorGraphAgent)
                BehaviorGraphAgent.Start();
        }

        protected override void OnReleased()
        {
            if (BehaviorGraphAgent)
            {
                BehaviorGraphAgent.End();
                if (BehaviorGraphAgent.Graph)
                    DestroyImmediate(BehaviorGraphAgent.Graph);

                BehaviorGraphAgent.Graph = null;
            }
            
            disposables?.Dispose();
            Movement.Enable(false);
            SwarmAgent.Enable(false);
            ViewPool.Release(View);
            StatsCollection.Clear();
            
            disposables = null;
            Config = null;
            View = null;
        }

        protected override void OnDisposed()
        {
            disposables?.Dispose();
            disposables = null;
            StatsCollection = null;
            ServiceProvider = null;
            ViewFactory = null;
            StatFactory = null;
            ViewPool = null;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (!Collider)
                Collider = GetComponent<Collider>();

            if (!NavMeshAgent)
                NavMeshAgent = GetComponent<NavMeshAgent>();

            if (!BehaviorGraphAgent)
                BehaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (SwarmAgent == null)
                return;

            var alone = SwarmAgent.Master == null;
            var isMaster = !alone && SwarmAgent.Master.Agent == SwarmAgent;
            var smallRadius = SwarmMatchSystem.MatchDistance / 10f;
            
            var size = alone || !isMaster ? smallRadius : SwarmMatchSystem.MatchDistance;
            var color = alone ? Color.black : GetColor(SwarmAgent.Master.Id);
            var pos = transform.position;
            
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.Disc(Quaternion.identity, pos, Vector3.up, size, false, 0f);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Color GetColor(int value)
        {
            return Color.HSVToRGB(value / (float)(MasterPool.Active.Count + MasterPool.Free.Count), 1f, 1f);
        }
#endif
    }
}