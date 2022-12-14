// Date Created: 2022/12/13
// Created by: JSW

using System;

namespace Packed.API.Core.Exceptions
{
    /// <summary>
    /// Exception which is raised when trying to create a list with an ID which already exists
    /// </summary>
    public class DuplicateListException : PackedApiException
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Create a new exception
        /// </summary>
        public DuplicateListException()
        {
        }

        /// <summary>
        /// Create a new exception with given message
        /// </summary>
        /// <param name="message">Exception message</param>
        public DuplicateListException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with given message which wraps an additional exception
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Nested exception</param>
        public DuplicateListException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion CONSTRUCTORS
    }
}