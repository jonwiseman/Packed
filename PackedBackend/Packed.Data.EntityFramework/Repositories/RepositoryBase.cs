// Date Created: 2022/12/10
// Created by: JSW

using Packed.Data.Core.Repositories;

namespace Packed.Data.EntityFramework.Repositories;

/// <summary>
/// Base repository implementing the <see cref="IRepositoryBase{T}"/> interface
/// </summary>
public abstract class RepositoryBase<T> : IRepositoryBase<T>
    where T : class
{
    #region FIELDS

    /// <summary>
    /// DB context representing state of database
    /// </summary>
    protected readonly PackedDbContext Context;

    #endregion FIELDS

    #region CONSTRUCTOR

    /// <summary>
    /// Create a new base repository. Inject context
    /// </summary>
    /// <param name="context">Context</param>
    protected RepositoryBase(PackedDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    #endregion CONSTRUCTOR

    #region METHODS

    /// <summary>
    /// Create a new entity
    /// </summary>
    /// <param name="entity">Entity to create</param>
    public void Create(T entity) => Context.Set<T>().Add(entity);

    /// <summary>
    /// Update an existing entity
    /// </summary>
    /// <param name="entity">Updated entity</param>
    public void Update(T entity) => Context.Set<T>().Update(entity);


    /// <summary>
    /// Delete an existing entity
    /// </summary>
    /// <param name="entity">Entity to delete</param>
    public void Delete(T entity) => Context.Set<T>().Remove(entity);


    /// <summary>
    /// Save all changes
    /// </summary>
    /// <returns>
    /// The number of entities affected
    /// </returns>
    public int SaveChanges()
    {
        return Context.SaveChanges();
    }

    /// <summary>
    /// Save all changes (async overload)
    /// </summary>
    /// <returns>
    /// The number of entities affected
    /// </returns>
    public Task<int> SaveChangesAsync()
    {
        return Context.SaveChangesAsync();
    }

    #endregion METHODS
}