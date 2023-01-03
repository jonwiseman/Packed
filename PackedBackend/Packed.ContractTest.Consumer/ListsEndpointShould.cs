// Date Created: 2022/12/27
// Created by: JSW

using System.Net;
using Packed.API.Client;
using Packed.API.Client.Responses;
using Packed.ContractTest.Shared;
using PactNet;
using PactNet.Matchers;
using static Packed.ContractTest.Shared.ContractTestData;

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

    #region TEST METHODS

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
                listId = new TypeMatcher(StandardList.Id),
                description = new TypeMatcher(StandardList.Description),
                items = new MinMaxTypeMatcher(new
                {
                    itemId = new TypeMatcher(StandardItem.Id),
                    name = new TypeMatcher(StandardItem.Name),
                    quantity = new TypeMatcher(StandardItem.Quantity),
                    placements = new MinMaxTypeMatcher(new
                    {
                        placementId = new TypeMatcher(StandardPlacement.Id),
                        containerId = new TypeMatcher(StandardPlacement.ContainerId)
                    }, 1)
                }, 1),
                containers = new MinMaxTypeMatcher(new
                {
                    containerId = new TypeMatcher(StandardContainer.Id),
                    name = new TypeMatcher(StandardContainer.Name)
                }, 1)
            }, 1));

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var lists = (await client.GetAllListsAsync()).ToList();

            // Assert
            Assert.IsNotNull(lists);
            Assert.AreEqual(1, lists.Count);

            // Ensure list deserialized correctly
            var list = lists.Single();
            Assert.AreEqual(StandardList.Id, list.Id);
            Assert.AreEqual(StandardList.Description, list.Description);

            // Ensure item deserialized correctly
            var item = list.Items.Single();
            Assert.AreEqual(StandardItem.Id, item.Id);
            Assert.AreEqual(StandardItem.Name, item.Name);
            Assert.AreEqual(StandardItem.Quantity, item.Quantity);

            // Ensure placement deserialized correctly
            var placement = item.Placements.Single();
            Assert.AreEqual(StandardPlacement.Id, placement.Id);
            Assert.AreEqual(StandardPlacement.ContainerId, placement.ContainerId);

            // Ensure container deserialized correctly
            var container = list.Containers.Single();
            Assert.AreEqual(StandardContainer.Id, container.Id);
            Assert.AreEqual(StandardContainer.Name, container.Name);
        });
    }

    /// <summary>
    /// Test to ensure that new lists can be created
    /// </summary>
    [TestMethod]
    public async Task CreateNewList()
    {
        // Arrange
        _pactBuilder
            .UponReceiving("A POST request to add a new list")
            .WithRequest(HttpMethod.Post, "/lists")
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new
            {
                description = StandardList.Description
            })
            .WillRespond()
            .WithStatus(HttpStatusCode.Created)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new
            {
                listId = new TypeMatcher(StandardList.Id),
                description = new TypeMatcher(StandardList.Description),
                items = Array.Empty<PackedItem>(),
                containers = Array.Empty<PackedContainer>()
            });

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var (list, _) = await client.CreateNewListAsync(StandardList.Description);

            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(StandardList.Id, list.Id);
            Assert.AreEqual(StandardList.Description, list.Description);
            Assert.AreEqual(0, list.Items.Count);
            Assert.AreEqual(0, list.Containers.Count);
        });
    }

    /// <summary>
    /// Test to ensure that a specific list can be retrieved
    /// </summary>
    [TestMethod]
    public async Task GetSpecificList()
    {
        // Arrange
        _pactBuilder
            .UponReceiving("A GET request for a specific list")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Get, $"/lists/{StandardList.Id}")
            .WithHeader("Accept", "application/json")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new
            {
                listId = new TypeMatcher(StandardList.Id),
                description = new TypeMatcher(StandardList.Description),
                items = new MinMaxTypeMatcher(new
                {
                    itemId = new TypeMatcher(StandardItem.Id),
                    name = new TypeMatcher(StandardItem.Name),
                    quantity = new TypeMatcher(StandardItem.Quantity),
                    placements = new MinMaxTypeMatcher(new
                    {
                        placementId = new TypeMatcher(StandardPlacement.Id),
                        containerId = new TypeMatcher(StandardPlacement.ContainerId)
                    }, 1)
                }, 1),
                containers = new MinMaxTypeMatcher(new
                {
                    containerId = new TypeMatcher(StandardContainer.Id),
                    name = new TypeMatcher(StandardContainer.Name)
                }, 1)
            });

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var list = await client.GetListByIdAsync(StandardList.Id);

            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(StandardList.Id, list.Id);
            Assert.AreEqual(StandardList.Description, list.Description);

            // Ensure item deserialized correctly
            var item = list.Items.Single();
            Assert.AreEqual(StandardItem.Id, item.Id);
            Assert.AreEqual(StandardItem.Name, item.Name);
            Assert.AreEqual(StandardItem.Quantity, item.Quantity);

            // Ensure placement deserialized correctly
            var placement = item.Placements.Single();
            Assert.AreEqual(StandardPlacement.Id, placement.Id);
            Assert.AreEqual(StandardPlacement.ContainerId, placement.ContainerId);

            // Ensure container deserialized correctly
            var container = list.Containers.Single();
            Assert.AreEqual(StandardContainer.Id, container.Id);
            Assert.AreEqual(StandardContainer.Name, container.Name);
        });
    }

    /// <summary>
    /// Test to ensure that a list can be updated
    /// </summary>
    [TestMethod]
    public async Task UpdateList()
    {
        // Arrange
        _pactBuilder
            .UponReceiving("A PUT request to update a specific list")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Put, $"/lists/{StandardList.Id}")
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new
            {
                description = $"{StandardList.Description} UPDATED"
            })
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new
            {
                listId = new TypeMatcher(StandardList.Id),
                description = new TypeMatcher($"{StandardList.Description} UPDATED"),
                items = new MinMaxTypeMatcher(new
                {
                    itemId = new TypeMatcher(StandardItem.Id),
                    name = new TypeMatcher(StandardItem.Name),
                    quantity = new TypeMatcher(StandardItem.Quantity),
                    placements = new MinMaxTypeMatcher(new
                    {
                        placementId = new TypeMatcher(StandardPlacement.Id),
                        containerId = new TypeMatcher(StandardPlacement.ContainerId)
                    }, 1)
                }, 1),
                containers = new MinMaxTypeMatcher(new
                {
                    containerId = new TypeMatcher(StandardContainer.Id),
                    name = new TypeMatcher(StandardContainer.Name)
                }, 1)
            });

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var list = await client.UpdateListAsync(StandardList.Id,
                $"{StandardList.Description} UPDATED");

            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(StandardList.Id, list.Id);
            Assert.AreEqual($"{StandardList.Description} UPDATED", list.Description);

            // Ensure item deserialized correctly
            var item = list.Items.Single();
            Assert.AreEqual(StandardItem.Id, item.Id);
            Assert.AreEqual(StandardItem.Name, item.Name);
            Assert.AreEqual(StandardItem.Quantity, item.Quantity);

            // Ensure placement deserialized correctly
            var placement = item.Placements.Single();
            Assert.AreEqual(StandardPlacement.Id, placement.Id);
            Assert.AreEqual(StandardPlacement.ContainerId, placement.ContainerId);

            // Ensure container deserialized correctly
            var container = list.Containers.Single();
            Assert.AreEqual(StandardContainer.Id, container.Id);
            Assert.AreEqual(StandardContainer.Name, container.Name);
        });
    }

    /// <summary>
    /// Test to ensure that a list can be deleted and an empty HTTP 204 is returned
    /// </summary>
    [TestMethod]
    public async Task DeleteList()
    {
        // Arrange
        _pactBuilder
            .UponReceiving("A DELETE request for a specific list")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Delete, $"/lists/{StandardList.Id}")
            .WillRespond()
            .WithStatus(HttpStatusCode.NoContent);

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            await client.DeleteListAsync(StandardList.Id);
        });
    }

    #endregion TEST METHODS
}