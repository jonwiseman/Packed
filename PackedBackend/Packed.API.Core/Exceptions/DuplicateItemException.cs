// Date Created: 2022/12/13
// Created by: JSW

using System;

namespace Packed.API.Core.Exceptions
{
    /// <summary>
    /// Exception to be thrown when trying to add an item to a list with the same
    /// name as an item already in that list
    /// </summary>
    public class DuplicateItemException : PackedApiException
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Create a new exception
        /// </summary>
        public DuplicateItemException()
        {
        }

        /// <summary>
        /// Create a new exception with given message
        /// </summary>
        /// <param name="message">Error message</param>
        public DuplicateItemException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with given message and nested inner exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Nested exception</param>
        public DuplicateItemException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion CONSTRUCTORS
    }
}