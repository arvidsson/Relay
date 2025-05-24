using System.Collections.Generic;
using System.Linq;

namespace Relay
{
    /// <summary>
    /// Maps unique tags to unique entities.
    /// </summary>
    public class TagManager
    {
        Dictionary<string, Entity> tagToEntity = new();

        /// <summary>
        /// Adds a tag to an entity.
        /// </summary>
        /// <param name="tag">The tag to add.</param>
        /// <param name="entity">The entity to add the tag to.</param>
        public void AddTagToEntity(string tag, Entity entity)
        {
            if (TagExists(tag)) return;

            if (EntityHasTag(tag, entity))
                return;

            tagToEntity[tag] = entity;
        }

        /// <summary>
        /// Checks if an entity has a tag.
        /// </summary>
        /// <param name="tag">The tag to check.</param>
        /// <param name="entity">The entity to check.</param>
        /// <returns>True if the entity has the tag, false otherwise.</returns>
        public bool EntityHasTag(string tag, Entity entity)
        {
            return TagExists(tag) && tagToEntity[tag] == entity;
        }

        /// <summary>
        /// Removes a tag from an entity.
        /// </summary>
        /// <param name="tag">The tag to remove.</param>
        /// <param name="entity">The entity to remove the tag from.</param>
        public void RemoveTagFromEntity(string tag, Entity entity)
        {
            if (!EntityHasTag(tag, entity))
                return;

            tagToEntity.Remove(tag);
        }
        
        /// <summary>
        /// Removes all tags from an entity.
        /// </summary>
        /// <param name="entity">The entity to remove from all tags.</param>
        public void RemoveTagsFromEntity(Entity entity)
        {
            var tagsToRemove = tagToEntity
                .Where(kvp => kvp.Value == entity)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var tag in tagsToRemove)
            {
                tagToEntity.Remove(tag);
            }
        }

        /// <summary>
        /// Gets an entity by tag.
        /// </summary>
        /// <param name="tag">The tag to get the entity from.</param>
        /// <returns>The entity with the tag.</returns>
        public Entity GetEntity(string tag)
        {
            return tagToEntity[tag];
        }

        bool TagExists(string tag)
        {
            return tagToEntity.ContainsKey(tag);
        }
    }
}