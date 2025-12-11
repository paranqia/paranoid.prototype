using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Core;
using Game.Gameplay;
using Game.Managers;

namespace Game.UI
{
    public class ActionQueueUI : MonoBehaviour
    {
        [Header("Config")]
        public Unit targetUnit; // Assign the player unit here
        public Transform queueContainer;
        public GameObject commandIconPrefab; // Simple image/text prefab

        private void Awake()
        {
            // Sync with BattleManager FIRST before subscribing to events
            SyncWithBattleManager();
            
            EventBus.Subscribe<CommandAddedEvent>(OnCommandAdded);
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void Start()
        {
            // Double-check sync in Start (in case BattleManager wasn't ready in Awake)
            if (targetUnit == null || (BattleManager.Instance != null && targetUnit != BattleManager.Instance.PlayerUnit))
            {
                SyncWithBattleManager();
            }
        }
        
        private void SyncWithBattleManager()
        {
            if (BattleManager.Instance != null && BattleManager.Instance.PlayerUnit != null)
            {
                if (targetUnit != null && targetUnit != BattleManager.Instance.PlayerUnit)
                {
                    Debug.LogWarning($"ActionQueueUI: Overriding inspector-assigned unit '{targetUnit.unitName}' with BattleManager's PlayerUnit '{BattleManager.Instance.PlayerUnit.unitName}'");
                }
                
                targetUnit = BattleManager.Instance.PlayerUnit;
                Debug.Log($"ActionQueueUI: Linked to Player Unit: {targetUnit.unitName} (ID: {targetUnit.GetInstanceID()})");
            }
            else if (targetUnit == null)
            {
                Debug.LogError("ActionQueueUI: Could not find Player Unit via BattleManager and none assigned in Inspector!");
            }
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<CommandAddedEvent>(OnCommandAdded);
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnCommandAdded(CommandAddedEvent evt)
        {
            Debug.Log($"ActionQueueUI: Event received from {evt.Unit.unitName}. Target is {targetUnit?.unitName}");
            if (evt.Unit == targetUnit)
            {
                Debug.Log("ActionQueueUI: Unit matches! Adding visual.");
                AddCommandVisual(evt.Command);
            }
            else
            {
                Debug.LogWarning($"ActionQueueUI: Unit mismatch! Event: {evt.Unit.GetInstanceID()}, Target: {targetUnit?.GetInstanceID()}");
            }
        }

        private void AddCommandVisual(ICommand command)
        {
            if (commandIconPrefab && queueContainer)
            {
                GameObject iconObj = Instantiate(commandIconPrefab, queueContainer);
                
                // Force reset scale (sometimes UI prefabs spawn with weird scales)
                iconObj.transform.localScale = Vector3.one;
                
                Debug.Log($"ActionQueueUI: Instantiated {iconObj.name}");

                Text text = iconObj.GetComponentInChildren<Text>();
                if (text)
                {
                    text.text = command.GetType().Name.Replace("Command", "");
                    Debug.Log($"ActionQueueUI: Set text to {text.text}");
                }
                else
                {
                    Debug.LogWarning($"ActionQueueUI: No Text component found in {iconObj.name} children!");
                }
            }
            else
            {
                Debug.LogError("ActionQueueUI: Missing Prefab or Container!");
            }
        }
        
        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            // Clear the queue when entering Execution Phase
            if (evt.NewState == GameState.ExecutionPhase)
            {
                ClearQueue();
            }
        }
        
        // TODO: Clear queue visual when turn executes
        public void ClearQueue()
        {
            foreach(Transform child in queueContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
