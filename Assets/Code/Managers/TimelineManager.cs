using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.Core;
using Game.Gameplay; // For ICommand

namespace Game.Managers
{
    public class TimelineManager : MonoBehaviour
    {
        public static TimelineManager Instance { get; private set; }

        [Header("Debug")]
        [SerializeField] private List<string> debugQueue = new List<string>();

        // We need to group commands by Unit for sorting, 
        // OR we just store all commands and sort them based on Owner's Agility + Sequence Index.
        // For Phase-based (Type B): 
        // 1. All commands are submitted.
        // 2. We sort "Turns" based on Agility.
        // 3. Inside a Unit's Turn, commands execute sequentially (1, 2, 3).
        
        // HOWEVER, GDD says "Turn Order will follow 'Actor' (Player 1, 2, 3 and Boss) by Agility".
        // And "When it's Actor's turn, they execute 3 Actions sequentially".
        
        // So the Execution Queue should be a list of "Unit Turns", and each Unit Turn has 3 Commands.
        // BUT, Interruption logic might cancel future actions.
        
        // Let's flatten the timeline for execution simplicity, but sort primarily by Unit Speed.
        // Sort Key: [Unit Agility (Desc)] -> [Command Sequence Index (Asc)]
        
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
                Debug.Log($"INTERRUPT SUCCESSFUL on {evt.Target.unitName}!");
                EventBus.Publish(new ActionInterruptedEvent(evt.Target, "Unknown", "Interrupted by " + evt.Source.unitName));
            }
        }

        public void AddCommand(ICommand command)
        {
            executionQueue.Add(command);
            UpdateDebugQueue();
        }
        
        // Called by ExecutionState before starting execution
        public void SortTimeline()
        {
            // Sorting Logic:
            // 1. Primary: Unit Agility (Higher = Earlier)
            // 2. Secondary: Command Order/Index (Preserve sequence 1->2->3)
            // Note: We need a stable sort or explicit index tracking.
            // Since List.Sort is unstable, we use LINQ OrderBy which is stable for secondary keys if chained? 
            // Actually OrderBy is stable.
            
            // To handle "Command Sequence", we assume the order they were added to the unit's list implies sequence.
            // But here they are all mixed in executionQueue.
            // We need to know "This command is 1st of Unit A".
            // Since we AddCommand in order (1,2,3) from PlayerTurnState, 
            // the relative order for the SAME unit is already correct in the list.
            
            executionQueue = executionQueue
                .OrderByDescending(cmd => cmd.Owner.currentAgility) // Fastest units first
                .ToList();

            // Wait! The above sort might mix commands of Unit A and Unit B if they have same Agility?
            // Or if we just sort by Agility, all commands of Unit A (Agility 100) will be grouped together?
            // Yes, if Unit A has Agility 100, all 3 commands have key 100.
            // Since OrderBy is stable (in C# Linq), the original relative order (1->2->3) is preserved.
            
            // What if Unit A and B have SAME Agility?
            // The relative order between A and B depends on original list order (who submitted first).
            // That's acceptable for MVP.
            
            Debug.Log("Timeline Sorted by Agility.");
            UpdateDebugQueue();
        }

        public void ClearQueue()
        {
            executionQueue.Clear();
            UpdateDebugQueue();
        }

        public bool TryInterruptUnit(Unit target)
        {
            // Interrupt Logic: Find the *Next* command for this target and remove it (and subsequent ones?)
            // GDD says "Cancel Action in future".
            // Usually interrupt cancels the CURRENT casting action or the NEXT action.
            // Let's remove ALL remaining commands for this unit in the queue to simulate "Turn Skipped" or "Stunned".
            // Or just the next one? GDD says "Interrupt/Cancel future Action".
            
            // Let's go with: Remove ALL pending commands for this unit.
            int removedCount = 0;
            for (int i = executionQueue.Count - 1; i >= 0; i--)
            {
                if (executionQueue[i].Owner == target)
                {
                    executionQueue.RemoveAt(i);
                    removedCount++;
                }
            }
            
            if (removedCount > 0)
            {
                Debug.Log($"TimelineManager: Removed {removedCount} commands from {target.unitName}.");
                UpdateDebugQueue();
                return true;
            }
            
            return false;
        }

        public System.Collections.IEnumerator ExecuteQueue()
        {
            if (isExecuting) yield break;
            isExecuting = true;

            // Sort before executing
            SortTimeline();

            Debug.Log("TimelineManager: Starting Execution Phase...");

            // Execute one by one
            while (executionQueue.Count > 0)
            {
                // Peek first
                ICommand currentCommand = executionQueue[0];
                executionQueue.RemoveAt(0);
                UpdateDebugQueue();

                if (currentCommand != null)
                {
                    // Check active
                    if (currentCommand.Owner != null && currentCommand.Owner.gameObject.activeInHierarchy && currentCommand.Owner.currentHP > 0)
                    {
                        // Signal Turn Start for this Unit if it's their first command in the sequence?
                        // For MVP, just execute.
                        
                        // Execute
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
        }

        private void UpdateDebugQueue()
        {
            debugQueue.Clear();
            foreach (var cmd in executionQueue)
            {
                debugQueue.Add($"[SPD:{cmd.Owner.currentAgility}] {cmd.Owner.unitName}: {cmd.GetType().Name}");
            }
        }
    }
}
