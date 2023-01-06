// Date Created: 2023/01/04
// Created by: JSW

using System.Net;
using Packed.API.Client;
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

    #endregion TEST METHODS
}