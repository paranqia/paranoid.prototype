using UnityEngine;
using System.Collections.Generic;
using Game.Core;

namespace Game.Gameplay
{
    public class Unit : MonoBehaviour
    {
        [Header("Data Blueprint")]
        public CharactersData data;

        [Header("Live Stats")]
        public string unitName;
        public int currentAgility;

        [Header("Battle Logic")]
        public List<ActionType> plannedActions = new List<ActionType>();
        
        public void AddAction(ActionType action)
        {
            if (plannedActions.Count < 3)
            {
                plannedActions.Add(action);
                Debug.Log($"{unitName} planned: {action}");
            }
        }

        public void ClearActions()
        {
            plannedActions.Clear();
        }

        public void Initialize()
        {
            if (data != null)
            {
                unitName = data.CharacterName;
                currentAgility = data.agility;
                
                Debug.Log($"Unit Spawned: {unitName} (SPD: {currentAgility})");
            }
        }
    }
}