// Date Created: 2022/12/10
// Created by: JSW

using System.Net;
using Packed.API.Core.DTOs;

namespace Packed.API.Factories;

/// <summary>
/// Base class for factory method pattern for creating API errors
/// </summary>
public abstract class ApiErrorFactoryBase
{
    /// <summary>
    /// Create an API error given the status code
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>
    /// An appropriately initialized error object to be returned to the client
    /// </returns>
    protected abstract PackedApiError CreateApiError(HttpStatusCode statusCode);

    /// <summary>
    /// Create an API error object
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="detail">Specific details about the error</param>
    /// <param name="requestPath">Path client requested when error was triggered</param>
    /// <returns>
    /// A completed error object to be returned to the client
    /// </returns>
    public PackedApiError GetApiError(HttpStatusCode statusCode, string detail, string requestPath)
    {
        // Allow factory method to create most properties
        var errorDto = CreateApiError(statusCode);

        // Set the detail ourselves since this has to be done on all errors
        errorDto.Detail = statusCode is HttpStatusCode.InternalServerError
            ? "An internal server error occurred during the processing of the request"
            : detail;

        // Set the instance or ourselves since this has to be done for all errors
        errorDto.Instance = requestPath;

        // Finally, return the error object
        return errorDto;
    }
}