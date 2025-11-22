using UnityEngine;
using Game.Gameplay; // For Unit reference
using Game.Core; // For Enums (GameState, SanityState, FieldState)

namespace Game.Gameplay
{
    // --- State Events ---
    public struct GameStateChangedEvent
    {
        public GameState NewState;
        public GameStateChangedEvent(GameState newState) { NewState = newState; }
    }

    // --- Turn / Timeline Events ---
    public struct TurnStartedEvent
    {
        public Unit Actor; // Who's turn is it?
        public TurnStartedEvent(Unit actor) { Actor = actor; }
    }

    public struct TurnEndedEvent
    {
        public Unit Actor;
        public TurnEndedEvent(Unit actor) { Actor = actor; }
    }

    public struct ActionInterruptedEvent
    {
        public Unit Target;
        public string OriginalActionName;
        public string Reason; // e.g. "Stunned", "Cancelled"
        public ActionInterruptedEvent(Unit target, string actionName, string reason)
        {
            Target = target;
            OriginalActionName = actionName;
            Reason = reason;
        }
    }

    // --- Combat Events ---
    public struct UnitDamagedEvent
    {
        public Unit Target;
        public Unit Attacker;
        public int DamageAmount;
        public bool IsCritical;
        
        public UnitDamagedEvent(Unit target, Unit attacker, int amount, bool isCrit)
        {
            Target = target;
            Attacker = attacker;
            DamageAmount = amount;
            IsCritical = isCrit;
        }
    }

    public struct UnitDiedEvent
    {
        public Unit DeadUnit;
        public UnitDiedEvent(Unit unit) { DeadUnit = unit; }
    }

    // --- Stat / Resource Events ---
    public struct SanityChangedEvent
    {
        public Unit Target;
        public int CurrentSanity;
        public int MaxSanity;
        public SanityChangedEvent(Unit target, int current, int max)
        {
            Target = target;
            CurrentSanity = current;
            MaxSanity = max;
        }
    }

    public struct SanityStateChangedEvent
    {
        public Unit Target;
        public SanityState NewState; // We will need to define SanityState enum in Unit or Core
        public SanityStateChangedEvent(Unit target, SanityState newState)
        {
            Target = target;
            NewState = newState;
        }
    }

    // --- Field Events ---
    public struct FieldStateChangedEvent
    {
        public FieldState NewState; // Need FieldState enum
        public FieldStateChangedEvent(FieldState newState) { NewState = newState; }
    }

    // --- Command / Request Events ---
    public struct RequestInterruptEvent
    {
        public Unit Target;
        public Unit Source;
        public RequestInterruptEvent(Unit target, Unit source)
        {
            Target = target;
            Source = source;
        }
    }
}
