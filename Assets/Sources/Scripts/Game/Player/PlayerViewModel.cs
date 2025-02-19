using Potman.Game.Context.Abstractions;
using Potman.Game.Player.Abstractions;
using Potman.Game.Player.Data;
using Potman.Game.Stats.Abstractions;
using System.Linq;
using UnityEngine;
using IServiceProvider = Potman.Common.DependencyInjection.IServiceProvider;

namespace Potman.Game.Player
{
    public class PlayerViewModel : MonoBehaviour, IPlayerViewModel
    {
        [field: SerializeField] public Transform Root { get; private set; }
        [field: SerializeField] public GameObject Container { get; private set; }
        public PlayerConfig Config { get; private set; }
        public IPlayerView View { get; private set; }
        public IStatsCollection StatsCollection { get; private set; }
        public IPlayerMovement PlayerMovement { get; private set; }
        public IPlayerAnimator PlayerAnimator { get; private set; }
        public IAimPositioning AimPositioning { get; private set; }
        public IServiceProvider ServiceProvider { get; private set; }

        private ICameraProvider cameraProvider;
        public void Init(IServiceProvider serviceProvider, PlayerConfig config, Vector3 position)
        {
            Config = config;
            ServiceProvider = serviceProvider;

            var statFactory = ServiceProvider.GetRequiredService<IStatFactory>();
            StatsCollection = ServiceProvider.GetRequiredService<IStatsCollection>();

            StatsCollection.AddRange(config.stats.Select(statFactory.Create));
            cameraProvider = ServiceProvider.GetRequiredService<ICameraProvider>();
            cameraProvider.SetupPlayerCameraRig(Root);

            View = ServiceProvider.GetRequiredService<IPlayerViewFactory>().Create(this);
            PlayerAnimator = ServiceProvider.GetRequiredService<IPlayerAnimatorFactory>().Create(this);
            PlayerMovement = ServiceProvider.GetRequiredService<IPlayerMovementFactory>().Create(this, position);
            AimPositioning = ServiceProvider.GetRequiredService<IPlayerAimPositioningFactory>().Create(this);
                
            PlayerAnimator.Enable(true);
            PlayerMovement.Enable(true);
            AimPositioning.Enable(true);
        }
        private void LateUpdate()
        {
            if (!AimPositioning.TryGetInput(out var direction))
                return;
            
            AimPositioning.Rotate(direction);
        }

        private void OnValidate()
        {
            if (!Root)
                Root = transform;

            if (!Container)
                Container = gameObject;
        }

        private void OnDestroy()
        {
            View = null;
            Config = null;
            PlayerMovement?.Dispose();
            PlayerAnimator?.Dispose();
            StatsCollection?.Clear();
            StatsCollection = null;
            PlayerMovement = null;
            PlayerAnimator = null;
        }
    }
}