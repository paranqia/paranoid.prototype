using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Core;
using Game.Gameplay; // For ICommand

namespace Game.Managers
{
    public class TimelineManager : MonoBehaviour
    {
        public static TimelineManager Instance { get; private set; }

        [Header("Debug")]
        [SerializeField] private List<string> debugQueue = new List<string>(); // For Inspector view

        private List<ICommand> executionQueue = new List<ICommand>();
        private bool isExecuting = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            EventBus.Subscribe<RequestInterruptEvent>(OnInterruptRequested);
        }

        private void OnDestroy()
        {
            EventBus.Unsubscribe<RequestInterruptEvent>(OnInterruptRequested);
        }

        private void OnInterruptRequested(RequestInterruptEvent evt)
        {
            bool success = TryInterruptUnit(evt.Target);
            if (success)
            {
                Debug.Log("INTERRUPT SUCCESSFUL!");
                EventBus.Publish(new ActionInterruptedEvent(evt.Target, "Unknown", "Interrupted by " + evt.Source.unitName));
            }
            else
            {
                Debug.Log("Interrupt failed (No pending actions).");
            }
        }

        public void AddCommand(ICommand command)
        {
            // Simple add for now. 
            // In future, we can insert based on Priority (e.g. Fast Cast).
            if (command.Priority == CommandPriority.Immediate)
            {
                executionQueue.Insert(0, command); // Add to front? Or execute immediately?
                // For "Immediate", we might want to run it NOW. 
                // But for safety, let's put it at index 0.
            }
            else if (command.Priority == CommandPriority.High)
            {
                // Find last high priority and insert after? 
                // For MVP, just add to front of normal commands.
                executionQueue.Insert(0, command);
            }
            else
            {
                executionQueue.Add(command);
            }
            
            UpdateDebugQueue();
        }

        public void ClearQueue()
        {
            executionQueue.Clear();
            UpdateDebugQueue();
        }

        public bool TryInterruptUnit(Unit target)
        {
            // Find the first command belonging to the target
            for (int i = 0; i < executionQueue.Count; i++)
            {
                if (executionQueue[i].Owner == target)
                {
                    Debug.Log($"TimelineManager: Interrupted {target.unitName}'s command at index {i}.");
                    executionQueue.RemoveAt(i);
                    UpdateDebugQueue();
                    return true;
                }
            }
            return false;
        }

        public IEnumerator ExecuteQueue()
        {
            if (isExecuting) yield break;
            isExecuting = true;

            Debug.Log("TimelineManager: Starting Execution Phase...");

            while (executionQueue.Count > 0)
            {
                // Get next command
                ICommand currentCommand = executionQueue[0];
                executionQueue.RemoveAt(0);
                UpdateDebugQueue();

                if (currentCommand != null)
                {
                    // Check if Owner is still alive/active
                    if (currentCommand.Owner != null && currentCommand.Owner.gameObject.activeInHierarchy)
                    {
                        // Execute and wait
                        yield return StartCoroutine(currentCommand.Execute());
                    }
                    else
                    {
                        Debug.Log($"Skipping command for {currentCommand.Owner?.unitName ?? "null"} (Dead/Inactive).");
                    }
                }
            }

            isExecuting = false;
            Debug.Log("TimelineManager: Execution Phase Complete.");
            
            // Notify BattleManager? Or BattleManager waits for this coroutine?
            // Ideally, BattleManager calls this and yields on it.
        }

        private void UpdateDebugQueue()
        {
            debugQueue.Clear();
            foreach (var cmd in executionQueue)
            {
                debugQueue.Add($"[{cmd.Priority}] {cmd.Owner.unitName}: {cmd.GetType().Name}");
            }
        }
    }
}
