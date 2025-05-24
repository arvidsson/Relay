using System.Collections.Generic;
using System.Linq;

namespace Relay
{
    /// <summary>
    /// Manages the firing and handling of events.
    /// </summary>
    public class EventManager
    {
        List<Event> firedEvents = new();

        /// <summary>
        /// Fire an event.
        /// </summary>
        /// <param name="ev">The event to fire.</param>
        public void FireEvent(Event ev)
        {
            firedEvents.Add(ev);
        }

        /// <summary>
        /// Handle events.
        /// </summary>
        /// <param name="entities">The entity manager.</param>
        public void HandleEvents(EntityManager entities)
        {
            // we need to process a copy of firedEvents, since new events could be fired while we are actually handling them
            List<Event> handleEvents = firedEvents.ToList();
            firedEvents.Clear();

            foreach (Event ev in handleEvents)
            {
                foreach (Entity entity in entities.GetEntities())
                {
                    entity.HandleEvent(ev);
                }
            }
        }
    }
}
