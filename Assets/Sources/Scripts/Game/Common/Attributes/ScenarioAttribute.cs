using System;
using Potman.Game.Scenarios.Data;

namespace Potman.Game.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScenarioAttribute : Attribute
    {
        public ScenarioId Id { get; }
        public ScenarioAttribute(ScenarioId id) => Id = id;
    }
}