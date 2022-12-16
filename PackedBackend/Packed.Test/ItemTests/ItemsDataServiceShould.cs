// Date Created: 2022/12/16
// Created by: JSW

using Moq;
using Packed.API.Core.Exceptions;
using Packed.API.Core.Services;
using Packed.Data.Core.Entities;

namespace Packed.Test.ItemTests;

/// <summary>
/// Test cases for ItemsDataService
/// </summary>
[TestClass]
public class ItemsDataServiceShould : PackedTestBase
{
    #region TEST LIFE CYCLE

    /// <summary>
    ///Initialization run before each test method
    /// </summary>
    [TestInitialize]
    public override void Initialize()
    {
        base.Initialize();

        ListRepositoryMock
            .Setup(r =>
                r.GetListByIdAsync(It.Is<int>(i =>
                    i == ItemsDataServiceTestData.ListWithNoItems.Id)))
            .ReturnsAsync(ItemsDataServiceTestData.ListWithNoItems);
        ListRepositoryMock
            .Setup(r =>
                r.GetListByIdAsync(It.Is<int>(i =>
                    i == ItemsDataServiceTestData.ListWithTwoItems.Id)))
            .ReturnsAsync(ItemsDataServiceTestData.ListWithTwoItems);
    }

    #endregion TEST LIFE CYCLE

    #region TEST METHODS

    /// <summary>
    /// Test to ensure that a list which exists returns the correct representation
    /// of all of its items
    /// </summary>
    /// <param name="list">List which we are testing</param>
    [DataTestMethod]
    [DynamicData(nameof(ItemsDataServiceTestData.ListData), dynamicDataDeclaringType: typeof(ItemsDataServiceTestData))]
    public async Task GetAllItemsForListWhichExists(List list)
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);

        // Act
        var foundItems = await dataService.GetItemsForListAsync(list.Id);

        // Assert
        Assert.IsNotNull(foundItems);
        Assert.AreEqual(list.Items.Count, foundItems.Count);

        // Make sure each item from the actual list matches exactly
        foreach (var item in list.Items)
        {
            // Get item out of found items
            var foundItem = foundItems
                .Single(i => item.Id == i.Id);

            // Make sure properties are equal
            Assert.AreEqual(item.Id, foundItem.Id);
            Assert.AreEqual(item.Name, foundItem.Name);
            Assert.AreEqual(item.Quantity, foundItem.Quantity);
        }
    }

    /// <summary>
    /// Test method to ensure that trying to get items for a list which
    /// does not exist throws a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundExceptionWhenListDoesNotExist()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);

        // Get a random integer ID which does not exist
        var listId = new Random().Next(int.MinValue, 0);

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.GetItemsForListAsync(listId));
    }

    #endregion TEST METHODS
}