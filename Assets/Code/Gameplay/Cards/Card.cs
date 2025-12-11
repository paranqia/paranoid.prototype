using UnityEngine;
using Game.Core;
using Game.Gameplay.BattleActions;
using System.Collections.Generic;

namespace Game.Gameplay.Cards
{
    [System.Serializable]
    public class Card
    {
        public CardData Data { get; private set; }
        public Unit Owner { get; private set; }
        
        // Unique ID for this instance?
        public string InstanceID { get; private set; }

        public Card(CardData data, Unit owner)
        {
            Data = data;
            Owner = owner;
            InstanceID = System.Guid.NewGuid().ToString();
        }

        public void Play(Unit target)
        {
            Debug.Log($"{Owner.unitName} plays {Data.cardName} on {target?.unitName ?? "Self/None"}");
            
            // Publish Event for FieldManager and others
            EventBus.Publish(new Game.Gameplay.CardPlayedEvent(Owner, this));

            foreach (var actionData in Data.actions)
            {
                ICommand cmd = CreateCommand(actionData, target);
                if (cmd != null)
                {
                    Owner.AddCommand(cmd);
                }
            }
        }

        private ICommand CreateCommand(CardActionData action, Unit target)
        {
            switch (action.actionType)
            {
                case ActionType.Attack:
                    // Pass element from Card Data
                    return new AttackCommand(Owner, target, Data.element);
                
                case ActionType.Defend:
                    // TODO: Pass action.value as shield
                    return new DefendCommand(Owner);
                
                case ActionType.Analyze:
                    return new AnalysisCommand(Owner);
                
                case ActionType.Skill:
                    // Placeholder for skill logic
                    Debug.LogWarning("Skill action not yet implemented.");
                    return null;
                    
                default:
                    return null;
            }
        }
    }
}
