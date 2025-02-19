using Potman.Game.Effects.Data;
using Potman.Game.Objects.Data;
using Potman.Game.Player.Data;
using Potman.Game.Units.Data;
using UnityEngine;

namespace Potman.Game.Scenarios.Data
{
    [CreateAssetMenu(order = -1, fileName = "ScenarioConfig", menuName = "Potman/Scenarios/ScenarioConfig")]
    public class ScenarioConfig : ScriptableObject
    {
        [Header("General")]
        [Tooltip("Which scenario code will execute with this config.")]
        public ScenarioId id;
        
        [Tooltip("Difficulty level of the scenario, to chose from many of configs."), Min(0)]
        public int difficulty;
        
        [Tooltip("How many time in seconds the game will playing. -1 is infinity."), Min(0)]
        public int gameTime;
        
        [Tooltip("The maximum limit of unit spawns in a scenario at all times. -1 is Infinity."), Min(0)]
        public int maxUnitsAllTheTime = 10_000;
        
        [Tooltip("A delay in Milliseconds before the game starts, when everything has loaded and the player has gained control over the character."), Min(0)]
        public int delayedStartMs = 3_000;

        [Space, Header("Providers")]
        [Tooltip("Available units and their conditions of occurrence for this scenario.")]
        public UnitSpawnConfig[] unitsScenario;

        [Tooltip("Available object and their conditions of occurrence for this scenario.")]
        public ObjectSpawnConfig[] objectsScenario;

        [Tooltip("Available effects and their conditions of occurrence for this scenario.")]
        public EffectSpawnConfig[] effectsScenario;

        [Tooltip("Scenario spawn points prototypes.")]
        public GameObject spawnPoints;

        [Tooltip("TODO")]
        public PlayerConfig playerConfig;
    }
}