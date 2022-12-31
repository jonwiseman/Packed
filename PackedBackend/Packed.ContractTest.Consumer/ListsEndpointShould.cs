// Date Created: 2022/12/27
// Created by: JSW

using System.Net;
using Packed.API.Client;
using Packed.API.Client.Responses;
using Packed.API.Core.Exceptions;
using Packed.ContractTest.Shared;
using PactNet;
using PactNet.Matchers;
using static Packed.ContractTest.Shared.ContractData;

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

    #endregion GET LISTS

    #region ADD LIST

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
                description = new TypeMatcher(StandardList.Description)
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

    // TODO: add test for bad request

    /// <summary>
    /// Test to ensure that attempting to create a duplicate list will return an HTTP 409 Conflict
    /// </summary>
    [TestMethod]
    public async Task ReturnConflictWhenCreatingDuplicateList()
    {
        // Arrange
        _pactBuilder
            .UponReceiving("A POST request to add a new list")
            .Given(ProviderStates.DuplicateList)
            .WithRequest(HttpMethod.Post, "/lists")
            .WithHeader("Content-Type", "application/json; charset=utf-8")
            .WithJsonBody(new
            {
                // Intentionally not using a type matcher here so that this value is always sent
                description = StandardList.Description
            })
            .WillRespond()
            .WithStatus(HttpStatusCode.Conflict)
            .WithHeader("Content-Type", "application/json")
            .WithJsonBody(new
            {
                type = Match.Type("errors/Conflict"),
                title = Match.Type("Conflict"),
                status = Match.Type((int)HttpStatusCode.Conflict),
                detail = Match.Type("A list with the same description already exists"),
                instance = Match.Type(new Uri("https://packed.api/lists")),
                errorId = Match.Type(ErrorGuid),
                timestamp = Match.Type(ErrorTime)
            });

        await _pactBuilder.VerifyAsync(async ctx =>
        {
            var httpClient = HttpClientFactory.Create();
            httpClient.BaseAddress = ctx.MockServerUri;
            var client = new PackedApiClient(httpClient);

            // Act/Assert
            await Assert.ThrowsExceptionAsync<DuplicateListException>(async () =>
                await client.CreateNewListAsync(StandardList.Description));
        });
    }

    #endregion ADD LIST
}