using System;

namespace Game.Core
{
    public static class EventBus
    {
        public static event Action<GameState> OnGameStateChanged;

        public static void PublishGameStateChange(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);
        }
    }
}