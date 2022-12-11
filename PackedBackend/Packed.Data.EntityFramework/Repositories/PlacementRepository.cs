// Date Created: 2022/12/11
// Created by: JSW

using Packed.Data.Core.Entities;

namespace Packed.Data.EntityFramework.Repositories;

/// <summary>
/// Repository for interacting with placements
/// </summary>
public class PlacementRepository : RepositoryBase<Placement>
{
    /// <summary>
    /// Create a new repository
    /// </summary>
    /// <param name="context">DB context</param>
    public PlacementRepository(PackedDbContext context)
        : base(context)
    {
    }
}