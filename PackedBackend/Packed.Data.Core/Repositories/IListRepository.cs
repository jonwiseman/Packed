// Date Created: 2022/12/10
// Created by: JSW

using System.Collections.Generic;
using System.Threading.Tasks;
using Packed.Data.Core.Entities;

namespace Packed.Data.Core.Repositories
{
    /// <summary>
    /// Interface representing a repository used to retrieve <see cref="List"/>
    /// entities from the database
    /// </summary>
    public interface IListRepository : IRepositoryBase<List>
    {
        /// <summary>
        /// Retrieve all lists
        /// </summary>
        /// <returns>
        /// All lists
        /// </returns>
        Task<List<List>> GetAllListsAsync();
    }
}