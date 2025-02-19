using Potman.Game.Units.Abstractions;
using UnityEngine;

namespace Potman.Game.Units
{
    public class UnitAvationViewModel : UnitViewModel
    {
        private IUnitViewFollower viewFollower;

        protected override void OnInit(Vector3 position)
        {
            Movement.Move(position);
            View.Container.TryGetComponent(out viewFollower);
            View.Root.position = position;
        }

        protected override void OnSpawned()
        {
            viewFollower?.Enable(true);
            base.OnSpawned();
        }

        protected override void OnReleased()
        {
            viewFollower?.Enable(false);
            viewFollower = null;
            base.OnReleased();
        }

        protected override void OnDisposed()
        {
            viewFollower = null;
            base.OnDisposed();
        }
    }
}