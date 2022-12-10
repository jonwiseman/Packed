// Date Created: 2022/12/10
// Created by: JSW

using Microsoft.AspNetCore.Mvc;
using Packed.Data.Core.DTOs;
using Packed.Data.Core.Repositories;

namespace Packed.API.Controllers;

[Route("lists")]
[ApiController]
public class ListController : ControllerBase
{
    /// <summary>
    /// Repository for interacting with lists
    /// </summary>
    private readonly IListRepository _listRepository;

    #region CONSTRUCTOR

    public ListController(IListRepository listRepository)
    {
        _listRepository = listRepository;
    }

    #endregion CONSTRUCTOR

    #region ACTION METHODS

    /// <summary>
    /// Get all lists
    /// </summary>
    /// <returns>
    /// All lists which currently exist
    /// </returns>
    public async Task<ActionResult<List<ListDto>>> GetAllLists()
    {
        // Find all lists which exist in the database
        var foundLists = await _listRepository.GetAllListsAsync();

        // Convert these list entities into DTO representations
        var returnLists = foundLists
            .Select(l => new ListDto(l))
            .ToList();

        return Ok(returnLists);
    }

    #endregion ACTION METHODS

    #region FIELDS

    #endregion FIELDS
}