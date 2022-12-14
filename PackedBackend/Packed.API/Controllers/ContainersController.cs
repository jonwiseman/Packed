// Date Created: 2022/12/13
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
/// Controller which handles all requests related to containers
/// </summary>
[TypeFilter(typeof(ModelStateInvalidFilter))]
[TypeFilter(typeof(UnhandledExceptionFilter))]
[Route("/lists/{listId}/containers")]
[ApiController]
public class ContainersController : ControllerBase
{
    #region FIELDS

    /// <summary>
    /// Data service for manipulating containers
    /// </summary>
    private readonly IPackedContainersDataService _containersDataService;

    /// <summary>
    /// Factory for creating and returning API error objects
    /// </summary>
    private readonly ApiErrorFactoryBase _apiErrorFactory;

    #endregion FIELDS

    #region CONSTRUCTOR

    /// <summary>
    /// Create a new controller
    /// </summary>
    /// <param name="containersDataService">Data service for manipulating containers</param>
    /// <param name="apiErrorFactory">Factory for creating and returning API error objects</param>
    public ContainersController(IPackedContainersDataService containersDataService, ApiErrorFactoryBase apiErrorFactory)
    {
        _containersDataService =
            containersDataService ?? throw new ArgumentNullException(nameof(containersDataService));
        _apiErrorFactory = apiErrorFactory ?? throw new ArgumentNullException(nameof(apiErrorFactory));
    }

    #endregion CONSTRUCTOR

    #region ACTION METHODS

    /// <summary>
    /// Get all containers which exist for the specified list
    /// </summary>
    /// <param name="listId">List ID</param>
    [HttpGet]
    public async Task<ActionResult<List<ContainerDto>>> GetContainers([FromRoute] [Range(0, int.MaxValue)] int listId)
    {
        try
        {
            return Ok(await _containersDataService.GetContainersAsync(listId));
        }
        catch (ListNotFoundException e)
        {
            return NotFound(_apiErrorFactory.GetApiError(
                HttpStatusCode.NotFound,
                e.Message,
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Add a new container to the specified list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="newContainer">New container</param>
    [HttpPost]
    public async Task<ActionResult<ContainerDto>> AddContainer([FromRoute] [Range(0, int.MaxValue)] int listId,
        [FromBody] ContainerDto newContainer)
    {
        try
        {
            // Create the new container
            var createdContainer = await _containersDataService.AddContainerAsync(listId, newContainer);

            // Return an HTTP 201 with a link to the new container
            return CreatedAtAction(nameof(GetContainerById), new
            {
                listId,
                containerId = createdContainer.Id
            }, createdContainer);
        }
        // Case where list couldn't be found
        catch (ListNotFoundException e)
        {
            return NotFound(_apiErrorFactory.GetApiError(
                HttpStatusCode.NotFound,
                e.Message,
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
        // Case where container with same name already exists 
        catch (DuplicateContainerException e)
        {
            return Conflict(_apiErrorFactory.GetApiError(
                HttpStatusCode.Conflict,
                e.Message,
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Retrieve a specific container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="containerId">Container ID</param>
    [HttpGet("{containerId}")]
    public async Task<ActionResult<ContainerDto>> GetContainerById([FromRoute] [Range(0, int.MaxValue)] int listId,
        [FromRoute] [Range(0, int.MaxValue)] int containerId)
    {
        try
        {
            return Ok(await _containersDataService.GetContainerByIdAsync(listId, containerId));
        }
        // Cases where list or container don't exist. Combine into one catch since we're returning a 404 in either case
        catch (PackedApiException e)
        {
            return NotFound(_apiErrorFactory.GetApiError(
                HttpStatusCode.NotFound,
                e.Message,
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Update an existing container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="containerId">Container ID</param>
    /// <param name="updatedContainer">The updated container</param>
    [HttpPut("{containerId}")]
    public async Task<ActionResult<ContainerDto>> UpdateContainer([FromRoute] [Range(0, int.MaxValue)] int listId,
        [FromRoute] [Range(0, int.MaxValue)] int containerId, ContainerDto updatedContainer)
    {
        try
        {
            return Ok(await _containersDataService.UpdateContainerAsync(listId, containerId, updatedContainer));
        }
        // Case where list couldn't be found
        catch (ListNotFoundException e)
        {
            return NotFound(_apiErrorFactory.GetApiError(
                HttpStatusCode.NotFound,
                e.Message,
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
        // Case where container could not be found
        catch (ContainerNotFoundException e)
        {
            return NotFound(_apiErrorFactory.GetApiError(
                HttpStatusCode.NotFound,
                e.Message,
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
        // Case where container with same name already exists 
        catch (DuplicateContainerException e)
        {
            return Conflict(_apiErrorFactory.GetApiError(
                HttpStatusCode.Conflict,
                e.Message,
                ControllerContext.HttpContext.Request.Path.ToString()));
        }
    }

    /// <summary>
    /// Delete an existing container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="containerId">Container ID</param>
    [HttpDelete("{containerId}")]
    public async Task<IActionResult> DeleteContainer([FromRoute] [Range(0, int.MaxValue)] int listId,
        [FromRoute] [Range(0, int.MaxValue)] int containerId)
    {
        try
        {
            // Try to delete the container
            await _containersDataService.DeleteContainerAsync(listId, containerId);

            // On success, return an HTTP 204 No Content
            return NoContent();
        }
        // Cases where list or container don't exist. Combine into one catch since we're returning a 404 in either case
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