// Date Created: 2023/01/03
// Created by: JSW

using Packed.API.Client.Responses;

namespace Packed.API.Client.Exceptions;

/// <summary>
/// Exception to be thrown when API client encounters an error which is documented in OpenAPI specification. These errors
/// should have defined error bodies, meaning we can retrieve information about them
/// </summary>
public class PackedApiClientException : Exception
{
    #region CONSTRUCTORS

    /// <summary>
    /// Create a new exception
    /// </summary>
    /// <param name="apiErrorMessageBody">Error body returned from Packed API</param>
    public PackedApiClientException(PackedApiError apiErrorMessageBody)
    {
        ApiErrorMessageBody = apiErrorMessageBody ?? throw new ArgumentNullException(nameof(apiErrorMessageBody));
    }

    #endregion CONSTRUCTORS

    #region PROPERTIES

    /// <summary>
    /// Error body returned from Packed API
    /// </summary>
    public PackedApiError ApiErrorMessageBody { get; }

    #endregion PROPERTIES
}