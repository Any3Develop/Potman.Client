using System;
using Potman.Common.Pools;
using Potman.Game.Units.Abstractions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Potman.Game.Units
{
    public class UnitView : PoolableView, IUnitView
    {
        [SerializeField] private string id = Guid.NewGuid().ToString();
        [SerializeField] private MeshRenderer graphics; // TODO FOR TEST
        
        public string Id => id;
        public IUnitViewModel ViewModel { get; private set; }
        private bool initedColor; // TODO FOR TEST

        public void Init(IUnitViewModel viewModel)
        {
            if (DisposedLog())
                return;

            ViewModel = viewModel;
        }

        protected override void OnSpawned()
        {
            if (!initedColor) // TODO FOR TEST
            {
                initedColor = true;
                var newColor = Color.Lerp(Random.value > 0.5f ? Color.red : Color.cyan,
                    Random.value > 0.5f ? Color.green : Color.yellow, Random.value);

                foreach (var renderers in graphics.GetComponentsInChildren<MeshRenderer>())
                    renderers.material.color = newColor;
            }
        }

        protected override void OnReleased()
        {
            Root.SetParent(null);
            ViewModel = null;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            id = Guid.NewGuid().ToString();
        }
    }
}