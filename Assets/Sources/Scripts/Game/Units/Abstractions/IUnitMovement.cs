using UnityEngine;

namespace Potman.Game.Units.Abstractions
{
    public interface IUnitMovement
    {
        Vector3 Position { get; }
        Quaternion Rotation { get; }
        
        bool Enabled { get; }
        bool IsStopped { get; }
        float RemainingDistance { get; }
        float StopDistance { get; set; }
        float Speed { get; set; }
        float TurnSpeed { get; set; }
        float Acceleration { get; set; }
        float Height { get; set; }
        float Radius { get; set; }

        void Enable(bool value);
        void SetSteering(Transform target);
        void MoveAuto(Vector3 worldPoint);
        void MoveRelative(Vector3 worldPoint);
        void Move(Vector3 worldPoint);
        void Stop();
    }
}