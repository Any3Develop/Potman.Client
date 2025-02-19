using System;
using Potman.Game.Context.Abstractions;
using Potman.Game.Scenarios.Abstractions;
using Unity.Behavior;
using Unity.Properties;
using Action = Unity.Behavior.Action;

namespace Potman.Game.Units.Behaviours.Modules
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "SetSteering", story: "SetAiSteering", category: "Action", id: "d7649fb847bd7860b6a286da5aa190d9")]
    public partial class SetSteering : Action
    {
        protected override Status OnStart()
        {
            var viewModel = GameObject.GetComponent<UnitViewModel>();
            var context = viewModel.ServiceProvider.GetRequiredService<IGameContext>();
            viewModel.Movement.SetSteering(context.Player.Root);
            return Status.Success;
        }

        protected override Status OnUpdate()
        {
            return Status.Success;
        }

        protected override void OnEnd()
        {
        }
    }
}

