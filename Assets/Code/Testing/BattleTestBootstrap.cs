using UnityEngine;
using Game.Managers;
using Game.Gameplay;
using System.Collections.Generic;

namespace Game.Testing
{
    // This script is for TESTING ONLY - It sets up a battle scene automatically
    public class BattleTestBootstrap : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("--- BATTLE TEST BOOTSTRAP STARTED ---");

            // 1. Create Managers if not exist
            if (BattleManager.Instance == null)
            {
                GameObject managerObj = new GameObject("BattleManager");
                Vector3 pos = Vector3.zero;
                BattleManager bm = managerObj.AddComponent<BattleManager>();
                managerObj.AddComponent<TimelineManager>();
                bm.deckManager = managerObj.AddComponent<DeckManager>();
                managerObj.AddComponent<FieldManager>();
                Debug.Log("[Bootstrap] Created Managers");
            }

            // 2. Create Player Unit
            GameObject playerObj = new GameObject("Player_Hero");
            Unit playerUnit = playerObj.AddComponent<Unit>();
            playerUnit.isPlayer = true;
            playerUnit.unitName = "Test Hero";
            playerUnit.maxHP = 100;
            playerUnit.currentHP = 100;
            playerUnit.currentAgility = 10;
            Debug.Log("[Bootstrap] Created Player Unit");

            // 3. Create Enemy Unit
            GameObject enemyObj = new GameObject("Enemy_Boss");
            Unit enemyUnit = enemyObj.AddComponent<Unit>();
            enemyUnit.isPlayer = false;
            enemyUnit.unitName = "Test Boss";
            enemyUnit.maxHP = 500;
            enemyUnit.currentHP = 500;
            enemyUnit.currentAgility = 5;
            Debug.Log("[Bootstrap] Created Enemy Unit");

            // 4. Manual Link
            BattleManager.Instance.SetUnits(new List<Unit> { playerUnit, enemyUnit });
            Debug.Log($"[Bootstrap] Linked Units: {BattleManager.Instance.Units.Count}");

            // 5. Check State
            Debug.Log($"[Bootstrap] Current Battle State: {BattleManager.Instance.CurrentState?.GetType().Name ?? "Null"}");

            // 6. Add Test UI
            if (GetComponent<SimpleBattleUI>() == null)
            {
                BattleManager.Instance.gameObject.AddComponent<SimpleBattleUI>();
                Debug.Log("[Bootstrap] Added SimpleBattleUI");
            }
            
            Debug.Log("--- BOOTSTRAP COMPLETE ---");
        }
    }
}
