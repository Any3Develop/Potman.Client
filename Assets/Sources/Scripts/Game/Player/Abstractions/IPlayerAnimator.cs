using System;
using UnityEngine;

namespace Potman.Game.Player.Abstractions
{
    public interface IPlayerAnimator : IDisposable
    {
        IPlayerViewModel ViewModel { get; }
        bool Enabled { get; }
        
        void Enable(bool value);
        void UpdateMove(Vector3 dir);
    }
}