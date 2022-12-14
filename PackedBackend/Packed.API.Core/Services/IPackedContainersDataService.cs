// Date Created: 2022/12/13
// Created by: JSW

using System.Collections.Generic;
using System.Threading.Tasks;
using Packed.API.Core.DTOs;
using Packed.API.Core.Exceptions;

namespace Packed.API.Core.Services
{
    /// <summary>
    /// Interface defining a service used to manipulate containers
    /// </summary>
    public interface IPackedContainersDataService
    {
        /// <summary>
        /// Retrieve all containers in the specified list
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <returns>
        /// All containers
        /// </returns>
        /// <exception cref="ListNotFoundException">List could not be found</exception>
        Task<List<ContainerDto>> GetContainersAsync(int listId);

        /// <summary>
        /// Add a new container to the specified list
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="newContainer">New container</param>
        /// <returns>
        /// A representation of the new container
        /// </returns>
        /// <exception cref="ListNotFoundException">List could not be found</exception>
        /// <exception cref="DuplicateContainerException">Container with same name already exists in the list</exception>
        Task<ContainerDto> AddContainerAsync(int listId, ContainerDto newContainer);

        /// <summary>
        /// Retrieve a specific container by ID
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="containerId">Container ID</param>
        /// <returns>
        /// The specified container
        /// </returns>
        /// <exception cref="ListNotFoundException">List could not be found</exception>
        /// <exception cref="ContainerNotFoundException">Container could not be found</exception>
        Task<ContainerDto> GetContainerByIdAsync(int listId, int containerId);

        /// <summary>
        /// Update an existing container
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="containerId">Container ID</param>
        /// <param name="updatedContainer">The updated container</param>
        /// <returns>
        /// A representation of the updated container
        /// </returns>
        /// <exception cref="ListNotFoundException">List could not be found</exception>
        /// <exception cref="ContainerNotFoundException">Container could not be found</exception>
        /// <exception cref="DuplicateContainerException">Container with same name already exists in the list</exception>
        Task<ContainerDto> UpdateContainerAsync(int listId, int containerId, ContainerDto updatedContainer);

        /// <summary>
        /// Remove a container
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="containerId">Container ID</param>
        /// <exception cref="ListNotFoundException">List could not be found</exception>
        /// <exception cref="ContainerNotFoundException">Container could not be found</exception>
        Task DeleteContainerAsync(int listId, int containerId);
    }
}