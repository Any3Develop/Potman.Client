using Potman.Game.Stats.Abstractions;
using Potman.Game.Stats.Data;
using Potman.Game.Stats.Utils;
using Potman.Game.Units.Abstractions;
using R3;
using UnityEngine;

namespace Potman.Game.Units
{
    public class UnitViewFollower : MonoBehaviour, IUnitViewFollower
    {
        public bool Enabled { get; private set; }
        public float SpeedDumping { get; private set; }
        public float TurnDumping { get; private set; }
        public float Altitude { get; private set; }
        public IUnitView View { get; private set; }

        private CompositeDisposable disposables;
        private Transform selfCached;
        private Transform steering;

        public void Enable(bool value)
        {
            if (Enabled == value)
                return;

            Enabled = value;
            if (value)
            {
                selfCached = transform;
                View = GetComponent<IUnitView>();
                steering = View.ViewModel.Root;
                disposables?.Dispose();
                disposables = new CompositeDisposable();
                
                var stats = View.ViewModel.StatsCollection;
                stats.SubscribeInitStat(StatType.FlyDumping, stat => SpeedDumping = CalcDumping(stat, StatType.MoveSpeed))
                    .AddTo(disposables);
                
                stats.SubscribeInitStat(StatType.FlyAltitude, stat => Altitude = stat.CurrentFloat)
                    .AddTo(disposables);
                
                stats.SubscribeInitStat(StatType.FlyTrunDumping, stat => TurnDumping = CalcDumping(stat, StatType.MoveTurnSpeed))
                    .AddTo(disposables);
                return;
            }

            Release();
        }
        
        private float CalcDumping(IRuntimeStat dumpingStat, StatType baseType)
        {
            if (!Enabled || dumpingStat == null || View?.ViewModel == null || !View.ViewModel.StatsCollection.TryGet(baseType, out var baseStat))
                return 0;

            return baseStat.CurrentFloat * dumpingStat.CurrentFloat;
        }

        private void Update()
        {
            if (!Enabled)
                return;

            var targetPos = steering.position;
            targetPos.y = Altitude;
            var delta = Time.deltaTime;
            var targetRot = Quaternion.LookRotation(steering.forward);
            var currRot = selfCached.rotation;

            var maxAngleDifference = Quaternion.Angle(currRot, targetRot);
            var t = Mathf.Min(1f, TurnDumping * Time.deltaTime / maxAngleDifference);
            selfCached.position = Vector3.Lerp(selfCached.position, targetPos, SpeedDumping * delta);
            selfCached.rotation = Quaternion.Slerp(currRot, targetRot, t);
        }

        private void Release()
        {
            Enabled = false;
            disposables?.Dispose();
            disposables = null;
            selfCached = null;
            steering = null;
            View = null;
        }

        private void OnDestroy() => Release();
    }
}