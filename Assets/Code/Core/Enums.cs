namespace Game.Core
{
    public enum GameState
    {
        BattleStart,
        PlanningPhase,
        ExecutionPhase,
        ResolutionPhase,
        Victory,
        Defeat
    }

    public enum SanityState
    {
        Lucid,      // 70-100%
        Strained,   // 30-69%
        Fractured   // <30%
    }

    public enum FieldState
    {
        Neutral,
        LogosDominance,   // Order
        IllogicDominance, // Chaos
        NihilDominance    // Void
    }

    public enum CharacterClass
    {
        Warlord,
        Magister,
        Aberrant,
        None
    }

    public enum Element
    {
        Nihil,
        Logos,
        Illogic,
        None
    }
}
