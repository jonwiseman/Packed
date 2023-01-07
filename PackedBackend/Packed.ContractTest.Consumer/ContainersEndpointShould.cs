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
                containerId = Match.Integer(StandardContainer.Id),
                name = Match.Type(StandardContainer.Name)
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

    /// <summary>
    /// Test to ensure that new containers can be created
    /// </summary>
    [TestMethod]
    public async Task CreateNewContainer()
    {
        // Arrange
        PactBuilder
            .UponReceiving("A POST request to create a new container")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Post, $"/lists/{StandardList.Id}/containers")
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new
            {
                name = $"{StandardContainer.Name} 2"
            })
            .WillRespond()
            .WithStatus(HttpStatusCode.Created)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new
            {
                containerId = Match.Integer(StandardContainer.Id),
                name = $"{StandardContainer.Name} 2"
            });

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var (container, _) = await client.CreateContainerAsync(StandardList.Id,
                $"{StandardContainer.Name} 2");

            // Assert
            Assert.IsNotNull(container);
            Assert.AreEqual($"{StandardContainer.Name} 2", container.Name);
        });
    }

    /// <summary>
    /// Test to ensure that a specific container can be retrieved
    /// </summary>
    [TestMethod]
    public async Task GetSpecificContainer()
    {
        // Arrange
        PactBuilder
            .UponReceiving("A GET request for a specific container")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Get, $"/lists/{StandardList.Id}/containers/{StandardContainer.Id}")
            .WithHeader("Accept", "application/json")
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new
            {
                containerId = Match.Integer(StandardContainer.Id),
                name = Match.Type(StandardContainer.Name)
            });

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var container = await client.GetContainerAsync(StandardList.Id, StandardContainer.Id);

            // Assert
            Assert.IsNotNull(container);
            Assert.AreEqual(StandardContainer.Id, container.Id);
            Assert.AreEqual(StandardContainer.Name, container.Name);
        });
    }

    /// <summary>
    /// Test to ensure that containers can be updated
    /// </summary>
    [TestMethod]
    public async Task UpdateContainer()
    {
        // Arrange
        PactBuilder
            .UponReceiving("A PUT request to update a container")
            .Given(ProviderStates.SpecificListExists)
            .WithRequest(HttpMethod.Put, $"/lists/{StandardList.Id}/containers/{StandardContainer.Id}")
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new
            {
                name = $"{StandardContainer.Name} 2"
            })
            .WillRespond()
            .WithStatus(HttpStatusCode.OK)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new
            {
                containerId = Match.Integer(StandardContainer.Id),
                name = $"{StandardContainer.Name} 2"
            });

        await PactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act
            var container = await client.UpdateContainerAsync(StandardList.Id, StandardContainer.Id,
                $"{StandardContainer.Name} 2");

            // Assert
            Assert.IsNotNull(container);
            Assert.AreEqual(StandardContainer.Id, container.Id);
            Assert.AreEqual($"{StandardContainer.Name} 2", container.Name);
        });
    }

    #endregion TEST METHODS
}