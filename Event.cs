using System.Collections.Generic;

namespace Relay
{
    /// <summary>
    /// Events are sent to entities and propagated through each component within the entity.
    /// If the entity is not the target, then the event is not handled at all.
    /// Each component handles the event and returns true or false, if false then we stop propagating the event to the other components.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// The event type.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// The event data.
        /// </summary>
        public EventData Data { get; set; }

        /// <summary>
        /// Whether the event has a specific entity target or not.
        /// </summary>
        public bool HasTarget => Target != null;

        /// <summary>
        /// Target for the event (optional).
        /// </summary>
        public Entity Target { get; private set; } = null;

        /// <summary>
        /// Indexer to access event data directly.
        /// </summary>
        public object this[string key]
        {
            get => Data[key];
            set => Data[key] = value;
        }

        public Event(string type, Entity target = null)
        {
            Type = type;
            Data = new EventData();
            Target = target;
        }

        public Event(string type, EventData data, Entity target = null)
        {
            Type = type;
            Data = data;
            Target = target;
        }
    }

    public class EventData : Dictionary<string, object> { }
}
