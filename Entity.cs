using Priority_Queue;
using System;
using System.Linq;

namespace Relay
{
    /// <summary>
    /// An entity is a collection of components.
    /// </summary>
    public class Entity : IEquatable<Entity>
    {
        /// <summary>
        /// The unique identifier of the entity.
        /// </summary>
        public uint Id { get; private set; }

        /// <summary>
        /// The world that the entity belongs to.
        /// </summary>
        public World World { get; private set; }

        // TODO: make into component instead
        public Coord Position { get; set; }

        private static uint nextId = 0;

        private SimplePriorityQueue<IComponent, int> components = new SimplePriorityQueue<IComponent, int>();

        public Entity(World world)
        {
            Id = nextId++;
            World = world;
        }

        public bool Equals(Entity e)
        {
            return Id == e.Id;
        }

        public override bool Equals(object obj)
        {
            if (obj is Entity)
                return Equals((Entity)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Destroys the entity.
        /// </summary>
        public void Destroy()
        {
            World.DestroyEntity(this);
        }

        #region Components

        /// <summary>
        /// Adds a component to the entity.
        /// </summary>
        /// <param name="component">The component to add.</param>
        public void AddComponent(IComponent component)
        {
            if (HasComponent(component))
                return;

            components.Enqueue(component, component.Priority);
            component.Init(this);
        }

        /// <summary>
        /// Adds multiple components to the entity.
        /// </summary>
        /// <param name="components">The components to add.</param>
        public void AddComponents(params IComponent[] components)
        {
            foreach (IComponent component in components)
            {
                AddComponent(component);
            }
        }

        /// <summary>
        /// Removes a component from the entity.
        /// </summary>
        /// <param name="component">The component to remove.</param>
        public void RemoveComponent(IComponent component)
        {
            if (!HasComponents())
                return;

            if (!HasComponent(component))
                return;

            components.Remove(component);
        }

        /// <summary>
        /// Removes multiple components from the entity.
        /// </summary>
        /// <param name="components">The components to remove.</param>
        public void RemoveComponents(params IComponent[] components)
        {
            foreach (IComponent component in components)
            {
                RemoveComponent(component);
            }
        }

        /// <summary>
        /// Checks if the entity has a component.
        /// </summary>
        /// <param name="component">The component to check.</param>
        /// <returns>True if the entity has the component, false otherwise.</returns> 
        public bool HasComponent(IComponent component)
        {
            return components.Contains(component);
        }

        /// <summary>
        /// Checks if the entity has a component with the given id.
        /// </summary>
        /// <param name="id">The id of the component to check.</param>
        /// <returns>True if the entity has the component, false otherwise.</returns>
        public bool HasComponent(string id)
        {
            return components.Any(c => c.Id == id);
        }

        /// <summary>
        /// Checks if the entity has any components.
        /// </summary>
        /// <returns>True if the entity has any components, false otherwise.</returns>
        public bool HasComponents()
        {
            return components.Count != 0;
        }

        /// <summary>
        /// Gets a component from the entity.
        /// </summary>
        /// <param name="id">The id of the component to get.</param>
        /// <returns>The component with the given id, or null if it doesn't exist.</returns>
        public IComponent GetComponent(string id)
        {
            return components.FirstOrDefault(c => c.Id == id);
        }

        #endregion

        #region Events

        /// <summary>
        /// Fires an event to the entity.
        /// </summary>
        /// <param name="ev">The event to fire.</param>
        public void FireEvent(Event ev)
        {
            World.FireEvent(ev);
        }

        /// <summary>
        /// Fires a simple event to the entity.
        /// </summary>
        /// <param name="ev">The event to fire.</param>
        public void FireEvent(string ev)
        {
            World.FireEvent(new Event(ev, this));
        }

        /// <summary>
        /// Fires an event to the entity with data.
        /// </summary>
        /// <param name="ev">The event to fire.</param>
        /// <param name="data">The data to fire the event with.</param> 
        public void FireEvent(string ev, EventData data)
        {
            World.FireEvent(new Event(ev, data, this));
        }

        /// <summary>
        /// Fires an event to all entities.
        /// </summary>
        /// <param name="ev">The event to fire.</param>
        public void FireEventToAll(string ev)
        {
            World.FireEvent(new Event(ev));
        }

        /// <summary>
        /// Fires an event to all entities with data.
        /// </summary>
        /// <param name="ev">The event to fire.</param>
        /// <param name="data">The data to fire the event with.</param>
        public void FireEventToAll(string ev, EventData data)
        {
            World.FireEvent(new Event(ev, data));
        }

        /// <summary>
        /// Handles an event which will be consumed by the components inside the entity.
        /// </summary>
        /// <param name="ev">The event to handle.</param>
        /// <returns>True if the event was handled, false otherwise.</returns>
        public bool HandleEvent(Event ev)
        {
            // is the event for a specific entity?
            if (ev.HasTarget)
            {
                if (ev.Target != this)
                    return false;
            }

            foreach (var component in components)
            {
                if (component.IsListening(ev.Type))
                {
                    bool continueHandleEvent = component.HandleEvent(ev);

                    if (!continueHandleEvent)
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Adds a tag to the entity.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        public void AddTag(string tag)
        {
            World.AddTag(tag, this);
        }

        /// <summary>
        /// Removes a tag from the entity.
        /// </summary>
        /// <param name="tag">The tag to remove.</param>
        public void RemoveTag(string tag)
        {
            World.RemoveTag(tag, this);
        }

        /// <summary>
        /// Adds the entity to a group.
        /// </summary>
        /// <param name="group">The group to add the entity to.</param>
        public void AddToGroup(string group)
        {
            World.AddEntityToGroup(group, this);
        }

        /// <summary>
        /// Removes the entity from a group.
        /// </summary>
        /// <param name="group">The group to remove the entity from.</param>
        public void RemoveFromGroup(string group)
        {
            World.RemoveEntityFromGroup(group, this);
        }

        /// <summary>
        /// Checks if the entity is in a group.
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <returns>True if the entity is in the group, false otherwise.</returns>
        public bool IsInGroup(string group)
        {
            return World.IsEntityInGroup(group, this);
        }

        #endregion
    }
}
