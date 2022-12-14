// Date Created: 2022/12/13
// Created by: JSW

using System;

namespace Packed.API.Core.Exceptions
{
    /// <summary>
    /// Exception which is raised when a list can't be found
    /// </summary>
    public class ListNotFoundException : PackedApiException
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Create a new exception
        /// </summary>
        public ListNotFoundException()
        {
        }

        /// <summary>
        /// Create a new exception with given message
        /// </summary>
        /// <param name="message">Exception message</param>
        public ListNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with given message and nested exception
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
        public ListNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion CONSTRUCTORS
    }
}