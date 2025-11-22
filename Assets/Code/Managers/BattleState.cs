using UnityEngine;
using Game.Managers;

namespace Game.Managers
{
    public abstract class BattleState
    {
        protected BattleManager owner;

        public BattleState(BattleManager owner)
        {
            this.owner = owner;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update() { }
    }
}
