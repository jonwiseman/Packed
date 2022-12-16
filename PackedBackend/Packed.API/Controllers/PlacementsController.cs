// Date Created: 2022/12/15
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
/// Controller for handling all requests related to placements
/// </summary>
[TypeFilter(typeof(ModelStateInvalidFilter))]
[TypeFilter(typeof(UnhandledExceptionFilter))]
[Route("lists/{listId}/items/{itemId}/placements")]
[ApiController]
public class PlacementsController : ControllerBase
{
    #region FIELDS

    /// <summary>
    /// Data service for manipulating placements
    /// </summary>
    private readonly IPackedPlacementsDataService _placementsDataService;

    /// <summary>
    /// Factory for creating and returning API errors
    /// </summary>
    private readonly ApiErrorFactoryBase _apiErrorFactory;

    #endregion FIELDS

    #region CONSTRUCTOR

    /// <summary>
    /// Create a new controller
    /// </summary>
    /// <param name="placementsDataService">Data service for manipulating placements</param>
    /// <param name="apiErrorFactory">Factory for creating and returning API errors</param>
    public PlacementsController(IPackedPlacementsDataService placementsDataService, ApiErrorFactoryBase apiErrorFactory)
    {
        _placementsDataService =
            placementsDataService ?? throw new ArgumentNullException(nameof(placementsDataService));
        _apiErrorFactory = apiErrorFactory ?? throw new ArgumentNullException(nameof(apiErrorFactory));
    }

    #endregion CONSTRUCTOR

    #region ACTION METHODS

    /// <summary>
    /// Get all placements
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    [HttpGet]
    public async Task<ActionResult<List<PlacementDto>>> GetPlacements([FromRoute] [Range(1, int.MaxValue)] int listId,
        [FromRoute] [Range(1, int.MaxValue)] int itemId)
    {
        try
        {
            return Ok(await _placementsDataService.GetPlacementsForItemAsync(listId, itemId));
        }
        // Case where list or item not found
        catch (PackedApiException e)
        {
            return NotFound(_apiErrorFactory.GetApiError(
                HttpStatusCode.NotFound,
                e.Message,
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Place an item into a container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="newPlacement">New placement</param>
    [HttpPost]
    public async Task<ActionResult<PlacementDto>> AddPlacement([FromRoute] [Range(1, int.MaxValue)] int listId,
        [FromRoute] [Range(1, int.MaxValue)] int itemId, [FromBody] PlacementDto newPlacement)
    {
        try
        {
            var createdPlacement = await _placementsDataService.PlaceItemAsync(listId, itemId, newPlacement);

            return CreatedAtAction(nameof(GetPlacement), new
            {
                listId,
                itemId,
                placementId = createdPlacement.Id
            }, createdPlacement);
        }
        catch (PackedApiException e)
        {
            // Case where placing the item would lead to too many placements
            if (e is ItemQuantityException)
            {
                return Conflict(_apiErrorFactory.GetApiError(
                    HttpStatusCode.Conflict,
                    e.Message,
                    ControllerContext.HttpContext.Request.Path.ToString()));
            }

            // Case where list or item couldn't be found
            return NotFound(_apiErrorFactory.GetApiError(
                HttpStatusCode.NotFound,
                e.Message,
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Get a specific placement
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="placementId">Placement ID</param>
    [HttpGet("{placementId}")]
    public async Task<ActionResult<PlacementDto>> GetPlacement([FromRoute] [Range(1, int.MaxValue)] int listId,
        [FromRoute] [Range(1, int.MaxValue)] int itemId, [FromRoute] [Range(1, int.MaxValue)] int placementId)
    {
        try
        {
            return Ok(await _placementsDataService.GetPlacementByIdAsync(listId, itemId, placementId));
        }
        // Case where list, item, or placement not found
        catch (PackedApiException e)
        {
            return NotFound(_apiErrorFactory.GetApiError(
                HttpStatusCode.NotFound,
                e.Message,
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Delete a placement
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="placementId">Placement ID</param>
    [HttpDelete("{placementId}")]
    public async Task<IActionResult> DeletePlacement([FromRoute] [Range(1, int.MaxValue)] int listId,
        [FromRoute] [Range(1, int.MaxValue)] int itemId, [FromRoute] [Range(1, int.MaxValue)] int placementId)
    {
        try
        {
            // Delete the placement
            await _placementsDataService.DeletePlacementAsync(listId, itemId, placementId);

            // Return an HTTP 204 No Content
            return NoContent();
        }
        // Case where list, item, or placement not found
        catch (PackedApiException e)
        {
            return NotFound(_apiErrorFactory.GetApiError(
                HttpStatusCode.NotFound,
                e.Message,
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    #endregion ACTION METHODS
}