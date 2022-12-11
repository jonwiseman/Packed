// Date Created: 2022/12/11
// Created by: JSW

using System.Threading.Tasks;
using Packed.Data.Core.Entities;

namespace Packed.Data.Core.Repositories
{
    /// <summary>
    /// Interface defining a unit of work which holds repositories for all relevant entities
    /// in the Packed back-end
    /// </summary>
    public interface IPackedUnitOfWork
    {
        /// <summary>
        /// Repository for interacting with lists
        /// </summary>
        IListRepository ListRepository { get; }

        /// <summary>
        /// Repository for interacting with items
        /// </summary>
        IRepositoryBase<Item> ItemRepository { get; }

        /// <summary>
        /// Repository for interacting with containers
        /// </summary>
        IRepositoryBase<Container> ContainerRepository { get; }

        /// <summary>
        /// Repository for interacting with placements
        /// </summary>
        IRepositoryBase<Placement> PlacementRepository { get; }

        /// <summary>
        /// Save all changes to database context made via repositories
        /// </summary>
        /// <returns>
        /// Number of entities affected
        /// </returns>
        Task<int> SaveChangesAsync();
    }
}