// Date Created: 2022/12/24
// Created by: JSW

using Moq;
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

    #endregion TEST METHODS
}