using Potman.Game.Player.Abstractions;
using UnityEngine;

namespace Potman.Game.Player
{
    public class PlayerAnimator : IPlayerAnimator
    {
        private readonly Animator animator;

        private static readonly int VelocityX = Animator.StringToHash("VelocityX");
        private static readonly int VelocityY = Animator.StringToHash("VelocityY");

        public IPlayerViewModel ViewModel { get; }
        public bool Enabled { get; private set; }

        public PlayerAnimator(
            IPlayerViewModel viewModel)
        {
            ViewModel = viewModel;
            animator = ViewModel.View.Container.GetComponentInChildren<Animator>(true);
        }

        public void Enable(bool value)
        {
            if (Enabled == value)
                return;

            Enabled = value;
            if (value)
                return;

            animator.StopPlayback();
        }

        public void UpdateMove(Vector3 dir)
        {
            animator.SetFloat(VelocityX, dir.x);
            animator.SetFloat(VelocityY, dir.z);
        }

        public void Dispose() {}
    }
}