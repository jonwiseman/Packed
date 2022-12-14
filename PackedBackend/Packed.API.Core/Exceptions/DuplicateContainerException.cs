// Date Created: 2022/12/13
// Created by: JSW

using System;

namespace Packed.API.Core.Exceptions
{
    /// <summary>
    /// Exception to be thrown when a container with the same name already exists in a list
    /// </summary>
    public class DuplicateContainerException : PackedApiException
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Create a new exception
        /// </summary>
        public DuplicateContainerException()
        {
        }

        /// <summary>
        /// Create a new exception with given message
        /// </summary>
        /// <param name="message">Error message</param>
        public DuplicateContainerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with given message and nested inner exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Nested exception</param>
        public DuplicateContainerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion CONSTRUCTORS
    }
}