// Date Created: 2023/01/08
// Created by: JSW

using System.Net;
using Packed.API.Client;
using Packed.ContractTest.Shared;
using PactNet.Matchers;
using static Packed.ContractTest.Shared.ContractTestData;

namespace Packed.ContractTest.Consumer;

/// <summary>
/// Test cases for /placements endpoint
/// </summary>
[TestClass]
public class PlacementsEndpointShould : ContractTestBase
{
    #region TEST METHODS

    /// <summary>
    /// Test to ensure that all placements can be retrieved
    /// </summary>
    [TestMethod]
    public async Task GetAllPlacements()
    {
        // Arrange
        PactBuilder
            .UponReceiving("A GET request for all placements for an item")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Get, $"/lists/{StandardList.Id}/items/{StandardItem.Id}/placements")
            .WithHeader("Accept", "application/json")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(Match.MinType(new
            {
                placementId = Match.Integer(StandardPlacement.Id),
                containerId = Match.Integer(StandardContainer.Id)
            }, 1));

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var placements =
                (await client.GetPlacementsAsync(StandardList.Id, StandardContainer.Id)).ToList();

            // Assert
            Assert.IsNotNull(placements);
            Assert.AreEqual(1, placements.Count);

            // Ensure placement deserialized correctly
            var placement = placements.Single();
            Assert.AreEqual(StandardPlacement.Id, placement.Id);
            Assert.AreEqual(StandardPlacement.ContainerId, placement.ContainerId);
        });
    }

    #endregion TEST METHODS
}