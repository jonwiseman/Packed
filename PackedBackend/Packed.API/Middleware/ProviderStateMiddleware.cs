// Date Created: 2023/01/03
// Created by: JSW

using System.Net;
using Moq;
using Packed.API.Extensions;
using Packed.ContractTest.Shared;
using Packed.Data.Core.Entities;
using Packed.Data.Core.Repositories;

namespace Packed.API.Middleware;

/// <summary>
/// Terminal middleware which handles provider states
/// </summary>
public class ProviderStateMiddleware
{
    #region FIELDS

    /// <summary>
    /// A dictionary which maps provider states to corresponding actions to set up those provider states
    /// </summary>
    private readonly IDictionary<string, Action<IDictionary<string, string>, Mock<IListRepository>>>
        _providerStateActions = new Dictionary<string, Action<IDictionary<string, string>, Mock<IListRepository>>>()
        {
            {
                ProviderStates.SpecificListExists.ToLower(),
                EnsureSpecificListExists
            }
        };

    /// <summary>
    /// The next middleware in the pipeline
    /// </summary>
    private readonly RequestDelegate _next;

    #endregion FIELDS

    #region CONSTRUCTOR

    /// <summary>
    /// Create and initialize middleware
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    public ProviderStateMiddleware(RequestDelegate next)
    {
        // Set next middleware so we can invoke it in the case where request
        // is not for provider state setup
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    #endregion CONSTRUCTOR

    #region METHODS

    /// <summary>
    /// Invoke the middleware
    /// </summary>
    /// <param name="context">Context</param>
    /// <param name="listRepositoryMock">List repository mock</param>
    /// <exception cref="InvalidOperationException">Empty provider state</exception>
    /// <exception cref="ArgumentOutOfRangeException">Unrecognized or unsupported provider state</exception>
    public async Task InvokeAsync(HttpContext context, Mock<IListRepository> listRepositoryMock)
    {
        if (!(context.Request.Path.Value?.StartsWith("/provider-states") ?? false))
        {
            await _next.Invoke(context);
            return;
        }

        if (context.Request.Method.Equals(HttpMethod.Post.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            // Deserialize provider state from request body
            var providerState = await context.Request.Body.ReadAndDeserializeFromJson<ProviderState>();

            // If no provider state provided, throw an exception
            if (string.IsNullOrWhiteSpace(providerState.State))
            {
                throw new InvalidOperationException("Provider state not provided");
            }

            // If we don't know how to set up the supplied provider state, throw an exception
            if (!_providerStateActions.TryGetValue(providerState.State.ToLower(), out var setupAction))
            {
                throw new ArgumentOutOfRangeException(providerState.State,
                    "Unsupported or unrecognized provider state");
            }

            // If we do know how to set up the supplied state, do so
            setupAction.Invoke(providerState.Params, listRepositoryMock);

            // Write back an HTTP 200 OK
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            await context.Response.WriteAsync("Completed setup");
        }
    }

    #endregion METHODS

    #region PROVIDER STATE SETUPS

    /// <summary>
    /// Handle states that require there to be one list which exists
    /// </summary>
    /// <param name="parameters">Parameters</param>
    /// <param name="listRepositoryMock">List repository mock</param>
    private static void EnsureOneListExists(IDictionary<string, string> parameters,
        Mock<IListRepository> listRepositoryMock)
    {
        listRepositoryMock
            .Setup(r => r.GetAllListsAsync())
            .ReturnsAsync(new List<List>
            {
                ContractTestData.StandardList
            });
    }

    /// <summary>
    /// Handle state that require there to be one specific list which exists
    /// </summary>
    /// <param name="parameters">Parameters</param>
    /// <param name="listRepositoryMock">List repository mock</param>
    private static void EnsureSpecificListExists(IDictionary<string, string> parameters,
        Mock<IListRepository> listRepositoryMock)
    {
        listRepositoryMock
            .Setup(r => r.GetAllListsAsync())
            .ReturnsAsync(new List<List>
            {
                ContractTestData.StandardList
            });

        listRepositoryMock
            .Setup(r => r.GetListByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(ContractTestData.StandardList);
    }

    #endregion PROVIDER STATE SETUPS
}