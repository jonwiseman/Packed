// Date Created: 2022/12/11
// Created by: JSW

namespace Packed.API.Exceptions;

/// <summary>
/// Exception to be thrown when an item can't be found
/// </summary>
public class ItemNotFoundException : PackedApiException
{
    #region CONSTRUCTORS

    /// <summary>
    /// Create a new exception
    /// </summary>
    public ItemNotFoundException()
    {
    }

    /// <summary>
    /// Create a new exception with given message
    /// </summary>
    /// <param name="message">Exception message</param>
    public ItemNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Create a new exception with given message which wraps an additional exception
    /// </summary>
    /// <param name="message">Exception message</param>
    /// <param name="innerException">Nested exception</param>
    public ItemNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    #endregion CONSTRUCTORS
}