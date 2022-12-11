// Date Created: 2022/12/10
// Created by: JSW

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Packed.API.Factories;

namespace Packed.API.Filters;

/// <summary>
/// An action filter for catching invalid model state
/// </summary>
public class ModelStateInvalidFilter : ActionFilterAttribute
{
    #region FIELDS

    /// <summary>
    /// Factory for creating new API error objects
    /// </summary>
    private readonly ApiErrorFactoryBase _apiErrorFactory;

    #endregion FIELDS

    #region CONSTRUCTOR

    /// <summary>
    /// Create a new model state invalid filter
    /// </summary>
    /// <param name="apiErrorFactory"></param>
    public ModelStateInvalidFilter(ApiErrorFactoryBase apiErrorFactory)
    {
        _apiErrorFactory = apiErrorFactory ?? throw new ArgumentNullException(nameof(apiErrorFactory));
    }

    #endregion CONSTRUCTOR

    #region METHODS

    /// <summary>
    /// Check model state. If invalid, return a 400 Bad Request to the client
    /// </summary>
    /// <param name="context">Context</param>
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // If model state is valid, then do nothing
        if (context.ModelState.IsValid) return;

        // Return an HTTP 400 Bad Request to the client
        context.Result = new JsonResult(_apiErrorFactory.GetApiError(
            HttpStatusCode.BadRequest,
            "Client made an improperly formatted request",
            context.HttpContext.Request.Path)
        )
        {
            StatusCode = (int)HttpStatusCode.BadRequest
        };
    }

    #endregion METHODS
}