using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public static class EventBus
    {
        private static Dictionary<Type, Delegate> eventTable = new Dictionary<Type, Delegate>();

        public static void Subscribe<T>(Action<T> listener)
        {
            var eventType = typeof(T);
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable[eventType] = null;
            }
            eventTable[eventType] = Delegate.Combine(eventTable[eventType], listener);
        }

        public static void Unsubscribe<T>(Action<T> listener)
        {
            var eventType = typeof(T);
            if (eventTable.ContainsKey(eventType))
            {
                var currentDel = eventTable[eventType];
                var newDel = Delegate.Remove(currentDel, listener);
                
                if (newDel == null)
                {
                    eventTable.Remove(eventType);
                }
                else
                {
                    eventTable[eventType] = newDel;
                }
            }
        }

        public static void Publish<T>(T eventMessage)
        {
            var eventType = typeof(T);
            if (eventTable.ContainsKey(eventType))
            {
                var del = eventTable[eventType];
                if (del != null)
                {
                    (del as Action<T>)?.Invoke(eventMessage);
                }
            }
        }

        public static void Clear()
        {
            eventTable.Clear();
        }
    }
}
