using System;
using Potman.Game.Stats.Data;
using Potman.Game.Stats.Utils;
using Potman.Game.Units.Abstractions;
using Potman.Game.Units.Abstractions.Swarming;
using Potman.Game.Units.Utils;
using R3;
using UnityEngine;
using UnityEngine.AI;

namespace Potman.Game.Units
{
    public class UnitNavmeshMovement : IUnitMovement
    {
        private readonly IUnitViewModel viewModel;
        private readonly IUnitPathFactory unitPathFactory;
        private readonly NavMeshAgent navMeshAgent;
        private readonly Transform navTransform;
        private Transform steeringTarget;
        
        private ISwarmAgent SwarmAgent => viewModel.SwarmAgent;
        private IDisposable steering;
        private IDisposable disposables;

        #region Properties
        public Vector3 Position => navTransform.position;
        public Quaternion Rotation => navTransform.rotation;
        public bool Enabled => navMeshAgent != null && navMeshAgent.enabled;
        public bool IsStopped => !Enabled || navMeshAgent.isStopped;
        public float RemainingDistance => Enabled && steeringTarget ? Vector3.Distance(steeringTarget.position, Position) : float.MaxValue;

        public float StopDistance
        {
            get => navMeshAgent.stoppingDistance;
            set => navMeshAgent.stoppingDistance = value;
        }

        public float Speed
        {
            get => navMeshAgent.speed;
            set => navMeshAgent.speed = value;
        }

        public float TurnSpeed
        {
            get => navMeshAgent.angularSpeed;
            set => navMeshAgent.angularSpeed = value;
        }

        public float Acceleration
        {
            get => navMeshAgent.acceleration;
            set => navMeshAgent.acceleration = value;
        }

        public float Height
        {
            get => navMeshAgent.height;
            set => navMeshAgent.height = value;
        }

        public float Radius
        {
            get => navMeshAgent.radius;
            set => navMeshAgent.radius = value;
        }

        public int Priority
        {
            get => navMeshAgent.avoidancePriority;
            set => navMeshAgent.avoidancePriority = value;
        }
#endregion

        public UnitNavmeshMovement(
            NavMeshAgent navMeshAgent,
            IUnitViewModel viewModel,
            IUnitPathFactory unitPathFactory)
        {
            this.navMeshAgent = navMeshAgent;
            this.viewModel = viewModel;
            this.unitPathFactory = unitPathFactory;
            navTransform = navMeshAgent.transform;
        }

        public void Enable(bool value)
        {
            if (Enabled == value)
                return;

            if (value)
            {
                navMeshAgent.enabled = true;
                OnEnabled();
                Stop();
                return;
            }

            Stop();
            OnDisabled();
            navMeshAgent.enabled = false;
        }

        public void SetSteering(Transform target)
        {
            if (!Enabled)
                return;
            
            Stop();
            
            if (!target)
                return;

            steeringTarget = target;
            steering = Observable.Interval(TimeSpan.FromSeconds(1/60f*10)).Subscribe(_ =>
            {
                if (steeringTarget)
                    MoveAuto(steeringTarget.position);
            });
        }

        private IUnitPath localTestPath;
        public void MoveAuto(Vector3 worldPoint)
        {
            if (!Enabled)
                return;
            
            // TODO FOR TEST
            localTestPath ??= unitPathFactory.Create(SwarmAgent.Id);
            if (!navMeshAgent.CalculatePath(worldPoint, localTestPath.Value))
                Debug.LogError($"Swarm path not calculated : {localTestPath.Value.status}, OwnerId : {localTestPath.Id}");

            OnPathChanged(localTestPath);
            // TODO FOR TEST
            
            return; // TODO FOR TEST
            if (SwarmAgent.IsMaster)
            {
                var path = unitPathFactory.Create(SwarmAgent.Id);
                if (!navMeshAgent.CalculatePath(worldPoint, path.Value))
                    Debug.LogError($"Swarm path not calculated : {path.Value.status}, OwnerId : {path.Id}");

                SwarmAgent.SetPath(path);
                return;
            }
            
            OnPathChanged(SwarmAgent.UnitPath);
        }

        public void MoveRelative(Vector3 worldPoint)
        {
            if (!Enabled)
                return;

            navMeshAgent.Move(worldPoint - Position);
        }

        public void Move(Vector3 worldPoint)
        {
            navTransform.position = worldPoint;
            AdjustPosition();
        }

        public void Stop()
        {
            steering?.Dispose();
            steering = null;
            
            if (!navMeshAgent.isOnNavMesh)
                return;

            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }

        private void OnPathChanged(IUnitPath unitPath)
        {
            if (!Enabled || unitPath is not {Available: true})
            {
                Debug.Log($"OnPathChanged cancelled : Enabled:{Enabled}, Path.Available:{unitPath?.Available == true}, Group:{SwarmAgent?.Master?.Id ?? 0}");
                return;
            }
            
            navMeshAgent.SetPath(unitPath.Value);
            navMeshAgent.isStopped = false;
        }

        private void OnMasterChanged()
        {
            if (!Enabled)
                return;
            
            if (steeringTarget)
                MoveAuto(steeringTarget.position);
        }

        private void AdjustPosition()
        {
            if (NavMesh.SamplePosition(navTransform.position, out var closestHit, 500f, navMeshAgent.areaMask))
                navTransform.position = closestHit.position;
        }

        private void OnEnabled()
        {
            var config = viewModel.Config;
            var stats = viewModel.StatsCollection;

            navMeshAgent.autoRepath = false;
            navMeshAgent.agentTypeID = config.movementType.AsAgentId();
            navMeshAgent.areaMask = config.walkableAreas;

            using var eventBuilder = Disposable.CreateBuilder();
            eventBuilder.Add(stats.SubscribeInitStat(StatType.MoveSpeed, stat => Speed = stat.CurrentFloat));
            eventBuilder.Add(stats.SubscribeInitStat(StatType.MoveTurnSpeed, stat => TurnSpeed = stat.CurrentFloat));
            eventBuilder.Add(stats.SubscribeInitStat(StatType.MoveAcceleration, stat => Acceleration = stat.CurrentFloat));
            eventBuilder.Add(stats.SubscribeInitStat(StatType.AgentHeght, stat => Height = stat.CurrentFloat));
            eventBuilder.Add(stats.SubscribeInitStat(StatType.AgentRadius, stat => Radius = stat.CurrentFloat));
            eventBuilder.Add(stats.SubscribeInitStat(StatType.AgentPriority, stat => Priority = stat.Current));
            SwarmAgent.OnPathChanged += OnPathChanged;
            SwarmAgent.OnMasterChanged += OnMasterChanged;
            disposables = eventBuilder.Build();
            
            AdjustPosition();
        }

        private void OnDisabled()
        {
            SwarmAgent.OnPathChanged -= OnPathChanged;
            SwarmAgent.OnMasterChanged -= OnMasterChanged;
            
            steering?.Dispose();
            steering = null;
            disposables?.Dispose();
            disposables = null;
        }
    }
}