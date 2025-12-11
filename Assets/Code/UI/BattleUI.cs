using UnityEngine;
using Game.Managers;
using Game.Gameplay;
using System.Collections.Generic;
using Game.Core;

namespace Game.UI
{
    public class BattleUI : MonoBehaviour
    {
        [Header("Components")]
        public Transform partyPanelContainer;
        public GameObject unitStatusPrefab;
        public CommandQueueUI commandQueueUI;
        public HandUI handUI;

        [Header("Runtime")]
        private List<UnitStatusUI> statusUIs = new List<UnitStatusUI>();
        private Unit currentSelectedUnit;

        private void OnEnable()
        {
            EventBus.Subscribe<UnitSelectedEvent>(OnUnitSelectedEvent);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<UnitSelectedEvent>(OnUnitSelectedEvent);
        }

        private void Start()
        {
            // Wait a frame for BattleManager to init?
            // Or assume setup is done.
            Initialize();
        }
        
        private void Update()
        {
            // TEMPORARY: Poll for selected unit from PlayerTurnState?
            // Ideally we use events.
            // Removed polling in favor of events.
        }

        public void Initialize()
        {
            if (BattleManager.Instance == null) return;

            // Clear dummy
            foreach (Transform child in partyPanelContainer) Destroy(child.gameObject);
            statusUIs.Clear();

            // Create Status UI for each Player Unit
            foreach (var unit in BattleManager.Instance.PlayerParty)
            {
                GameObject obj = Instantiate(unitStatusPrefab, partyPanelContainer);
                UnitStatusUI ui = obj.GetComponent<UnitStatusUI>();
                if (ui)
                {
                    ui.Bind(unit);
                    statusUIs.Add(ui);
                }
            }
        }
        
        private void OnUnitSelectedEvent(UnitSelectedEvent evt)
        {
            if (currentSelectedUnit == evt.SelectedUnit) return;
            currentSelectedUnit = evt.SelectedUnit;

            // Highlight in HUD
            foreach (var ui in statusUIs)
            {
                ui.SetSelected(ui.targetUnit == currentSelectedUnit);
            }

            // Update Command Queue
            if (commandQueueUI)
            {
                commandQueueUI.SetUnit(currentSelectedUnit);
            }
        }
    }
}
