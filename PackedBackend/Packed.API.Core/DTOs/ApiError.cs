// Date Created: 2023/01/03
// Created by: JSW

using System;
using System.Text.Json.Serialization;

namespace Packed.API.Core.DTOs
{
    /// <summary>
    /// Standard DTO for all API errors
    /// </summary>
    public class ApiError
    {
        #region CONSTRUCTOR

        /// <summary>
        /// Default constructor.  Initialize time and set a random error GUID
        /// </summary>
        public ApiError()
        {
            ErrorId = Guid.NewGuid();
            TimeStamp = DateTime.UtcNow;
        }

        #endregion CONSTRUCTOR

        #region PROPERTIES

        /// <summary>
        /// URI to resource describing the error
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Short title of error
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// HTTP status code of error
        /// </summary>
        [JsonPropertyName("status")]
        public int StatusCode { get; set; }

        /// <summary>
        /// Detailed description of the error
        /// </summary>
        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        /// <summary>
        /// Request URI which caused the error
        /// </summary>
        [JsonPropertyName("instance")]
        public string Instance { get; set; }

        /// <summary>
        /// Unique tracking number for the error
        /// </summary>
        [JsonPropertyName("errorId")]
        public Guid ErrorId { get; }

        /// <summary>
        /// Time at which the error occurred
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime TimeStamp { get; }

        #endregion PROPERTIES
    }
}