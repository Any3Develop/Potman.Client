using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Potman.Common.DependencyInjection;
using Potman.Game.Common.Attributes;
using Potman.Game.Scenarios.Abstractions;
using Potman.Game.Scenarios.Data;

namespace Potman.Game.Scenarios
{
    public class ScenarioFactory : IScenarioFactory
    {
        private readonly IAbstractFactory abstractFactory;
        private readonly Dictionary<ScenarioId, Type> scenarios;

        public ScenarioFactory(IAbstractFactory abstractFactory)
        {
            this.abstractFactory = abstractFactory;
            var baseType = typeof(IScenario);
            var attrType = typeof(ScenarioAttribute);
            
            scenarios = baseType.Assembly.GetTypes()
                .Where(type => baseType.IsAssignableFrom(type) && type.CustomAttributes.Any(x => x.AttributeType == attrType))
                .ToDictionary(key => key.GetCustomAttribute<ScenarioAttribute>().Id, value => value);
        }
        
        public IScenario Create(ScenarioConfig config)
        {
            if (!scenarios.TryGetValue(config.id, out var type))
                throw new NullReferenceException($"Can't create a {nameof(IScenario)} with id : {config.id}, because it's not registered.");

            return (IScenario)abstractFactory.Create(type, config);
        }
    }
}