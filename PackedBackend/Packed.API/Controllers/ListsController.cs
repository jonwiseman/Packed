// Date Created: 2022/12/10
// Created by: JSW

using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Packed.API.Exceptions;
using Packed.API.Factories;
using Packed.API.Filters;
using Packed.API.Services;
using Packed.Data.Core.DTOs;

namespace Packed.API.Controllers;

/// <summary>
/// Controller for handling all requests directly related to lists
/// </summary>
[TypeFilter(typeof(ModelStateInvalidFilter))]
[TypeFilter(typeof(UnhandledExceptionFilter))]
[Route("lists")]
[ApiController]
public class ListsController : ControllerBase
{
    #region CONSTRUCTOR

    public ListsController(IPackedDataService packedDataService, ApiErrorFactoryBase apiErrorFactory)
    {
        _packedDataService = packedDataService ?? throw new ArgumentNullException(nameof(packedDataService));
        _apiErrorFactory = apiErrorFactory ?? throw new ArgumentNullException(nameof(apiErrorFactory));
    }

    #endregion CONSTRUCTOR

    #region FIELDS

    /// <summary>
    /// Service for interacting with data access layer
    /// </summary>
    private readonly IPackedDataService _packedDataService;

    /// <summary>
    /// Factory for creating and returning API errors
    /// </summary>
    private readonly ApiErrorFactoryBase _apiErrorFactory;

    #endregion FIELDS

    #region ACTION METHODS

    /// <summary>
    /// Get all lists
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ListDto>>> GetAllLists()
    {
        var lists = await _packedDataService.GetAllListsAsync();

        return Ok(lists);
    }

    /// <summary>
    /// Create a new list
    /// </summary>
    /// <param name="newList">New list to create</param>
    [HttpPost]
    public async Task<ActionResult<ListDto>> CreateNewList([FromBody] ListDto newList)
    {
        try
        {
            // Try to create the new list
            var createdList = await _packedDataService.CreateNewListAsync(newList);
            return CreatedAtAction(nameof(GetListById), new
            {
                listId = createdList.Id
            }, createdList);
        }
        // Case where we try to create a list with a description which already exists
        catch (DuplicateListException)
        {
            return Conflict(_apiErrorFactory.GetApiError(HttpStatusCode.Conflict,
                $"A list with description '{newList.Description}' already exists",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Retrieve a specific list by ID
    /// </summary>
    /// <param name="listId">ID of the list to retrieve</param>
    [HttpGet("{listId}")]
    public async Task<ActionResult<ListDto>> GetListById([FromRoute] [Range(1, int.MaxValue)] int listId)
    {
        try
        {
            return Ok(await _packedDataService.GetListByIdAsync(listId));
        }
        catch (ListNotFoundException)
        {
            return NotFound(_apiErrorFactory.GetApiError(HttpStatusCode.NotFound,
                $"List with ID {listId} could not be found",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Update an existing list
    /// </summary>
    /// <param name="listId">ID of list to update</param>
    /// <param name="updatedList">Updated list representation</param>
    [HttpPut("{listId}")]
    public async Task<ActionResult<ListDto>> UpdateList([FromRoute] [Range(1, int.MaxValue)] int listId,
        [FromBody] ListDto updatedList)
    {
        try
        {
            // Try to update the list with given ID
            return Ok(await _packedDataService.UpdateListAsync(listId, updatedList));
        }
        // Case where we couldn't find the list we were supposed to update
        catch (ListNotFoundException)
        {
            return NotFound(_apiErrorFactory.GetApiError(HttpStatusCode.NotFound,
                $"List with ID {listId} could not be found",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
        // Case where we try to update description to a one which already exists
        catch (DuplicateListException)
        {
            return Conflict(_apiErrorFactory.GetApiError(HttpStatusCode.Conflict,
                $"A list with description '{updatedList.Description}' already exists",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Delete list with given ID
    /// </summary>
    /// <param name="listId">ID of list to delete</param>
    [HttpDelete("{listId}")]
    public async Task<IActionResult> DeleteList([FromRoute] [Range(1, int.MaxValue)] int listId)
    {
        try
        {
            // Try to delete the list
            await _packedDataService.DeleteListAsync(listId);

            // If successful, then return a 204 No Content
            return NoContent();
        }
        // If the list couldn't be found, then return a 404 Not Found
        catch (ListNotFoundException)
        {
            return NotFound(_apiErrorFactory.GetApiError(HttpStatusCode.NotFound,
                $"List with ID {listId} could not be found",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    #endregion ACTION METHODS
}