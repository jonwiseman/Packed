// Date Created: 2022/12/27
// Created by: JSW

using System.Net;
using Packed.API.Client;
using Packed.ContractTest.Shared;
using PactNet;
using PactNet.Matchers;

namespace Packed.ContractTest.Consumer;

/// <summary>
/// Class which defines contract tests for the /lists endpoint
/// </summary>
[TestClass]
public class ListsEndpointShould
{
    #region FIELDS

    /// <summary>
    /// Pact builder
    /// </summary>
    private IPactBuilderV3 _pactBuilder = null!;

    #endregion FIELDS

    #region TEST LIFE CYCLE

    /// <summary>
    /// Initialize Pact Builder
    /// </summary>
    [TestInitialize]
    public void Initialize()
    {
        var pact = Pact.V3(ContractInfo.ConsumerName, ContractInfo.ProviderName, new PactConfig
        {
            PactDir = $"{Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}pacts"
        });

        _pactBuilder = pact.WithHttpInteractions();
    }

    #endregion TEST LIFE CYCLE

    #region GET LISTS

    /// <summary>
    /// Test to ensure that all lists can be retrieved
    /// </summary>
    [TestMethod]
    public async Task RetrieveAllLists()
    {
        // Arrange
        _pactBuilder
            .UponReceiving("A GET request to retrieve all lists")
            .Given(ProviderStates.ListExists)
            .WithRequest(HttpMethod.Get, "/lists")
            .WithHeader("Accept", "application/json")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new MinMaxTypeMatcher(new
            {
                listId = new TypeMatcher(1),
                description = new TypeMatcher("First list"),
                items = new MinMaxTypeMatcher(new
                {
                    itemId = new TypeMatcher(1),
                    name = new TypeMatcher("First item"),
                    quantity = new TypeMatcher(1),
                    placements = new MinMaxTypeMatcher(new
                    {
                        placementId = new TypeMatcher(1),
                        containerId = new TypeMatcher(1)
                    }, 1)
                }, 1),
                containers = new MinMaxTypeMatcher(new
                {
                    containerId = new TypeMatcher(1),
                    name = new TypeMatcher("First container")
                }, 1)
            }, 1));

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            var lists = await client.GetAllListsAsync();
            Assert.IsNotNull(lists);
            Assert.AreNotEqual(0, lists.Count());
        });
    }

    #endregion GET LISTS
}