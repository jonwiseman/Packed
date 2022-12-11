// Date Created: 2022/12/10
// Created by: JSW

using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Packed.API.Exceptions;
using Packed.API.Factories;
using Packed.API.Services;
using Packed.Data.Core.DTOs;

namespace Packed.API.Controllers;

/// <summary>
/// Controller which handles all requests directly related to items
/// </summary>
[Route("lists/{listId}/items")]
[ApiController]
public class ItemsController : ControllerBase
{
    #region FIELDS

    /// <summary>
    /// Data service for interacting with Packed data
    /// </summary>
    private readonly IPackedDataService _packedDataService;

    /// <summary>
    /// Factory for creating API error objects
    /// </summary>
    private readonly ApiErrorFactoryBase _apiErrorFactory;

    #endregion FIELDS

    #region CONSTRUCTOR

    public ItemsController(IPackedDataService packedDataService, ApiErrorFactoryBase apiErrorFactory)
    {
        _packedDataService = packedDataService ?? throw new ArgumentNullException(nameof(packedDataService));
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
            return Ok(await _packedDataService.GetItemsForListAsync(listId));
        }
        catch (ListNotFoundException)
        {
            return NotFound(_apiErrorFactory.GetApiError(HttpStatusCode.NotFound,
                $"List with ID {listId} could not be found",
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    #endregion ACTION METHODS
}