// Date Created: 2022/12/10
// Created by: JSW

namespace Packed.API.Exceptions;

/// <summary>
/// Base class for all Packed API exceptions
/// </summary>
public abstract class PackedApiException : Exception
{
    #region CONSTRUCTORS

    /// <summary>
    /// Create a new exception
    /// </summary>
    protected PackedApiException()
    {
    }

    /// <summary>
    /// Create a new exception with given message
    /// </summary>
    /// <param name="message">Exception message</param>
    protected PackedApiException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Create a new exception with given message which wraps an additional exception
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Nested exception</param>
    protected PackedApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    #endregion CONSTRUCTORS
}