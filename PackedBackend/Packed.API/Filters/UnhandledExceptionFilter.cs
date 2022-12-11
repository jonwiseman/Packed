// Date Created: 2022/12/10
// Created by: JSW

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Packed.API.Factories;

namespace Packed.API.Filters;

/// <summary>
/// An exception filter for catching unhandled exceptions and return HTTP 500s
/// </summary>
public class UnhandledExceptionFilter : ExceptionFilterAttribute
{
    #region FIELDS

    /// <summary>
    /// Factory for creating API error objects
    /// </summary>
    private readonly ApiErrorFactoryBase _apiErrorFactory;

    #endregion FIELDS

    #region CONSTRUCTOR

    /// <summary>
    /// Create a new exception filter
    /// </summary>
    /// <param name="apiErrorFactory">Factory for creating API error objects</param>
    public UnhandledExceptionFilter(ApiErrorFactoryBase apiErrorFactory)
    {
        _apiErrorFactory = apiErrorFactory;
    }

    #endregion CONSTRUCTOR

    /// <summary>
    /// Send back an HTTP 500 to the client
    /// </summary>
    /// <param name="context">Context</param>
    public override void OnException(ExceptionContext context)
    {
        // TODO: see about adding handling for common exceptions like ListNotFoundException, etc.

        // Send back an HTTP 500
        context.Result = new JsonResult(_apiErrorFactory.GetApiError(HttpStatusCode.InternalServerError,
            string.Empty, context.HttpContext.Request.Path))
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }
}