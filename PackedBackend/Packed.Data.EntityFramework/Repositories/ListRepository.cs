// Date Created: 2022/12/10
// Created by: JSW

using Microsoft.EntityFrameworkCore;
using Packed.Data.Core.Entities;
using Packed.Data.Core.Repositories;

namespace Packed.Data.EntityFramework.Repositories;

/// <summary>
/// Repository for retrieving <see cref="List"/> entities from
/// the database
/// </summary>
public class ListRepository : RepositoryBase<List>, IListRepository
{
    #region CONSTRUCTOR

    public ListRepository(PackedDbContext context)
        : base(context)
    {
    }

    #endregion CONSTRUCTOR

    #region METHODS

    /// <summary>
    /// Retrieve all lists
    /// </summary>
    /// <returns>
    /// All lists
    /// </returns>
    public Task<List<List>> GetAllListsAsync()
    {
        return Context
            .Lists
            .Include(l => l.Items)
            .ThenInclude(i => i.Placements)
            .Include(l => l.Containers)
            .ToListAsync();
    }

    #endregion METHODS
}