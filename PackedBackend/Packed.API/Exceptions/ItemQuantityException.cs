// Date Created: 2022/12/11
// Created by: JSW

namespace Packed.API.Exceptions;

/// <summary>
/// Exception to be thrown when an update to the quantity of an item
/// would cause an inconsistency with the current number of placements.
/// Basically, don't want to be able to have more placements than items
/// </summary>
public class ItemQuantityException : PackedApiException
{
    #region CONSTRUCTORS

    /// <summary>
    /// Create a new exception
    /// </summary>
    public ItemQuantityException()
    {
    }

    /// <summary>
    /// Create a new exception with given message
    /// </summary>
    /// <param name="message">Exception message</param>
    public ItemQuantityException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Create a new exception with given message and nested exception
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="innerException">Inner exception</param>
    public ItemQuantityException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    #endregion CONSTRUCTORS
}