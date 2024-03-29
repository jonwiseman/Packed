// Date Created: 2023/01/03
// Created by: JSW

using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Packed.API.Client.Exceptions;
using Packed.API.Client.Responses;
using Packed.API.Core.Exceptions;
using Packed.API.Extensions;

namespace Packed.API.Client;

/// <summary>
/// Client which provides a means of making requests to the Packed API
/// </summary>
/// <remarks>
/// TODO: add common base methods
/// </remarks>
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
    public async Task<IEnumerable<PackedList>> GetAllListsAsync()
    {
        // Create request message
        var request = new HttpRequestMessage(HttpMethod.Get, "lists")
        {
            Headers = { { "Accept", "application/json" } }
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
            // API should return all existing list
            HttpStatusCode.OK => await stream.ReadAndDeserializeFromJson<List<PackedList>>(),

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
    public async Task<PackedList> GetListByIdAsync(int listId)
    {
        // Create request message
        var request = new HttpRequestMessage(HttpMethod.Get, $"lists/{listId}")
        {
            Headers = { { "Accept", "application/json" } }
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
            // API should return specified list
            HttpStatusCode.OK => await stream.ReadAndDeserializeFromJson<PackedList>(),

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
    /// <param name="description">New list</param>
    /// <returns>
    /// A representation of the new list
    /// </returns>
    /// <exception cref="DuplicateListException">A list with the given name already exists</exception>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    /// <exception cref="JsonSerializationException">Error deserializing response</exception>
    public async Task<(PackedList, string)> CreateNewListAsync(string description)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Post, "lists")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new { description },
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Encoding.UTF8, "application/json")
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
            HttpStatusCode.Created => (await stream.ReadAndDeserializeFromJson<PackedList>(),
                response.Headers.Location?.ToString() ?? string.Empty),

            // List with same description already exists
            HttpStatusCode.Conflict =>
                throw new DuplicateListException($"List with description {description} already exists"),

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
    /// <param name="updatedDescription">Updated list description</param>
    /// <returns>
    /// A representation of the updated list
    /// </returns>
    /// <exception cref="ListNotFoundException">The list could not be found</exception>
    /// <exception cref="DuplicateListException">List with given description already exists</exception>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    /// <exception cref="JsonSerializationException">Error deserializing response</exception>
    public async Task<PackedList> UpdateListAsync(int listId, string updatedDescription)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Put, $"lists/{listId}")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new { description = updatedDescription },
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Encoding.UTF8, "application/json")
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
            HttpStatusCode.OK => await stream.ReadAndDeserializeFromJson<PackedList>(),

            // List could not be found
            HttpStatusCode.NotFound => throw new ListNotFoundException($"List with ID {listId} could not be found"),

            // List with same description already exists
            HttpStatusCode.Conflict =>
                throw new DuplicateListException($"List with description {updatedDescription} already exists"),

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

    #region ITEMS

    /// <summary>
    /// Get all items for the given list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <returns>
    /// All items belonging to the specified list
    /// </returns>
    /// <exception cref="ListNotFoundException">The list could not be found</exception>
    /// <exception cref="PackedApiClientException">Encountered a documented API error</exception>
    /// <exception cref="HttpRequestException">Encountered an undocumented API error</exception>
    public async Task<IEnumerable<PackedItem>> GetItemsForListAsync(int listId)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Get, $"lists/{listId}/items")
        {
            Headers = { { "Accept", "application/json" } }
        };

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
            case HttpStatusCode.OK:
                return await stream.ReadAndDeserializeFromJson<List<PackedItem>>();
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

    /// <summary>
    /// Create a new item in the specified list
    /// </summary>
    /// <param name="listId">ID of list to add item to</param>
    /// <param name="name">Name of new item</param>
    /// <param name="quantity">New item quantity</param>
    /// <returns>
    /// A representation of the new item and a link to the location of the new item
    /// </returns>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    /// <exception cref="DuplicateItemException">Item with same name already in list</exception>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    public async Task<(PackedItem, string)> CreateItemForListAsync(int listId, string name, int quantity)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Post, $"lists/{listId}/items")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        name,
                        quantity
                    },
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Encoding.UTF8, "application/json")
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
            // API should return representation of created item
            HttpStatusCode.Created => (await stream.ReadAndDeserializeFromJson<PackedItem>(),
                response.Headers.Location?.ToString() ?? string.Empty),

            HttpStatusCode.NotFound =>
                throw new ListNotFoundException($"List with ID {listId} not found"),

            // Item with same name already exists
            HttpStatusCode.Conflict =>
                throw new DuplicateItemException($"Item with name {name} already exists"),

            // Errors documented in OpenAPI specification
            HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Retrieve a specific item from the specified list
    /// </summary>
    /// <param name="listId">List to retrieve item from</param>
    /// <param name="itemId">ID of item to retrieve</param>
    /// <returns>
    /// The specified item
    /// </returns>
    /// <exception cref="PackedApiClientException">Recognized API error</exception>
    /// <exception cref="HttpRequestException">Unrecognized API error</exception>
    public async Task<PackedItem> GetItemFromListAsync(int listId, int itemId)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Get, $"lists/{listId}/items/{itemId}")
        {
            Headers = { { "Accept", "application/json" } }
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
            HttpStatusCode.OK => await stream.ReadAndDeserializeFromJson<PackedItem>(),

            // Errors documented in OpenAPI specification
            HttpStatusCode.NotFound or HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized
                or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Update an item
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="newName">New name of item</param>
    /// <param name="newQuantity">New quantity of item</param>
    /// <returns>
    /// A representation of the updated item
    /// </returns>
    /// <exception cref="DuplicateItemException">Item with same name already exists</exception>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    public async Task<PackedItem> UpdateItemAsync(int listId, int itemId, string newName, int newQuantity)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Put, $"lists/{listId}/items/{itemId}")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        name = newName,
                        quantity = newQuantity
                    },
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Encoding.UTF8, "application/json")
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
            // API should return representation of updated item
            HttpStatusCode.OK => await stream.ReadAndDeserializeFromJson<PackedItem>(),

            // Item with same name already exists
            HttpStatusCode.Conflict =>
                throw new DuplicateItemException($"Item with name {newName} already exists"),

            // Errors documented in OpenAPI specification
            HttpStatusCode.NotFound or HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized
                or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Delete an item
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    public async Task DeleteItemAsync(int listId, int itemId)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Delete, $"lists/{listId}/items/{itemId}");

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
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.InternalServerError:
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>());
            default:
                throw new HttpRequestException(
                    $"Response with unexpected status code returned from request to {request.RequestUri}");
        }
    }

    #endregion ITEMS

    #region CONTAINERS

    /// <summary>
    /// Get all containers belonging to a specific list
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <returns>
    /// All existing containers
    /// </returns>
    /// <exception cref="ListNotFoundException">List could not be found</exception>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    public async Task<IEnumerable<PackedContainer>> GetContainersForListAsync(int listId)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Get, $"lists/{listId}/containers")
        {
            Headers = { { "Accept", "application/json" } }
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
            HttpStatusCode.OK => await stream.ReadAndDeserializeFromJson<List<PackedContainer>>(),

            HttpStatusCode.NotFound => throw new ListNotFoundException($"List with ID {listId} could not be found"),

            // Errors documented in OpenAPI specification
            HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Create a new container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="name">Container name</param>
    /// <returns>
    /// A representation of the created container and a link to the container's location
    /// </returns>
    /// <exception cref="ListNotFoundException">List not found</exception>
    /// <exception cref="DuplicateContainerException">Container with same name already exists</exception>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    public async Task<(PackedContainer, string)> CreateContainerAsync(int listId, string name)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Post, $"lists/{listId}/containers")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        name
                    },
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Encoding.UTF8, "application/json")
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
            // API should return representation of created container
            HttpStatusCode.Created => (await stream.ReadAndDeserializeFromJson<PackedContainer>(),
                response.Headers.Location?.ToString() ?? string.Empty),

            HttpStatusCode.NotFound =>
                throw new ListNotFoundException($"List with ID {listId} not found"),

            // Container with same name already exists
            HttpStatusCode.Conflict =>
                throw new DuplicateContainerException($"Container with name {name} already exists"),

            // Errors documented in OpenAPI specification
            HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Get a specific container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="containerId">Container ID</param>
    /// <returns>
    /// The specified container
    /// </returns>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    public async Task<PackedContainer> GetContainerAsync(int listId, int containerId)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Get, $"lists/{listId}/containers/{containerId}")
        {
            Headers = { { "Accept", "application/json" } }
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
            HttpStatusCode.OK => await stream.ReadAndDeserializeFromJson<PackedContainer>(),

            // Errors documented in OpenAPI specification
            HttpStatusCode.NotFound or HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized
                or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Update a container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="containerId">Container ID</param>
    /// <param name="updatedName">Updated container name</param>
    /// <returns>
    /// A representation of the updated container
    /// </returns>
    /// <exception cref="DuplicateContainerException">Container with name already exists</exception>
    /// <exception cref="PackedApiClientException">Recognized API exception</exception>
    /// <exception cref="HttpRequestException">Unrecognized API exception</exception>
    public async Task<PackedContainer> UpdateContainerAsync(int listId, int containerId, string updatedName)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Put, $"lists/{listId}/containers/{containerId}")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        name = updatedName
                    },
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Encoding.UTF8, "application/json")
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
            // API should return representation of updated container
            HttpStatusCode.OK => await stream.ReadAndDeserializeFromJson<PackedContainer>(),

            // Container with same name already exists
            HttpStatusCode.Conflict =>
                throw new DuplicateContainerException($"Container with name {updatedName} already exists"),

            // Errors documented in OpenAPI specification
            HttpStatusCode.NotFound or HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized
                or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Delete a container
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="containerId">Container ID</param>
    /// <exception cref="PackedApiClientException">Recognized API error</exception>
    /// <exception cref="HttpRequestException">Unrecognized API error</exception>
    public async Task DeleteContainerAsync(int listId, int containerId)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Delete, $"lists/{listId}/containers/{containerId}");

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
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.InternalServerError:
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>());
            default:
                throw new HttpRequestException(
                    $"Response with unexpected status code returned from request to {request.RequestUri}");
        }
    }

    #endregion CONTAINERS

    #region PLACEMENTS

    /// <summary>
    /// Get all placements for the given item
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <returns>
    /// All placements
    /// </returns>
    /// <exception cref="PackedApiClientException">Recognized API error</exception>
    /// <exception cref="HttpRequestException">Unrecognized API error</exception>
    public async Task<IEnumerable<PackedPlacement>> GetPlacementsAsync(int listId, int itemId)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Get, $"lists/{listId}/items/{itemId}/placements")
        {
            Headers = { { "Accept", "application/json" } }
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
            HttpStatusCode.OK => await stream.ReadAndDeserializeFromJson<List<PackedPlacement>>(),

            // Errors documented in OpenAPI specification
            HttpStatusCode.NotFound or HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized
                or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Create a new placement
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="containerId">Container ID</param>
    /// <returns></returns>
    /// <exception cref="ItemQuantityException">Too many placements</exception>
    /// <exception cref="PackedApiClientException">Recognized API error</exception>
    /// <exception cref="HttpRequestException">Unrecognized API error</exception>
    public async Task<(PackedPlacement, string)> CreatePlacementAsync(int listId, int itemId, int containerId)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Post, $"lists/{listId}/items/{itemId}/placements")
        {
            Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        containerId
                    },
                    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }),
                Encoding.UTF8, "application/json")
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
            // API should return representation of created placement
            HttpStatusCode.Created => (await stream.ReadAndDeserializeFromJson<PackedPlacement>(),
                response.Headers.Location?.ToString() ?? string.Empty),

            HttpStatusCode.Conflict =>
                throw new ItemQuantityException($"Item with ID {itemId} has too many placements"),

            // Errors documented in OpenAPI specification
            HttpStatusCode.NotFound or HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized
                or HttpStatusCode.InternalServerError =>
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>()),

            // Errors which are not documented and that we can't get any information out of
            _ => throw new HttpRequestException(
                $"Response with unexpected status code returned from request to {request.RequestUri}")
        };
    }

    /// <summary>
    /// Delete a placement
    /// </summary>
    /// <param name="listId">List ID</param>
    /// <param name="itemId">Item ID</param>
    /// <param name="placementId">Placement ID</param>
    /// <exception cref="PackedApiClientException">Recognized API error</exception>
    /// <exception cref="HttpRequestException">Unrecognized API error</exception>
    public async Task DeletePlacementAsync(int listId, int itemId, int placementId)
    {
        // Create request message with body
        var request = new HttpRequestMessage(HttpMethod.Delete,
            $"lists/{listId}/items/{itemId}/placements/{placementId}");

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
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.Unauthorized:
            case HttpStatusCode.InternalServerError:
                throw new PackedApiClientException(await stream.ReadAndDeserializeFromJson<PackedApiError>());
            default:
                throw new HttpRequestException(
                    $"Response with unexpected status code returned from request to {request.RequestUri}");
        }
    }

    #endregion PLACEMENTS
}