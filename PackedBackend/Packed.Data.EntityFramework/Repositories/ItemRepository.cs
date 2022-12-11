// Date Created: 2022/12/11
// Created by: JSW

using Packed.Data.Core.Entities;

namespace Packed.Data.EntityFramework.Repositories;

/// <summary>
/// Repository for interacting with items
/// </summary>
public class ItemRepository : RepositoryBase<Item>
{
    /// <summary>
    /// Create a new item repository
    /// </summary>
    /// <param name="context">DB Context</param>
    public ItemRepository(PackedDbContext context)
        : base(context)
    {
    }
}