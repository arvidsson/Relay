using System.Collections.Generic;

namespace Relay
{
    /// <summary>
    /// Component interface.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// The component priority - used for deciding in which order the components should handle events.
        /// </summary>
        int Priority { get; }

        void Init(Entity entity);
        void StartListening(string eventType);
        void StopListening(string eventType);
        bool IsListening(string eventType);

        bool HandleEvent(Event ev);
    }

    /// <summary>
    /// A component listens to events and handles them by doing logic and manipulating the event data.
    /// </summary>
    public abstract class Component : IComponent
    {
        /// <summary>
        /// Which entity owns the component.
        /// </summary>
        protected Entity Owner { get; private set; }

        /// <summary>
        /// Which event types the component is listening to.
        /// </summary>
        private HashSet<string> registeredEventTypes = new HashSet<string>();

        public Component()
        {
            StartListening("CreatedEntity");
            StartListening("DestroyedEntity");
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        /// <param name="entity">Which entity owns the component.</param>
        public void Init(Entity entity)
        {
            Owner = entity;
        }

        /// <summary>
        /// Adds an event type the component should listen to.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        public void StartListening(string eventType)
        {
            registeredEventTypes.Add(eventType);
        }

        /// <summary>
        /// Removes an event type the component should listen to.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        public void StopListening(string eventType)
        {
            registeredEventTypes.Remove(eventType);
        }

        /// <summary>
        /// Returns true if the component is listening to an event type.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        public bool IsListening(string eventType)
        {
            return registeredEventTypes.Contains(eventType);
        }

        public abstract int Priority { get; }

        /// <summary>
        /// Handles an event.
        /// </summary>
        /// <param name="ev">The event.</param>
        /// <returns>True if the event should be continued to be handled by other components, false if it should be stopped.</returns>
        public virtual bool HandleEvent(Event ev)
        {
            switch (ev.Type)
            {
                case "CreatedEntity":
                    OnCreate();
                    break;
                case "DestroyedEntity":
                    OnDestroy();
                    break;
                case "Tick":
                    OnTick();
                    break;
            }

            return true;
        }

        /// <summary>
        /// Called when event "CreatedEntity" is received.
        /// </summary>
        public virtual void OnCreate() { }

        /// <summary>
        /// Called when event "DestroyedEntity" is received.
        /// </summary>
        public virtual void OnDestroy() { }

        /// <summary>
        /// Called when event "Tick" is received.
        /// </summary>
        public virtual void OnTick() { }
    }
}
