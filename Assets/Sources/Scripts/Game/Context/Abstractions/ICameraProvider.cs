using UnityEngine;

namespace Potman.Game.Context.Abstractions
{
    public interface ICameraProvider
    {
        Camera Camera { get; }
        void SetupPlayerCameraRig(Transform root);
    }
}