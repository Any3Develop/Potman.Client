using System;
using Potman.Common.Pools.Abstractions;
using Potman.Game.Stats.Data;
using Potman.Game.Units.Abstractions;
using Potman.Game.Units.Abstractions.Swarming;
using UnityEngine;

namespace Potman.Game.Units.Swarming
{
    public class SwarmAgent : ISwarmAgent
    {
        private static int agentsCounter;
        private readonly IUnitViewModel viewModel;
        private readonly ISwarmMatchSystem matchSystem;
        private readonly IPool<IUnitPath> pathPool;

        public event Action OnMasterChanged;
        public event Action<IUnitPath> OnPathChanged;
        public int Id { get; } = agentsCounter++;
        public int MatchType { get; private set; }
        public float MatchCoef { get; private set; }
        public bool Enabled { get; private set; }
        public bool IsMaster => Master?.Agent == null || Master.Agent == this;
        public Vector3 Position => Enabled ? viewModel.Movement.Position : Vector3.zero;
        public ISwarmMaster Master { get; private set; }
        public IUnitPath UnitPath { get; private set; }

        public SwarmAgent(
            IUnitViewModel viewModel,
            ISwarmMatchSystem matchSystem,
            IPool<IUnitPath> pathPool)
        {
            this.viewModel = viewModel;
            this.matchSystem = matchSystem;
            this.pathPool = pathPool;
        }

        public void Enable(bool value)
        {
            if (Enabled == value)
                return;
            
            if (!value)
            {
                OnPathChanged = null;
                OnMasterChanged = null;
                matchSystem.Stop(this);
                Master?.Disconnect(this);
                Enabled = false;
                MatchCoef = -1;
                ReleasePaths();
                return;
            }

            Enabled = true;
            MatchType = (int)viewModel.Config.movementType;
            if (viewModel.StatsCollection.TryGet(StatType.MoveSpeed, out var spdStat))
                MatchCoef += spdStat.CurrentFloat;
            
            matchSystem.Start(this);
        }

        public void SetPath(IUnitPath unitPath)
        {
            ReleasePaths(unitPath);

            if (!Enabled)
                return;

            UnitPath = unitPath;
            OnPathChanged?.Invoke(unitPath);
        }
        
        public void SetMaster(ISwarmMaster value, ISwarmAgent previous, bool notify = true)
        {
            if (!Enabled)
                return;
            
            if (previous != null)
                previous.OnPathChanged -= SetPath;
            
            if (!IsMaster)
                Master.Agent.OnPathChanged -= SetPath;
            
            Master = value;
            
            if (!IsMaster)
                Master.Agent.OnPathChanged += SetPath;
            
            if (notify)
                OnMasterChanged?.Invoke();
            
            if (IsMaster)
                matchSystem.Start(this);
            
            Debug.LogWarning($"Master is set : {value?.Id ?? -1}");
        }

        private void ReleasePaths(IUnitPath incoming = null)
        {
            if (UnitPath?.Id == Id)
                pathPool.Release(UnitPath);

            if (!Enabled && incoming?.Id == Id)
                pathPool.Release(incoming);

            UnitPath = null;
        }
    }
}