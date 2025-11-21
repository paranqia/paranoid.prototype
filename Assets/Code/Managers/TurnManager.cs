using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.Gameplay;

namespace Game.Managers
{
    public class TurnManager : MonoBehaviour
    {
        public List<Unit> allUnits = new List<Unit>();

        private Queue<Unit> turnQueue = new Queue<Unit>();

        public void SetupTurnOrder(List<Unit> unitsInBattle)
        {
            allUnits = unitsInBattle;

            foreach (var unit in allUnits)
            {
                unit.Initialize();
            }

            var sortedUnits = allUnits.OrderByDescending(u => u.currentAgility).ToList();

            turnQueue.Clear();
            foreach (var unit in sortedUnits)
            {
                turnQueue.Enqueue(unit);
                Debug.Log($"Queue: {unit.unitName} (SPD: {unit.currentAgility})");
            }
        }
        
        public Unit GetNextUnit()
        {
            if (turnQueue.Count > 0)
            {
                return turnQueue.Dequeue();
            }
            return null;
        }

    }
}