using Potman.Common.InputSystem.Abstractions;
using Potman.Common.Utilities;
using Potman.Game.Context.Abstractions;
using Potman.Game.Player.Abstractions;
using Potman.Game.Player.Data;
using UnityEngine;

namespace Potman.Game.Player
{
    public interface IAimPositioning
    {
        bool Enabled { get; }

        void Enable(bool value);
        bool TryGetInput(out Vector3 dir);
        void Rotate(Vector3 dir);
    }

    public class AimPositioning : IAimPositioning
    {
        private const string SpinePath = "POTMAN/mixamorig:Hips/mixamorig:Spine";
        private readonly IInputAction lookAction;
        private readonly IInputAction lookActivationAction;
        private readonly IInputController inputController;
        private readonly ICameraProvider cameraProvider;
        private readonly Transform aim;

        public bool Enabled { get; private set; }

        public AimPositioning(
            IInputController<PlayerActions> inputController,
            ICameraProvider cameraProvider,
            IPlayerViewModel playerViewModel)
        {
            lookAction = inputController.Get(PlayerActions.Look);
            lookActivationAction = inputController.Get(PlayerActions.LookActivation);
            this.cameraProvider = cameraProvider;
            aim = playerViewModel.View.Root.Find(SpinePath);
        }

        public void Enable(bool value)
        {
            if (Enabled == value)
                return;
            
            Enabled = value;
        }
        
        public void Rotate(Vector3 dir)
        {
            if (!Enabled || !aim)
                return;
            
            aim.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
        
        public bool TryGetInput(out Vector3 dir)
        {
            if (!Enabled || lookActivationAction is not { Enabled: true } || lookActivationAction.GetControlMagnitude() <= 0f)
            {
                dir = Vector3.zero;
                return false;
            }

            dir = lookAction.ReadValue<Vector2>();
            if (dir.sqrMagnitude <= 0) 
                return false;
            
#if UNITY_EDITOR || UNITY_STANDALONE // IMPORTANT: platforms with a pointer or cursor return a point on the screen. For mobile/joysticks input return the direction vector by default.
            var scrAim = cameraProvider.Camera.WorldToScreenPoint(aim.position); // get origin screen point.
            dir = (dir - scrAim).normalized; // calculate the direction between origin and point on the screen.
#endif  
            // Swap Y and Z components to convert screen (X[left/right],Y[up/down]) to (X[left/right],Z[forward/backward]) world direction, and except Y vertical direction.
            dir.Set(y: 0, z: dir.y);
            return true;
        }
    }
}
