// Date Created: 2022/12/10
// Created by: JSW

using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Packed.API.Core.DTOs;
using Packed.API.Core.Exceptions;
using Packed.API.Core.Services;
using Packed.API.Factories;
using Packed.API.Filters;

namespace Packed.API.Controllers;

/// <summary>
/// Controller which handles all requests directly related to items
/// </summary>
[TypeFilter(typeof(ModelStateInvalidFilter))]
[TypeFilter(typeof(UnhandledExceptionFilter))]
[Route("lists/{listId}/items")]
[ApiController]
public class ItemsController : ControllerBase
{
    #region FIELDS

    /// <summary>
    /// Data service for interacting with Packed data
    /// </summary>
    private readonly IPackedItemsDataService _packedItemsDataService;

    /// <summary>
    /// Factory for creating API error objects
    /// </summary>
    private readonly ApiErrorFactoryBase _apiErrorFactory;

    #endregion FIELDS

    #region CONSTRUCTOR

    /// <summary>
    /// Create a new items controller
    /// </summary>
    /// <param name="packedItemsDataService">Packed data service</param>
    /// <param name="apiErrorFactory">Error factory</param>
    public ItemsController(IPackedItemsDataService packedItemsDataService, ApiErrorFactoryBase apiErrorFactory)
    {
        _packedItemsDataService =
            packedItemsDataService ?? throw new ArgumentNullException(nameof(packedItemsDataService));
        _apiErrorFactory = apiErrorFactory ?? throw new ArgumentNullException(nameof(apiErrorFactory));
    }

    #endregion CONSTRUCTOR

    #region ACTION METHODS

    /// <summary>
    /// Get all items in the specified list
    /// </summary>
    /// <param name="listId">List ID</param>
    [HttpGet]
    public async Task<ActionResult<ItemDto>> GetAllItems([FromRoute] [Range(1, int.MaxValue)] int listId)
    {
        try
        {
            return Ok(await _packedItemsDataService.GetItemsForListAsync(listId));
        }
        catch (ListNotFoundException)
        {
            return NotFound(_apiErrorFactory.GetApiError(HttpStatusCode.NotFound,
                $"List with ID {listId} could not be found",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Create a new item inside the specified list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="newItem">New item to create</param>
    [HttpPost]
    public async Task<ActionResult<ItemDto>> CreateNewItemForList([FromRoute] [Range(1, int.MaxValue)] int listId,
        [FromBody] ItemDto newItem)
    {
        try
        {
            // Attempt to add item to list
            var addedItem = await _packedItemsDataService.AddItemToListAsync(listId, newItem);

            // Return a 201 Created with a link to retrieve the created item
            return CreatedAtAction(nameof(GetItemById), new
            {
                listId,
                itemId = addedItem.Id
            }, addedItem);
        }
        // Case where specified list could not be found
        catch (ListNotFoundException)
        {
            return NotFound(_apiErrorFactory.GetApiError(HttpStatusCode.NotFound,
                $"List with ID {listId} could not be found",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
        // Case where an item with the same name already exists in the list
        catch (DuplicateItemException)
        {
            return Conflict(_apiErrorFactory.GetApiError(HttpStatusCode.Conflict,
                $"Item with name '{newItem.Name}' already exists in list with ID {listId}",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Retrieve the specified item
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    [HttpGet("{itemId}")]
    public async Task<ActionResult<ItemDto>> GetItemById([FromRoute] [Range(1, int.MaxValue)] int listId,
        [FromRoute] [Range(1, int.MaxValue)] int itemId)
    {
        try
        {
            return Ok(await _packedItemsDataService.GetItemByIdAsync(listId, itemId));
        }
        // Case where list not found
        catch (ListNotFoundException)
        {
            return NotFound(_apiErrorFactory.GetApiError(HttpStatusCode.NotFound,
                $"List with ID {listId} could not be found",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
        // Case where item not found
        catch (ItemNotFoundException)
        {
            return NotFound(_apiErrorFactory.GetApiError(HttpStatusCode.NotFound,
                $"Item with ID {itemId} could not be found in list with ID {listId}",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Update an existing item
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="updatedItem">Updated item</param>
    [HttpPut("{itemId}")]
    public async Task<ActionResult<ItemDto>> UpdateItem([FromRoute] [Range(1, int.MaxValue)] int listId,
        [FromRoute] [Range(1, int.MaxValue)] int itemId, [FromBody] ItemDto updatedItem)
    {
        try
        {
            return Ok(await _packedItemsDataService.UpdateItemAsync(listId, itemId, updatedItem));
        }
        // Case where specified list could not be found
        catch (ListNotFoundException)
        {
            return NotFound(_apiErrorFactory.GetApiError(HttpStatusCode.NotFound,
                $"List with ID {listId} could not be found",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
        // Case where item not found
        catch (ItemNotFoundException)
        {
            return NotFound(_apiErrorFactory.GetApiError(HttpStatusCode.NotFound,
                $"Item with ID {itemId} could not be found in list with ID {listId}",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
        // Case where an item with the same name already exists in the list
        catch (DuplicateItemException)
        {
            return Conflict(_apiErrorFactory.GetApiError(HttpStatusCode.Conflict,
                $"Item with name '{updatedItem.Name}' already exists in list with ID {listId}",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
        // Case where there would be more items than placements
        catch (ItemQuantityException)
        {
            return Conflict(_apiErrorFactory.GetApiError(HttpStatusCode.Conflict,
                $"Cannot change to {updatedItem.Quantity} due to existing placements",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Delete an item
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    [HttpDelete("{itemId}")]
    public async Task<IActionResult> DeleteItem([FromRoute] [Range(1, int.MaxValue)] int listId,
        [FromRoute] [Range(1, int.MaxValue)] int itemId)
    {
        try
        {
            await _packedItemsDataService.DeleteItemAsync(listId, itemId);
            return NoContent();
        }
        // Case where specified list could not be found
        catch (ListNotFoundException)
        {
            return NotFound(_apiErrorFactory.GetApiError(HttpStatusCode.NotFound,
                $"List with ID {listId} could not be found",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
        // Case where item not found
        catch (ItemNotFoundException)
        {
            return NotFound(_apiErrorFactory.GetApiError(HttpStatusCode.NotFound,
                $"Item with ID {itemId} could not be found in list with ID {listId}",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    #endregion ACTION METHODS
}