// Date Created: 2022/12/10
// Created by: JSW

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
    }
}