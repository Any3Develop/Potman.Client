using Potman.Game.Player.Abstractions;
using UnityEngine;

namespace Potman.Game.Player
{
    public class PlayerView : MonoBehaviour, IPlayerView
    {
        [field:SerializeField] public Transform Root { get; private set; }
        [field:SerializeField] public GameObject Container { get; private set; }
        public IPlayerViewModel ViewModel { get; private set; }

        public void Init(IPlayerViewModel viewModel)
        {
            ViewModel = viewModel;
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
            ViewModel = null;
        }
    }
}