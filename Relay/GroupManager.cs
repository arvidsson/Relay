using System.Collections.Generic;
using System.Linq;

namespace Relay
{
    /// <summary>
    /// Maps unique groups to a collection of entities.
    /// </summary>
    public class GroupManager
    {
        Dictionary<string, List<Entity>> groupToEntities = new();

        /// <summary>
        /// Adds an entity to a group.
        /// </summary>
        /// <param name="group">The group to add the entity to.</param>
        /// <param name="entity">The entity to add to the group.</param>
        public void AddEntityToGroup(string group, Entity entity)
        {
            if (!GroupExists(group))
            {
                groupToEntities[group] = new();
            }

            if (!IsEntityInGroup(group, entity))
            {
                groupToEntities[group].Add(entity);
            }
        }

        /// <summary>
        /// Checks if an entity is in a group.
        /// </summary>
        /// <param name="group">The group to check.</param>
        /// <param name="entity">The entity to check.</param>
        /// <returns>True if the entity is in the group, false otherwise.</returns>
        public bool IsEntityInGroup(string group, Entity entity)
        {
            if (!GroupExists(group)) return false;

            return groupToEntities[group].Contains(entity);
        }

        /// <summary>
        /// Removes an entity from a group.
        /// </summary>
        /// <param name="group">The group to remove the entity from.</param>
        /// <param name="entity">The entity to remove from the group.</param>
        public void RemoveEntityFromGroup(string group, Entity entity)
        {
            if (!GroupExists(group))
                return;

            if (!IsEntityInGroup(group, entity))
                return;

            groupToEntities[group].Remove(entity);
        }

        /// <summary>
        /// Removes an entity from all groups.
        /// </summary>
        /// <param name="entity">The entity to remove from all groups.</param>
        public void RemoveEntityFromAllGroups(Entity entity)
        {
            var groupsToRemoveFrom = groupToEntities
                .Where(kvp => kvp.Value.Contains(entity))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var group in groupsToRemoveFrom)
            {
                RemoveEntityFromGroup(group, entity);
            }
        }

        /// <summary>
        /// Gets all entities in a group.
        /// </summary>
        /// <param name="group">The group to get the entities from.</param>
        /// <returns>A list of entities in the group.</returns>
        public List<Entity> GetEntities(string group)
        {
            return groupToEntities[group];
        }

        bool GroupExists(string group)
        {
            return groupToEntities.ContainsKey(group);
        }
    }
}