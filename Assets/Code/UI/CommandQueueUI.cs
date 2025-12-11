using UnityEngine;
using UnityEngine.UI;
using Game.Gameplay;
using Game.Core;
using System.Collections.Generic;

namespace Game.UI
{
    public class CommandQueueUI : MonoBehaviour
    {
        [Header("Config")]
        public GameObject slotPrefab;
        public Transform slotsContainer;
        public int maxSlots = 3;

        private Unit currentUnit;
        private List<GameObject> activeSlots = new List<GameObject>();

        private void OnEnable()
        {
            EventBus.Subscribe<CommandAddedEvent>(OnCommandAdded);
            // EventBus.Subscribe<UnitSelectedEvent>(OnUnitSelected); // Wait for this
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<CommandAddedEvent>(OnCommandAdded);
        }

        // Called by BattleUI when active unit changes
        public void SetUnit(Unit unit)
        {
            currentUnit = unit;
            Refresh();
        }

        private void OnCommandAdded(CommandAddedEvent evt)
        {
            if (evt.Unit == currentUnit)
            {
                Refresh();
            }
        }

        public void Refresh()
        {
            // Clear old slots
            foreach (Transform child in slotsContainer)
            {
                Destroy(child.gameObject);
            }
            activeSlots.Clear();

            if (currentUnit == null) return;

            // Create new slots
            foreach (var cmd in currentUnit.plannedCommands)
            {
                GameObject slot = Instantiate(slotPrefab, slotsContainer);
                Text slotText = slot.GetComponentInChildren<Text>();
                if (slotText)
                {
                    // Basic Icon/Text Mapping
                    string txt = "?";
                    if (cmd is Game.Gameplay.BattleActions.AttackCommand) txt = "ATK";
                    else if (cmd is Game.Gameplay.BattleActions.DefendCommand) txt = "DEF";
                    else if (cmd is Game.Gameplay.BattleActions.AnalysisCommand) txt = "ANA";
                    
                    slotText.text = txt;
                }
                activeSlots.Add(slot);
            }
        }
    }
}

