// Date Created: 2022/12/10
// Created by: JSW

using System.Threading.Tasks;

namespace Packed.Data.Core.Repositories
{
    /// <summary>
    /// Repository representing operations common to all repositories
    /// </summary>
    public interface IRepositoryBase<T>
        where T : class
    {
        /// <summary>
        /// Create a new entity
        /// </summary>
        /// <param name="entity">Entity to create</param>
        void Create(T entity);

        /// <summary>
        /// Update an existing entity
        /// </summary>
        /// <param name="entity">Updated entity</param>
        void Update(T entity);

        /// <summary>
        /// Delete an existing entity
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        void Delete(T entity);

        /// <summary>
        /// Save all changes
        /// </summary>
        /// <returns>
        /// The number of entities affected
        /// </returns>
        int SaveChanges();

        /// <summary>
        /// Save all changes (async overload)
        /// </summary>
        /// <returns>
        /// The number of entities affected
        /// </returns>
        Task<int> SaveChangesAsync();
    }
}