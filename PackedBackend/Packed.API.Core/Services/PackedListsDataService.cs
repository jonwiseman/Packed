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
    /// Standard implementation of the <see cref="IPackedListsDataService"/> interface
    /// </summary>
    public class PackedListsDataService : IPackedListsDataService
    {
        #region FIELDS

        /// <summary>
        /// Unit of work for interacting with Packed backend
        /// </summary>
        private readonly IPackedUnitOfWork _packedUnitOfWork;

        #endregion FIELDS

        #region CONSTRUCTOR

        /// <summary>
        /// Create a new data service
        /// </summary>
        /// <param name="packedUnitOfWork">Packed unit of work</param>
        public PackedListsDataService(IPackedUnitOfWork packedUnitOfWork)
        {
            _packedUnitOfWork = packedUnitOfWork ?? throw new ArgumentNullException(nameof(packedUnitOfWork));
        }

        #endregion CONSTRUCTOR

        #region METHODS

        /// <summary>
        /// Retrieve all lists which currently exist
        /// </summary>
        /// <returns>
        /// All lists which currently exist
        /// </returns>
        public async Task<IEnumerable<ListDto>> GetAllListsAsync()
        {
            // Find all lists which exist in the database
            var foundLists = (await _packedUnitOfWork.ListRepository.GetAllListsAsync())
                             ?? new List<List>();

            // Convert these list entities into DTO representations
            return foundLists
                .Select(l => new ListDto(l))
                .ToList();
        }

        /// <summary>
        /// Retrieve the list with the specified ID, if it exists
        /// </summary>
        /// <param name="listId">ID of the list to search for</param>
        /// <returns>
        /// The specified list, or null if it does not exist
        /// </returns>
        public async Task<ListDto> GetListByIdAsync(int listId)
        {
            return new ListDto(await GetList(listId));
        }

        /// <summary>
        /// Create a new list
        /// </summary>
        /// <param name="newList">New list</param>
        /// <returns>
        /// A representation of the new list
        /// </returns>
        /// <exception cref="DuplicateListException">A list with the given name already exists</exception>
        public async Task<ListDto> CreateNewListAsync(ListDto newList)
        {
            // Initialize entity to create
            var listToCreate = new List
            {
                Description = newList.Description,
                Items = new List<Item>(),
                Containers = new List<Container>()
            };

            try
            {
                // Create the entity in our context
                _packedUnitOfWork.ListRepository.Create(listToCreate);

                // Save all changes to database context
                await _packedUnitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // If the exception is that we have a unique violation, then we throw a DuplicateListException
                if (e.InnerException is PostgresException p && p.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    throw new DuplicateListException("A list with the same name already exists", e);
                }

                // Otherwise something else happen and we rethrow the exception
                throw;
            }

            // Return the new representation of the created list
            return new ListDto(listToCreate);
        }

        /// <summary>
        /// Update an existing list
        /// </summary>
        /// <param name="listId">ID of list to update</param>
        /// <param name="updatedList">Updated list</param>
        /// <returns>
        /// A representation of the updated list
        /// </returns>
        /// <exception cref="ListNotFoundException">The list could not be found</exception>
        /// <exception cref="DuplicateListException">List with given description already exists</exception>
        public async Task<ListDto> UpdateListAsync(int listId, ListDto updatedList)
        {
            // Get the list
            var foundList = await GetList(listId);

            // If there is no actual update to perform, then do nothing
            if (string.Equals(updatedList.Description, foundList.Description))
            {
                return new ListDto(foundList);
            }

            // If we have found the list, then go ahead and set the description...
            foundList.Description = updatedList.Description;

            try
            {
                // ...and attempt to update the list
                _packedUnitOfWork.ListRepository.Update(foundList);
                await _packedUnitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                // If the exception is that we have a unique violation, then we throw a DuplicateListException
                if (e.InnerException is PostgresException p && p.SqlState == PostgresErrorCodes.UniqueViolation)
                {
                    throw new DuplicateListException("A list with the same name already exists", e);
                }

                // Otherwise something else happen and we rethrow the exception
                throw;
            }

            return new ListDto(foundList);
        }

        /// <summary>
        /// Delete list with given ID
        /// </summary>
        /// <param name="listId">ID of list to delete</param>
        /// <exception cref="ListNotFoundException">List could not be found</exception>
        public async Task DeleteListAsync(int listId)
        {
            // If we found the list, then delete it
            _packedUnitOfWork.ListRepository.Delete(await GetList(listId));
            await _packedUnitOfWork.SaveChangesAsync();
        }

        #endregion METHODS

        #region HELPER METHODS

        /// <summary>
        /// Retrieve the specified list and throw an exception if it does not exist
        /// </summary>
        /// <param name="listId">List ID</param>
        /// <returns>
        /// The specified list
        /// </returns>
        /// <exception cref="ListNotFoundException">The list could not be found</exception>
        private async Task<List> GetList(int listId)
        {
            // Attempt to find the list
            var foundList = await _packedUnitOfWork.ListRepository.GetListByIdAsync(listId);

            // If we couldn't find the list, then throw a ListNotFoundException
            if (foundList is null)
            {
                throw new ListNotFoundException($"Could not find list with ID {listId}");
            }

            return foundList;
        }

        #endregion HELPER METHODS
    }
}