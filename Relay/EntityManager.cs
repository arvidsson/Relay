using System.Collections.Generic;
using System.Linq;

namespace Relay
{
    /// <summary>
    /// Manages the creation and destruction of entities.
    /// </summary>
    public class EntityManager
    {
        List<Entity> entities = new();
        List<Entity> createdEntities = new();
        List<Entity> destroyedEntities = new();

        /// <summary>
        /// Get all alive entities.
        /// </summary>
        /// <returns>All alive entities.</returns>
        public List<Entity> GetEntities()
        {
            return entities;
        }

        /// <summary>
        /// Create a new entity.
        /// </summary>
        /// <param name="world">The world to create the entity in.</param>
        /// <returns>The created entity.</returns>
        public Entity CreateEntity(World world)
        {
            Entity entity = new Entity(world);
            createdEntities.Add(entity);
            return entity;
        }

        /// <summary>
        /// Destroy an entity.
        /// </summary>
        /// <param name="entity">The entity to destroy.</param>
        public void DestroyEntity(Entity entity)
        {
            destroyedEntities.Add(entity);
        }

        /// <summary>
        /// Process created entities (actually adding them to the active list of entities).
        /// </summary>
        /// <param name="events">The event manager.</param>
        public void ProcessCreatedEntities(EventManager events)
        {
            foreach (Entity createdEntity in createdEntities)
            {
                entities.Add(createdEntity);
                events.FireEvent(new Event(type: "CreatedEntity", target: createdEntity));
            }

            createdEntities.Clear();
        }

        /// <summary>
        /// Process destroyed entities (actually removing them from the active list of entities, and removing them from tags and groups).
        /// </summary>
        /// <param name="events">The event manager.</param>
        /// <param name="tags">The tag manager.</param>
        /// <param name="groups">The group manager.</param>
        public void ProcessDestroyedEntities(EventManager events, TagManager tags, GroupManager groups)
        {
            foreach (Entity destroyedEntity in destroyedEntities)
            {
                entities.Remove(destroyedEntity);
                tags.RemoveTagsFromEntity(destroyedEntity);
                groups.RemoveEntityFromGroups(destroyedEntity);
            }

            destroyedEntities.Clear();
        }
    }
}
