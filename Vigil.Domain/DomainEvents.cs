using System;
using System.Collections.Generic;
using System.Linq;

namespace Vigil
{
    public static class DomainEvents
    {
        [ThreadStatic]
        private static List<Delegate> actions;

        public static void Register<T>(Action<T> callback) where T : IDomainEvent
        {
            if (actions == null)
            {
                actions = new List<Delegate>();
            }
            actions.Add(callback);
        }

        public static void ClearCallbacks()
        {
            actions = null;
        }

        public static void Raise<T>(T args) where T : IDomainEvent
        {
            if (actions != null)
            {
                var these = actions.OfType<Action<T>>();
                foreach (Action<T> action in actions)
                {
                    action(args);
                }
            }
        }
    }
}
