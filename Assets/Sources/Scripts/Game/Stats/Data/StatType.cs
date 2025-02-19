namespace Potman.Game.Stats.Data
{
    public enum StatType
    {
        None = 0,
        
        // Base Stats
        BaseHeath = 20,
        BaseManna = 21,
        BaseArmor = 22,
        BaseDamage = 23,
        BaseStamina = 24,
        BaseLethality = 25,
        BaseLevel = 26,
        
        // Agent Components
        AgentHeght = 40,
        AgentRadius = 41,
        AgentPriority = 42,
        
        // Move Components
        MoveSpeed = 60,
        MoveAcceleration = 61,
        MoveDumpingFactor = 62,
        MoveTurnSpeed = 63,
        
        // Fly Components
        FlyAltitude = 80,
        FlyDumping = 81,
        FlyTrunDumping = 82,
        
        // Attack Components
        AttackDelay = 100,
        AttackSpeed = 101,
        AttackRange = 102,
        
        // Effect Components
        EffectRegenHeath = 120,
        EffectRegenArmor = 121,
        EffectRegenStamina = 122,
    }
}