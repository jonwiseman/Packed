// Date Created: 2023/01/04
// Created by: JSW

using System.Net;
using Packed.API.Client;
using Packed.API.Client.Responses;
using Packed.ContractTest.Shared;
using PactNet.Matchers;
using static Packed.ContractTest.Shared.ContractTestData;

namespace Packed.ContractTest.Consumer;

/// <summary>
/// Class which defines contract tests for the /lists/{listId}/items endpoint
/// </summary>
[TestClass]
public class ItemsEndpointShould : ContractTestBase
{
    #region TEST METHODS

    /// <summary>
    /// Test to ensure that all items are retrieved
    /// </summary>
    [TestMethod]
    public async Task GetAllItems()
    {
        // Arrange
        PactBuilder
            .UponReceiving("A GET request to retrieve all items for list with ID 1")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Get, $"/lists/{StandardList.Id}/items")
            .WithHeader("Accept", "application/json")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new MinMaxTypeMatcher(new
            {
                itemId = new TypeMatcher(StandardItem.Id),
                name = new TypeMatcher(StandardItem.Name),
                quantity = new TypeMatcher(StandardItem.Quantity),
                placements = new MinMaxTypeMatcher(new
                {
                    placementId = new TypeMatcher(StandardPlacement.Id),
                    containerId = new TypeMatcher(StandardPlacement.ContainerId)
                }, 1)
            }, 1));

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var items = (await client.GetItemsForListAsync(StandardList.Id)).ToList();

            // Assert
            Assert.IsNotNull(items);
            Assert.AreEqual(1, items.Count);

            // Ensure item deserialized correctly
            var item = items.Single();
            Assert.AreEqual(StandardItem.Id, item.Id);
            Assert.AreEqual(StandardItem.Name, item.Name);
            Assert.AreEqual(StandardItem.Quantity, item.Quantity);

            // Ensure placement deserialized correctly
            var placement = item.Placements.Single();
            Assert.AreEqual(StandardPlacement.Id, placement.Id);
            Assert.AreEqual(StandardPlacement.ContainerId, placement.ContainerId);
        });
    }

    /// <summary>
    /// Test to ensure that a new item can be created
    /// </summary>
    [TestMethod]
    public async Task CreateNewItem()
    {
        // Arrange
        PactBuilder
            .UponReceiving("A POST request to add a new item")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Post, $"/lists/{StandardList.Id}/items")
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new
            {
                name = $"{StandardItem.Name} 2",
                quantity = StandardItem.Quantity
            })
            .WillRespond()
            .WithStatus(HttpStatusCode.Created)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new
            {
                itemId = Match.Integer(StandardItem.Id),
                name = $"{StandardItem.Name} 2",
                quantity = StandardItem.Quantity,
                placements = Array.Empty<PackedPlacement>()
            });

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var (item, _) = await client.CreateItemForListAsync(StandardList.Id, $"{StandardItem.Name} 2",
                StandardItem.Quantity);

            // Assert
            Assert.IsNotNull(item);
            Assert.AreEqual(StandardItem.Id, item.Id);
            Assert.AreEqual($"{StandardItem.Name} 2", item.Name);
            Assert.AreEqual(StandardItem.Quantity, item.Quantity);
        });
    }

    /// <summary>
    /// Test to ensure that a specific item can be retrieved
    /// </summary>
    [TestMethod]
    public async Task GetSpecificItem()
    {
        // Arrange
        PactBuilder
            .UponReceiving(
                $"A GET request to retrieve item with ID {StandardItem.Id} from list with ID {StandardList.Id}")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Get, $"/lists/{StandardList.Id}/items/{StandardItem.Id}")
            .WithHeader("Accept", "application/json")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new
            {
                itemId = new TypeMatcher(StandardItem.Id),
                name = new TypeMatcher(StandardItem.Name),
                quantity = new TypeMatcher(StandardItem.Quantity),
                placements = new MinMaxTypeMatcher(new
                {
                    placementId = new TypeMatcher(StandardPlacement.Id),
                    containerId = new TypeMatcher(StandardPlacement.ContainerId)
                }, 1)
            });

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var item = await client.GetItemFromListAsync(StandardList.Id, StandardItem.Id);

            // Assert
            Assert.AreEqual(StandardItem.Id, item.Id);
            Assert.AreEqual(StandardItem.Name, item.Name);
            Assert.AreEqual(StandardItem.Quantity, item.Quantity);

            // Ensure placement deserialized correctly
            var placement = item.Placements.Single();
            Assert.AreEqual(StandardPlacement.Id, placement.Id);
            Assert.AreEqual(StandardPlacement.ContainerId, placement.ContainerId);
        });
    }

    /// <summary>
    /// Test to ensure that an item can be updated
    /// </summary>
    [TestMethod]
    public async Task UpdateItem()
    {
        // Arrange
        PactBuilder
            .UponReceiving("A PUT request to update an item")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Put, $"/lists/{StandardList.Id}/items/{StandardItem.Id}")
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new
            {
                name = $"{StandardItem.Name} 2",
                quantity = StandardItem.Quantity + 1
            })
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new
            {
                itemId = Match.Integer(StandardItem.Id),
                name = $"{StandardItem.Name} 2",
                quantity = StandardItem.Quantity + 1,
                placements = new MinMaxTypeMatcher(new
                {
                    placementId = new TypeMatcher(StandardPlacement.Id),
                    containerId = new TypeMatcher(StandardPlacement.ContainerId)
                }, 1)
            });

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var updatedItem = await client.UpdateItemAsync(StandardList.Id, StandardItem.Id, $"{StandardItem.Name} 2",
                StandardItem.Quantity + 1);

            // Assert
            Assert.IsNotNull(updatedItem);
            Assert.AreEqual(StandardItem.Id, updatedItem.Id);
            Assert.AreEqual($"{StandardItem.Name} 2", updatedItem.Name);
            Assert.AreEqual(StandardItem.Quantity + 1, updatedItem.Quantity);

            // Ensure placement deserialized correctly
            var placement = updatedItem.Placements.Single();
            Assert.AreEqual(StandardPlacement.Id, placement.Id);
            Assert.AreEqual(StandardPlacement.ContainerId, placement.ContainerId);
        });
    }

    /// <summary>
    /// Test to ensure an item can be deleted
    /// </summary>
    [TestMethod]
    public async Task DeleteItem()
    {
        // Arrange
        PactBuilder
            .UponReceiving("A DELETE request for a specific item")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Delete, $"/lists/{StandardList.Id}/items/{StandardItem.Id}")
            .WillRespond()
            .WithStatus(HttpStatusCode.NoContent);

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            await client.DeleteItemAsync(StandardList.Id, StandardItem.Id);
        });
    }

    #endregion TEST METHODS
}