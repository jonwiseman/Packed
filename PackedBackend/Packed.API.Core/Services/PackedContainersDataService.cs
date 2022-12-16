// Date Created: 2022/12/13
// Created by: JSW

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using Packed.API.Core.DTOs;
using Packed.API.Core.Exceptions;
using Packed.Data.Core.Entities;
using Packed.Data.Core.Repositories;

namespace Packed.API.Core.Services
{
    /// <summary>
    /// Data service for manipulating containers
    /// </summary>
    public class PackedContainersDataService : PackedDataServiceBase, IPackedContainersDataService
    {
        #region CONSTRUCTOR

        /// <summary>
        /// Create a new data service
        /// </summary>
        /// <param name="packedUnitOfWork">Unit of work for interacting with Packed data store</param>
        public PackedContainersDataService(IPackedUnitOfWork packedUnitOfWork)
            : base(packedUnitOfWork)
        {
        }

        #endregion CONSTRUCTOR

        #region METHODS

        /// <summary>
        /// Retrieve all containers in the specified list
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <returns>
        /// All containers
        /// </returns>
        /// <exception cref="ListNotFoundException">List could not be found</exception>
        public async Task<List<ContainerDto>> GetContainersAsync(int listId)
        {
            // Get list
            var foundList = await GetList(listId);

            return foundList.Containers
                       ?.Select(c => new ContainerDto(c))
                       .ToList()
                   ?? new List<ContainerDto>();
        }

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
        public async Task<ContainerDto> AddContainerAsync(int listId, ContainerDto newContainer)
        {
            // Get list. Although we won't use the return value, it ensures that the list actually exists
            await GetList(listId);

            // If the list was found, then we will attempt to add the new container
            // Start by creating the entity we are going to add
            var containerToAdd = new Container
            {
                ListId = listId,
                Name = newContainer.Name,
                Placements = new List<Placement>()
            };

            try
            {
                // Then we'll add it to the list of items tracked by the context
                PackedUnitOfWork.ContainerRepository.Create(containerToAdd);

                // Finally, we'll attempt to persist the container
                await PackedUnitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // If the exception is that we have a unique violation, then we throw a DuplicateListException
                if (e.InnerException is PostgresException p && p.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    throw new DuplicateContainerException("A container with the same name already exists", e);
                }

                // Otherwise something else happen and we rethrow the exception
                throw;
            }

            // On success, we'll return a DTO representation of the container
            return new ContainerDto(containerToAdd);
        }

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
        public async Task<ContainerDto> GetContainerByIdAsync(int listId, int containerId)
        {
            // Get list
            var foundList = await GetList(listId);

            // If we found the list, then we'll attempt to locate the exact container
            var foundContainer = GetContainer(foundList, containerId);

            // Finally, return a DTO representation of the container
            return new ContainerDto(foundContainer);
        }

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
        public async Task<ContainerDto> UpdateContainerAsync(int listId, int containerId, ContainerDto updatedContainer)
        {
            // Get list
            var foundList = await GetList(listId);

            // If we found the list, then we'll attempt to locate the exact container
            var foundContainer = GetContainer(foundList, containerId);

            // Bit of short-circuit logic here: if we're not actually updating the container name then we don't have to do anything
            if (string.Equals(updatedContainer.Name, foundContainer.Name))
            {
                return new ContainerDto(foundContainer);
            }

            // Now that we've found the container, we can make the actual update
            foundContainer.Name = updatedContainer.Name;

            try
            {
                // Update the entity
                PackedUnitOfWork.ContainerRepository.Update(foundContainer);

                // Save changes to data store
                await PackedUnitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // If the exception is that we have a unique violation, then we throw a DuplicateListException
                if (e.InnerException is PostgresException p && p.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    throw new DuplicateContainerException("A container with the same name already exists", e);
                }

                // Otherwise something else happen and we rethrow the exception
                throw;
            }

            return new ContainerDto(foundContainer);
        }

        /// <summary>
        /// Remove a container
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <param name="containerId">Container ID</param>
        /// <exception cref="ListNotFoundException">List could not be found</exception>
        /// <exception cref="ContainerNotFoundException">Container could not be found</exception>
        public async Task DeleteContainerAsync(int listId, int containerId)
        {
            // Get list
            var foundList = await GetList(listId);

            // If we found the list, then we'll attempt to locate the exact container
            var foundContainer = GetContainer(foundList, containerId);

            // Finally, delete the container
            PackedUnitOfWork.ContainerRepository.Delete(foundContainer);
            await PackedUnitOfWork.SaveChangesAsync();
        }

        #endregion METHODS
    }
}