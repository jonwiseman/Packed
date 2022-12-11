// Date Created: 2022/12/10
// Created by: JSW

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Packed.API.Exceptions;
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
            var createdList = await _packedDataService.CreateNewList(newList);
            return CreatedAtAction(nameof(GetListById), new
            {
                listId = createdList.Id
            }, createdList);
        }
        // If 
        catch (DuplicateListException e)
        {
            return Conflict();
        }
    }

    /// <summary>
    /// Retrieve a specific list by ID
    /// </summary>
    /// <param name="listId">ID of the list to retrieve</param>
    [HttpGet("{listId}")]
    public async Task<ActionResult<ListDto>> GetListById([FromRoute] [Range(1, int.MaxValue)] int listId)
    {
        var foundList = await _packedDataService.GetListByIdAsync(listId);

        return foundList is null ? NotFound() : Ok(foundList);
    }

    #endregion ACTION METHODS
}