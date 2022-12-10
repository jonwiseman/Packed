// Date Created: 2022/12/10
// Created by: JSW

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Packed.API.Services;
using Packed.Data.Core.DTOs;

namespace Packed.API.Controllers;

/// <summary>
/// Controller for handling all requests directly related to lists
/// </summary>
[Route("lists")]
[ApiController]
public class ListsController : ControllerBase
{
    #region FIELDS

    /// <summary>
    /// Service for interacting with data access layer
    /// </summary>
    private readonly IPackedDataService _packedDataService;

    #endregion FIELDS

    #region CONSTRUCTOR

    public ListsController(IPackedDataService packedDataService)
    {
        _packedDataService = packedDataService;
    }

    #endregion CONSTRUCTOR

    #region ACTION METHODS

    /// <summary>
    /// Get all lists
    /// </summary>
    /// <returns>
    /// All lists which currently exist
    /// </returns>
    [HttpGet]
    public async Task<ActionResult<List<ListDto>>> GetAllLists()
    {
        var lists = await _packedDataService.GetAllListsAsync();

        return Ok(lists);
    }

    /// <summary>
    /// Retrieve a specific list by ID
    /// </summary>
    /// <param name="listId">ID of the list to retrieve</param>
    /// <returns>
    /// The specified list or a 404 if not found
    /// </returns>
    [HttpGet("{listId:int}")]
    public async Task<ActionResult<ListDto>> GetListById([FromRoute] [Range(1, int.MaxValue)] int listId)
    {
        var foundList = await _packedDataService.GetListByIdAsync(listId);

        return foundList is null ? NotFound() : Ok(foundList);
    }

    #endregion ACTION METHODS
}