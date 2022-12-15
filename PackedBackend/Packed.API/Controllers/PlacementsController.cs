// Date Created: 2022/12/15
// Created by: JSW

using Microsoft.AspNetCore.Mvc;
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

    #endregion FIELDS

    #region CONSTRUCTOR

    public PlacementsController()
    {
        
    }

    #endregion CONSTRUCTOR

    #region ACTION METHODS

    #endregion ACTION METHODS
}