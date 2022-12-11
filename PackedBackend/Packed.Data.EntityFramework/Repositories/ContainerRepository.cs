// Date Created: 2022/12/11
// Created by: JSW

using Packed.Data.Core.Entities;

namespace Packed.Data.EntityFramework.Repositories;

/// <summary>
/// Repository for interacting with containers
/// </summary>
public class ContainerRepository : RepositoryBase<Container>
{
    /// <summary>
    /// Create a new repository
    /// </summary>
    /// <param name="context">DB context</param>
    public ContainerRepository(PackedDbContext context)
        : base(context)
    {
    }
}