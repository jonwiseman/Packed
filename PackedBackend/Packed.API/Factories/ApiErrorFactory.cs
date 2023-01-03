// Date Created: 2022/12/10
// Created by: JSW

using System.Net;
using Packed.API.Core.DTOs;

namespace Packed.API.Factories;

/// <summary>
/// Standard implementation of the <see cref="ApiErrorFactoryBase"/> abstract class
/// for creating API errors
/// </summary>
public class ApiErrorFactory : ApiErrorFactoryBase
{
    /// <summary>
    /// Create an API error given the status code
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>
    /// An appropriately initialized error object to be returned to the client
    /// </returns>
    protected override ApiError CreateApiError(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.InternalServerError => new ApiError
            {
                Type = "errors/InternalServerError",
                Title = "Internal Server Error",
                StatusCode = (int)HttpStatusCode.InternalServerError
            },
            HttpStatusCode.NotFound => new ApiError
            {
                Type = "errors/NotFound",
                Title = "Resource Not Found",
                StatusCode = (int)HttpStatusCode.NotFound
            },
            HttpStatusCode.Conflict => new ApiError
            {
                Type = "errors/Conflict",
                Title = "Resource Conflict",
                StatusCode = (int)HttpStatusCode.Conflict
            },
            HttpStatusCode.BadRequest => new ApiError
            {
                Type = "errors/BadRequest",
                Title = "Bad Request",
                StatusCode = (int)HttpStatusCode.BadRequest
            },
            // Careful you don't throw an exception while trying to notify client about an error
            _ => throw new ArgumentOutOfRangeException(nameof(statusCode))
        };
    }
}