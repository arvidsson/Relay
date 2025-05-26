namespace Relay
{
    /// <summary>
    /// The result of a behaviour handling an event.
    /// </summary>
    public enum BehaviourResult
    {
        /// <summary>
        /// Continue propagating the event.
        /// </summary>
        Continue,

        /// <summary>
        /// Stop propagating the event.
        /// </summary>
        Stop,
    }

    /// <summary>
    /// A behaviour listens to events and handles them by doing logic and manipulating the event data.
    /// </summary>
    public abstract class Behaviour
    {
        /// <summary>
        /// Which entity owns the behaviour.
        /// </summary>
        public Entity Owner { get; internal set; }

        /// <summary>
        /// The category of the behaviour - used for grouping behaviours.
        /// </summary>
        public string Category { get; protected set; } = "Default";

        /// <summary>
        /// The priority of the behaviour - used for deciding in which order the behaviours should handle events within the same category.
        /// </summary>
        public int Priority { get; protected set; } = 0;

        /// <summary>
        /// The event types the behaviour is listening to.
        /// </summary>
        public HashSet<string> ListensTo { get; } = new();

        /// <summary>
        /// Handles the event.
        /// </summary>
        /// <param name="ev">The event to handle.</param>
        /// <returns>The result of the behaviour.</returns>
        public virtual BehaviourResult HandleEvent(Event ev)
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

            return BehaviourResult.Continue;
        }

        protected Behavior()
        {
            ListensTo.Add("CreatedEntity");
            ListensTo.Add("DestroyedEntity");
        }

        /// <summary>
        /// Adds an event type to the behaviour's listening list.
        /// </summary>
        /// <param name="eventTypes">The event types to listen to.</param>
        protected void ListenTo(params string[] eventTypes)
        {
            foreach (var eventType in eventTypes)
                ListensTo.Add(eventType);
        }

        /// <summary>
        /// Returns true if the behaviour is listening to an event type.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        public bool IsListening(string eventType)
        {
            return ListensTo.Contains(eventType);
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