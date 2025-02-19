using System;

namespace Potman.Game.Player.Abstractions
{
    public interface IPlayerMovement : IDisposable
    {
        bool Enabled { get; }
        
        void Enable(bool value);
    }
}