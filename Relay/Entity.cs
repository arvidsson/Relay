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

        static uint nextId = 0;

        Dictionary<Type, Component> components = new();
        Dictionary<string, SimplePriorityQueue<Behaviour, int>> behaviours = new();

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
        /// <typeparam name="T">The type of component to add.</typeparam>
        /// <param name="component">The component to add.</param>
        public void AddComponent<T>(T component) where T : Component
        {
            components[component.GetType()] = component;
        }

        /// <summary>
        /// Adds multiple components to the entity.
        /// </summary>
        /// <param name="components">The components to add.</param>
        public void AddComponents(params Component[] components)
        {
            foreach (var component in components)
            {
                AddComponent(component);
            }
        }

        /// <summary>
        /// Removes a component from the entity.
        /// </summary>
        /// <typeparam name="T">The type of component to remove.</typeparam>
        public void RemoveComponent<T>() where T : Component
        {
            components.Remove(typeof(T));
        }

        /// <summary>
        /// Checks if the entity has a component of type T.
        /// </summary>
        /// <typeparam name="T">The type of component to check.</typeparam>
        /// <returns>True if the entity has the component, false otherwise.</returns>
        public bool HasComponent<T>() where T : Component
        {
            return components.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Gets a component of type T from the entity.
        /// </summary>
        /// <typeparam name="T">The type of component to get.</typeparam>
        /// <returns>The component of type T, or null if it doesn't exist.</returns>
        public T GetComponent<T>() where T : Component
        {
            components.TryGetValue(typeof(T), out var component);
            return component as T;
        }

        #endregion

        #region Behaviours

        public void AddBehaviour(Behaviour behaviour)
        {
            if (HasBehaviour(behaviour))
                return;

            if (!behaviours.ContainsKey(behaviour.Category))
                behaviours[behaviour.Category] = new SimplePriorityQueue<Behaviour, int>();

            behaviours[behaviour.Category].Enqueue(behaviour, behaviour.Priority);
            behaviour.Owner = this;
        }

        /// <summary>
        /// Adds multiple behaviours to the entity.
        /// </summary>
        /// <param name="behaviours">The behaviours to add.</param>
        public void AddBehaviours(params Behaviour[] behaviours)
        {
            foreach (Behaviour behaviour in behaviours)
            {
                AddBehaviour(behaviour);
            }
        }

        /// <summary>
        /// Removes a behaviour from the entity.
        /// </summary>
        /// <param name="behaviour">The behaviour to remove.</param>
        public void RemoveBehaviour(Behaviour behaviour)
        {
            if (!HasBehaviour(behaviour))
                return;

            behaviours[behaviour.Category].Remove(behaviour);
        }

        /// <summary>
        /// Removes multiple behaviours from the entity.
        /// </summary>
        /// <param name="behaviours">The behaviours to remove.</param>
        public void RemoveBehaviours(params Behaviour[] behaviours)
        {
            foreach (Behaviour behaviour in behaviours)
            {
                RemoveBehaviour(behaviour);
            }
        }

        /// <summary>
        /// Checks if the entity has a behaviour.
        /// </summary>
        /// <param name="behaviour">The behaviour to check.</param>
        /// <returns>True if the entity has the behaviour, false otherwise.</returns> 
        public bool HasBehaviour(Behaviour behaviour)
        {
            return behaviours.ContainsKey(behaviour.Category) && behaviours[behaviour.Category].Contains(behaviour);
        }

        /// <summary>
        /// Gets a behaviour of type T from the entity.
        /// </summary>
        /// <typeparam name="T">The type of behaviour to get.</typeparam>
        /// <returns>The behaviour of type T, or null if it doesn't exist.</returns>
        public T GetBehaviour<T>() where T : Behaviour
        {
            foreach (var category in behaviours.Values)
            {
                foreach (var behaviour in category)
                {
                    if (behaviour is T typedBehaviour)
                    {
                        return typedBehaviour;
                    }
                }
            }
            return null;
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
        public void HandleEvent(Event ev)
        {
            // is the event for a specific entity?
            if (ev.HasTarget)
            {
                if (ev.Target != this)
                    return;
            }

            var behavioursByCategory = behaviours.OrderBy(kvp => World.BehaviourCategoryOrder.IndexOf(kvp.Key));

            foreach (var group in behavioursByCategory)
            {
                foreach (var behaviour in group.Value)
                {
                    if (behaviour.IsListening(ev.Type))
                    {
                        var result = behaviour.HandleEvent(ev);
                        if (result == BehaviourResult.Stop)
                            return;
                    }
                }
            }
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
