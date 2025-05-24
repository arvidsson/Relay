using System.Collections.Generic;

namespace Relay
{
    /// <summary>
    /// The world is where all the entities live.
    /// It orchestrates the creation and destruction of entities, and the firing and handling of events.
    /// </summary>
    public class World
    {
        EntityManager entities = new();
        EventManager events = new();
        TagManager tags = new();
        GroupManager groups = new();

        /// <summary>
        /// Updates the world:
        /// - processing created and destroyed entites
        /// - firing a global "Tick" event
        /// - handling all the events currently in the event queue
        /// </summary>
        public void Update()
        {
            entities.ProcessCreatedEntities(events);
            entities.ProcessDestroyedEntities(events, tags, groups);
            events.FireEvent(new Event(type: "Tick"));
            events.HandleEvents(entities);
        }

        #region Events

        /// <summary>
        /// Fires an event to all entities.
        /// </summary>
        /// <param name="ev">The event</param>
        public void FireEvent(Event ev)
        {
            events.FireEvent(ev);
        }

        /// <summary>
        /// Fires a simple event to all entities.
        /// </summary>
        /// <param name="ev">The event</param>
        public void FireEvent(string type)
        {
            FireEvent(new Event(type: type));
        }

        #endregion

        #region Entities

        /// <summary>
        /// Creates a new entity.
        /// </summary>
        public Entity CreateEntity()
        {
            Entity entity = entities.CreateEntity(this);
            return entity;
        }

        /// <summary>
        /// Destroys an entity.
        /// </summary>
        public void DestroyEntity(Entity entity)
        {
            events.FireEvent(new Event(type: "DestroyedEntity", target: entity));
            entities.DestroyEntity(entity);
        }

        /// <summary>
        /// Returns all entities which are currently alive.
        /// </summary>
        public List<Entity> GetEntities()
        {
            return entities.GetEntities();
        }

        #endregion

        #region Tags

        /// <summary>
        /// Adds a tag to an entity.
        /// </summary>
        public void AddTag(string tag, Entity entity)
        {
            tags.AddTagToEntity(tag, entity);
        }

        /// <summary>
        /// Checks if an entity has a tag.
        /// </summary>
        public bool HasTag(string tag, Entity entity)
        {
            return tags.EntityHasTag(tag, entity);
        }

        /// <summary>
        /// Removes a tag from an entity.
        /// </summary>
        public void RemoveTag(string tag, Entity entity)
        {
            tags.RemoveTagFromEntity(tag, entity);
        }

        /// <summary>
        /// Gets an entity by tag.
        /// </summary>
        public Entity GetEntity(string tag)
        {
            return tags.GetEntity(tag);
        }

        #endregion

        #region Groups

        /// <summary>
        /// Adds an entity to a group.
        /// </summary>
        public void AddEntityToGroup(string group, Entity entity)
        {
            groups.AddEntityToGroup(group, entity);
        }

        /// <summary>
        /// Checks if an entity is in a group.
        /// </summary>
        public bool IsEntityInGroup(string group, Entity entity)
        {
            return groups.IsEntityInGroup(group, entity);
        }

        /// <summary>
        /// Removes an entity from a group.
        /// </summary>
        public void RemoveEntityFromGroup(string group, Entity entity)
        {
            groups.RemoveEntityFromGroup(group, entity);
        }

        /// <summary>
        /// Gets entities by group.
        /// </summary>
        public List<Entity> GetEntities(string group)
        {
            return groups.GetEntities(group);
        }

        #endregion
    }
}
