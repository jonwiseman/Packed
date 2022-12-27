// Date Created: 2022/12/27
// Created by: JSW

using System.Net;
using Newtonsoft.Json;
using Packed.API.Client.Exceptions;
using Packed.API.Client.Extensions;
using Packed.API.Core.DTOs;
using Packed.API.Core.Exceptions;

namespace Packed.API.Client;

/// <summary>
/// Client which provides a means of making requests to the Packed API
/// </summary>
public class PackedApiClient : IPackedApiClient
{
    #region FIELDS

    /// <summary>
    /// HTTP client for making API requests
    /// </summary>
    private readonly HttpClient _httpClient;

    #endregion FIELDS

    #region CONSTRUCTOR

    /// <summary>
    /// Create a new API client
    /// </summary>
    /// <param name="httpClient">Injected HTTP client for making API requests</param>
    /// <remarks>
    /// HTTP Client should be configured prior to injection
    /// </remarks>
    public PackedApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    #endregion CONSTRUCTOR

    #region LISTS

    /// <summary>
    /// Retrieve all lists which currently exist
    /// </summary>
    /// <returns>
    /// All lists which currently exist
    /// </returns>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    /// <exception cref="JsonSerializationException">Error deserializing response</exception>
    public async Task<IEnumerable<ListDto>> GetAllListsAsync()
    {
        // Create request message
        var request = new HttpRequestMessage(HttpMethod.Get, "lists");

        // Initialize a token source
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

        // Send request and wait for response
        var response =
            await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, tokenSource.Token);

        // Open stream to response
        await using var stream = await response.Content.ReadAsStreamAsync();

        // Based on the status code, either attempt to deserialize the response stream or throw an exception
        return response.StatusCode switch
        {
            // API should return all existing list
            HttpStatusCode.OK => await stream.ReadAndDeserializeFromJson<List<ListDto>>(),

            // Errors documented in OpenAPI specification
            HttpStatusCode.Unauthorized or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Retrieve the list with the specified ID, if it exists
    /// </summary>
    /// <param name="listId">ID of the list to search for</param>
    /// <returns>
    /// The specified list
    /// </returns>
    /// <exception cref="ListNotFoundException">The list could not be located</exception>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    /// <exception cref="JsonSerializationException">Error deserializing response</exception>
    public async Task<ListDto> GetListByIdAsync(int listId)
    {
        // Create request message
        var request = new HttpRequestMessage(HttpMethod.Get, $"lists/{listId}");

        // Initialize a token source
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

        // Send request and wait for response
        var response =
            await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, tokenSource.Token);

        // Open stream to response
        await using var stream = await response.Content.ReadAsStreamAsync();

        // Based on the status code, either attempt to deserialize the response stream or throw an exception
        return response.StatusCode switch
        {
            // API should return specified list
            HttpStatusCode.OK => await stream.ReadAndDeserializeFromJson<ListDto>(),

            // List could not be found
            HttpStatusCode.NotFound => throw new ListNotFoundException($"List with ID {listId} not found"),

            // Errors documented in OpenAPI specification
            HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Create a new list
    /// </summary>
    /// <param name="newList">New list</param>
    /// <returns>
    /// A representation of the new list
    /// </returns>
    /// <exception cref="DuplicateListException">A list with the given name already exists</exception>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    /// <exception cref="JsonSerializationException">Error deserializing response</exception>
    public async Task<(ListDto, string)> CreateNewListAsync(ListDto newList)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Post, "lists")
        {
            Content = new StringContent(JsonConvert.SerializeObject(newList))
        };

        // Initialize a token source
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

        // Send request and wait for response
        var response =
            await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, tokenSource.Token);

        // Open stream to response
        await using var stream = await response.Content.ReadAsStreamAsync();

        // Based on the status code, either attempt to deserialize the response stream or throw an exception
        return response.StatusCode switch
        {
            // API should return representation of created list
            HttpStatusCode.Created => (await stream.ReadAndDeserializeFromJson<ListDto>(),
                response.Headers.Location?.ToString() ?? string.Empty),

            // List with same description already exists
            HttpStatusCode.Conflict =>
                throw new DuplicateListException($"List with description {newList.Description} already exists"),

            // Errors documented in OpenAPI specification
            HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Update an existing list
    /// </summary>
    /// <param name="listId">ID of list to update</param>
    /// <param name="updatedList">Updated list</param>
    /// <returns>
    /// A representation of the updated list
    /// </returns>
    /// <exception cref="ListNotFoundException">The list could not be found</exception>
    /// <exception cref="DuplicateListException">List with given description already exists</exception>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    /// <exception cref="JsonSerializationException">Error deserializing response</exception>
    public async Task<ListDto> UpdateListAsync(int listId, ListDto updatedList)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Put, $"lists/{listId}")
        {
            Content = new StringContent(JsonConvert.SerializeObject(updatedList))
        };

        // Initialize a token source
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

        // Send request and wait for response
        var response =
            await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, tokenSource.Token);

        // Open stream to response
        await using var stream = await response.Content.ReadAsStreamAsync();

        // Based on the status code, either attempt to deserialize the response stream or throw an exception
        return response.StatusCode switch
        {
            // API should return representation of updated list
            HttpStatusCode.OK => await stream.ReadAndDeserializeFromJson<ListDto>(),

            // List could not be found
            HttpStatusCode.NotFound => throw new ListNotFoundException($"List with ID {listId} could not be found"),

            // List with same description already exists
            HttpStatusCode.Conflict =>
                throw new DuplicateListException($"List with description {updatedList.Description} already exists"),

            // Errors documented in OpenAPI specification
            HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Delete list with given ID
    /// </summary>
    /// <param name="listId">ID of list to delete</param>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    public async Task DeleteListAsync(int listId)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Delete, $"lists/{listId}");

        // Initialize a token source
        using var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

        // Send request and wait for response
        var response =
            await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, tokenSource.Token);

        // Open stream to response
        await using var stream = await response.Content.ReadAsStreamAsync();

        // Based on the status code, either attempt to deserialize the response stream or throw an exception
        switch (response.StatusCode)
        {
            case HttpStatusCode.NoContent:
                return;
            case HttpStatusCode.NotFound:
                throw new ListNotFoundException($"List with ID {listId} could not be found");
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.InternalServerError:
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>());
            default:
                throw new HttpRequestException(
                    $"Response with unexpected status code returned from request to {request.RequestUri}");
        }
    }

    #endregion LISTS
}