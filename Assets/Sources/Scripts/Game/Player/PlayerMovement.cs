using System;
using Potman.Common.Events;
using Potman.Common.InputSystem.Abstractions;
using Potman.Common.Utilities;
using Potman.Game.Player.Abstractions;
using Potman.Game.Player.Data;
using Potman.Game.Scenarios.Data;
using Potman.Game.Scenarios.Events;
using Potman.Game.Stats.Data;
using Potman.Game.Stats.Utils;
using R3;
using Unity.Cinemachine;
using UnityEngine;

namespace Potman.Game.Player
{
    public class PlayerMovement : IPlayerMovement
    {
        private readonly IInputAction moveAction;
        private readonly Rigidbody target;
        private Vector3 moveDir;
        
        private IDisposable disposables;
        private bool isPaused;
        
        public IPlayerViewModel ViewModel { get; }
        public bool Enabled { get; private set; }
        public float Speed { get; private set; }
        public float Acceleration { get; private set; }
        public float DumpingFactor { get; private set; }
        public float TurnSpeed { get; private set; }
        public Vector3 Velocity => Enabled ? target.linearVelocity : Vector3.zero;

        public PlayerMovement(IPlayerViewModel viewModel, IInputController<PlayerActions> input, Vector3 initialPosition)
        {
            ViewModel = viewModel;
            moveAction = input.Get(PlayerActions.Move);
            target = ViewModel.Container.GetComponent<Rigidbody>();
            moveDir = target.transform.forward;
            target.position = initialPosition;
        }

        public void Enable(bool value)
        {
            if (Enabled == value)
                return;
            
            if (value)
            {
                using var eventBuilder = Disposable.CreateBuilder();
                var stats = ViewModel.StatsCollection;
                
               eventBuilder.Add(MessageBroker.Receive<SenarioChangedEvent>().Subscribe(OnScenarioChanged));
               eventBuilder.Add(stats.SubscribeInitStat(StatType.MoveSpeed, stat => Speed = stat.CurrentFloat));
               eventBuilder.Add(stats.SubscribeInitStat(StatType.MoveAcceleration, stat => Acceleration = stat.CurrentFloat));
               eventBuilder.Add(stats.SubscribeInitStat(StatType.MoveDumpingFactor, stat => DumpingFactor = stat.CurrentFloat));
               eventBuilder.Add(stats.SubscribeInitStat(StatType.MoveTurnSpeed, stat => TurnSpeed = stat.CurrentFloat));
               eventBuilder.Add(Observable.EveryUpdate(UnityFrameProvider.FixedUpdate).Subscribe(_ => FixedUpdate()));
                
                disposables = eventBuilder.Build();
                moveAction.Enable();
                Enabled = true;
                return;
            }

            Release();
        }

        private void FixedUpdate()
        {
            if (isPaused)
                return;
            
            var input = moveAction.ReadValue<Vector2>();
            
            if (input.magnitude > 0)
            {
                moveDir = new Vector3(input.x, 0, input.y);
                target.AddForce(moveDir * Acceleration, ForceMode.Acceleration);
            }
            else
                target.linearVelocity *= DumpingFactor;

            if (target.linearVelocity.magnitude > Speed) 
                target.linearVelocity = target.linearVelocity.normalized * Speed;


            var velocity = target.linearVelocity;
            var velAbs = velocity.Abs();
            if (velAbs.z >= velAbs.x)
                velocity.Set(z:velAbs.z);
            else
                velocity.Set(z:velAbs.x, x:velocity.z);

            target.rotation = Quaternion.Slerp(target.rotation, Quaternion.LookRotation(moveDir), TurnSpeed * Time.deltaTime);
            ViewModel.PlayerAnimator.UpdateMove(velocity);
        }

        private void Release()
        {
            Enabled = false;
            moveAction?.Disable();
            disposables?.Dispose();
            disposables = null;
        }

        private void OnScenarioChanged(SenarioChangedEvent evData)
        {
            isPaused = evData.Current.AnyFlags(ScenarioState.Paused);
        }

        public void Dispose() => Release();
    }
}