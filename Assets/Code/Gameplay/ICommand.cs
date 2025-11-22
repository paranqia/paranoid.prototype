using System.Collections;
using Game.Gameplay;

namespace Game.Gameplay
{
    public enum CommandPriority
    {
        Normal = 0,
        High = 10,      // Fast Cast
        Immediate = 20  // Interrupts
    }

    [System.Flags]
    public enum CommandTags
    {
        None = 0,
        Melee = 1 << 0,
        Ranged = 1 << 1,
        Magic = 1 << 2,
        Interrupt = 1 << 3,
        Defensive = 1 << 4
    }

    public interface ICommand
    {
        Unit Owner { get; }
        CommandPriority Priority { get; }
        CommandTags Tags { get; }

        /// <summary>
        /// Executes the command. Returns an IEnumerator for coroutine handling (animations).
        /// </summary>
        IEnumerator Execute();
    }
}
