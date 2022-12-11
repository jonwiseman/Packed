// Date Created: 2022/12/11
// Created by: JSW

using Packed.Data.Core.Entities;
using Packed.Data.Core.Repositories;

namespace Packed.Data.EntityFramework.Repositories;

/// <summary>
/// Unit of work interface for Packed back-end
/// </summary>
public class PackedUnitOfWork : IPackedUnitOfWork
{
    #region FIELDS

    /// <summary>
    /// Packed database context
    /// </summary>
    private readonly PackedDbContext _dbContext;

    #endregion FIELDS

    #region CONSTRUCTOR

    public PackedUnitOfWork(PackedDbContext dbContext)
    {
        // Set DB context
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        // Create repositories
        ListRepository = new ListRepository(_dbContext);
        ItemRepository = new ItemRepository(_dbContext);
        ContainerRepository = new ContainerRepository(_dbContext);
        PlacementRepository = new PlacementRepository(_dbContext);
    }

    #endregion CONSTRUCTOR

    #region PROPERTIES

    /// <summary>
    /// Repository for interacting with lists
    /// </summary>
    public IListRepository ListRepository { get; }

    /// <summary>
    /// Repository for interacting with items
    /// </summary>
    public IRepositoryBase<Item> ItemRepository { get; }

    /// <summary>
    /// Repository for interacting with containers
    /// </summary>
    public IRepositoryBase<Container> ContainerRepository { get; }

    /// <summary>
    /// Repository for interacting with placements
    /// </summary>
    public IRepositoryBase<Placement> PlacementRepository { get; }

    /// <summary>
    /// Save all changes to database context made via repositories
    /// </summary>
    /// <returns>
    /// Number of entities affected
    /// </returns>
    public Task<int> SaveChangesAsync()
    {
        return _dbContext.SaveChangesAsync();
    }

    #endregion PROPERTIES

    #region METHODS

    #endregion METHODS
}