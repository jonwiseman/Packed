// Date Created: 2022/12/24
// Created by: JSW

using Moq;
using Packed.API.Core.DTOs;
using Packed.API.Core.Exceptions;
using Packed.API.Core.Services;
using Packed.Data.Core.Entities;
using static Packed.Test.PlacementTests.PlacementsDataServiceTestData;

namespace Packed.Test.PlacementTests;

/// <summary>
/// Test cases for <see cref="PackedPlacementsDataService"/>
/// </summary>
[TestClass]
public class PlacementsDataServiceShould : PackedTestBase
{
    #region TEST LIFE CYCLE

    /// <summary>
    /// Initialization to be performed prior to each test
    /// </summary>
    [TestInitialize]
    public override void Initialize()
    {
        base.Initialize();

        // Set up list with two items
        ListRepositoryMock
            .Setup(r =>
                r.GetListByIdAsync(ListWithTwoItems.Id))
            .ReturnsAsync(ListWithTwoItems);
    }

    #endregion TEST LIFE CYCLE

    #region TEST METHODS

    /// <summary>
    /// Test to ensure that all placements for an item are retrieved
    /// </summary>
    [DataTestMethod]
    [DynamicData(nameof(ItemData), dynamicDataDeclaringType: typeof(PlacementsDataServiceTestData))]
    public async Task GetAllPlacementsForItem(Item item)
    {
        // Arrange
        var dataService = new PackedPlacementsDataService(UnitOfWorkMock.Object);

        // Act
        var foundPlacements = await dataService.GetPlacementsForItemAsync(item.ListId, item.Id);

        // Assert
        Assert.IsNotNull(foundPlacements);
        Assert.AreEqual(item.Placements.Count, foundPlacements.Count);
        foreach (var placement in item.Placements)
        {
            var foundPlacement = foundPlacements.Single(p => p.Id == placement.Id);
            Assert.AreEqual(placement.Id, foundPlacement.Id);
            Assert.AreEqual(placement.ContainerId, foundPlacement.ContainerId);
        }
    }

    /// <summary>
    /// Test to ensure that attempting to retrieve placements for an item in
    /// a list which does not exist causes a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundOnGetAllForInvalidList()
    {
        // Arrange
        var dataService = new PackedPlacementsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.GetPlacementsForItemAsync(randomNegativeId, randomNegativeId));
    }

    /// <summary>
    /// Test to ensure that attempting to retrieve placements for an item
    /// which does not exist causes a <see cref="ItemNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseItemNotFoundExceptionOnGetAllForInvalidItem()
    {
        // Arrange
        var dataService = new PackedPlacementsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ItemNotFoundException>(async () =>
            await dataService.GetPlacementsForItemAsync(ListWithTwoItems.Id, randomNegativeId));
    }

    /// <summary>
    /// Test to ensure that a placement can be retrieved by ID
    /// </summary>
    [TestMethod]
    public async Task GetSpecificPlacement()
    {
        // Arrange
        var dataService = new PackedPlacementsDataService(UnitOfWorkMock.Object);
        var placementToFind = ListWithTwoItems.Items.First().Placements.First();
        var ids = (ListWithTwoItems.Id, ListWithTwoItems.Items.First().Id,
            placementToFind.Id);

        // Act
        var foundPlacement = await dataService.GetPlacementByIdAsync(ids.Item1, ids.Item2, ids.Item3);

        // Assert
        Assert.IsNotNull(foundPlacement);
        Assert.AreEqual(placementToFind.Id, foundPlacement.Id);
        Assert.AreEqual(placementToFind.ContainerId, foundPlacement.ContainerId);
    }

    /// <summary>
    /// Test to ensure that attempting to retrieve a specific placement for a
    /// list which does not exist causes a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundOnGetSpecificForInvalidList()
    {
        // Arrange
        var dataService = new PackedPlacementsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.GetPlacementByIdAsync(randomNegativeId, randomNegativeId, randomNegativeId));
    }

    /// <summary>
    /// Test to ensure that attempting to retrieve a specific placement for an
    /// item which does not exist causes a <see cref="ItemNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseItemNotFoundOnGetSpecificForInvalidItem()
    {
        // Arrange
        var dataService = new PackedPlacementsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ItemNotFoundException>(async () =>
            await dataService.GetPlacementByIdAsync(ListWithTwoItems.Id, randomNegativeId, randomNegativeId));
    }

    /// <summary>
    /// Test to ensure that attempting to retrieve a specific placement
    /// which does not exist causes a <see cref="PlacementNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaisePlacementNotFoundOnGetSpecificForInvalidPlacement()
    {
        // Arrange
        var dataService = new PackedPlacementsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<PlacementNotFoundException>(async () =>
            await dataService.GetPlacementByIdAsync(ListWithTwoItems.Id, ListWithTwoItems.Items.First().Id,
                randomNegativeId));
    }

    /// <summary>
    /// Test to ensure that new placements can be made
    /// </summary>
    [TestMethod]
    public async Task PlaceItems()
    {
        // Arrange
        var dataService = new PackedPlacementsDataService(UnitOfWorkMock.Object);
        var ids = (ListWithTwoItems.Id, ListWithTwoItems.Items.Last().Id,
            ListWithTwoItems.Containers.Single().Id);
        var newPlacement = new PlacementDto
        {
            ContainerId = ids.Item3
        };

        // Act
        var returnedPlacement = await dataService.PlaceItemAsync(ids.Item1, ids.Item2, newPlacement);

        // Assert
        Assert.IsNotNull(returnedPlacement);
        Assert.AreEqual(ids.Item3, newPlacement.ContainerId);
        UnitOfWorkMock
            .Verify(uow => uow.PlacementRepository.Create(It.IsAny<Placement>()));
        UnitOfWorkMock
            .Verify(uow => uow.SaveChangesAsync());
    }

    /// <summary>
    /// Test to ensure that attempting to place an item for a list which does
    /// not exist causes a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundOnPlacementForInvalidList()
    {
        // Arrange
        var dataService = new PackedPlacementsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.PlaceItemAsync(randomNegativeId, randomNegativeId, new PlacementDto
            {
                ContainerId = randomNegativeId
            }));
    }

    /// <summary>
    /// Test to ensure that attempting to place an item which does not exist
    /// causes a <see cref="ItemNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseItemNotFoundOnPlacementForInvalidList()
    {
        // Arrange
        var dataService = new PackedPlacementsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ItemNotFoundException>(async () =>
            await dataService.PlaceItemAsync(ListWithTwoItems.Id, randomNegativeId, new PlacementDto
            {
                ContainerId = randomNegativeId
            }));
    }

    /// <summary>
    /// Test to ensure that attempting to place an item into a container
    /// which does not exist causes a <see cref="ContainerNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseContainerNotFoundOnPlacementForInvalidContainer()
    {
        // Arrange
        var dataService = new PackedPlacementsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ContainerNotFoundException>(async () =>
            await dataService.PlaceItemAsync(ListWithTwoItems.Id, ListWithTwoItems.Items.First().Id, new PlacementDto
            {
                ContainerId = randomNegativeId
            }));
    }

    /// <summary>
    /// Test to ensure that attempting to place an item too many times causes
    /// a <see cref="ItemQuantityException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseItemQuantityOnPlacementWhenTooManyPlacements()
    {
        // Arrange
        var dataService = new PackedPlacementsDataService(UnitOfWorkMock.Object);

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ItemQuantityException>(async () =>
            await dataService.PlaceItemAsync(ListWithTwoItems.Id, ListWithTwoItems.Items.First().Id, new PlacementDto
            {
                ContainerId = ListWithTwoItems.Containers.Single().Id
            }));
    }

    #endregion TEST METHODS
}