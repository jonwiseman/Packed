// Date Created: 2023/01/07
// Created by: JSW

using System.Net;
using Packed.API.Client;
using Packed.ContractTest.Shared;
using PactNet.Matchers;
using static Packed.ContractTest.Shared.ContractTestData;

namespace Packed.ContractTest.Consumer;

/// <summary>
/// Test methods for the /lists/{listId}/containers endpoint
/// </summary>
[TestClass]
public class ContainersEndpointShould : ContractTestBase
{
    #region TEST METHODS

    /// <summary>
    /// Test to ensure that all containers for a list are returned
    /// </summary>
    [TestMethod]
    public async Task ReturnAllContainers()
    {
        // Arrange
        PactBuilder
            .UponReceiving("A GET request to retrieve all containers")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Get, $"/lists/{StandardList.Id}/containers")
            .WithHeader("Accept", "application/json")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new MinMaxTypeMatcher(new
            {
                containerId = new TypeMatcher(StandardContainer.Id),
                name = new TypeMatcher(StandardContainer.Name)
            }, 1));

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var containers = (await client.GetContainersForListAsync(StandardList.Id)).ToList();

            // Assert
            Assert.IsNotNull(containers);
            Assert.AreEqual(1, containers.Count);

            // Make sure info for individual container is correct
            var container = containers.Single();
            Assert.AreEqual(StandardContainer.Id, container.Id);
            Assert.AreEqual(StandardContainer.Name, container.Name);
        });
    }

    #endregion TEST METHODS
}