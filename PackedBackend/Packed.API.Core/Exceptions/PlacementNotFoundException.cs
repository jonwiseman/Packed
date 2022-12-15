// Date Created: 2022/12/15
// Created by: JSW

using System;

namespace Packed.API.Core.Exceptions
{
    /// <summary>
    /// Exception to be thrown when a placement can't be found
    /// </summary>
    public class PlacementNotFoundException : PackedApiException
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Create a new exception
        /// </summary>
        public PlacementNotFoundException()
        {
        }

        /// <summary>
        /// Create a new exception with given message
        /// </summary>
        /// <param name="message">Error message</param>
        public PlacementNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new exception with given message and nested inner exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Nested exception</param>
        public PlacementNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion CONSTRUCTORS
    }
}