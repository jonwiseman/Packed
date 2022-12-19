// Date Created: 2022/12/16
// Created by: JSW

using Moq;
using Packed.API.Core.DTOs;
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
    public async Task RaiseListNotFoundExceptionOnGetAllWhenListDoesNotExist()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);

        // Get a random integer ID which does not exist
        var listId = new Random().Next(int.MinValue, 0);

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.GetItemsForListAsync(listId));
    }

    /// <summary>
    /// Test to ensure that we can add new items to existing lists
    /// </summary>
    /// <param name="list">List test data</param>
    [DataTestMethod]
    [DynamicData(nameof(ItemsDataServiceTestData.ListData), dynamicDataDeclaringType: typeof(ItemsDataServiceTestData))]
    public async Task AddNewItemsToLists(List list)
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);

        // Create a new item with random quantity
        var newItem = new ItemDto
        {
            Name = "New item",
            Quantity = new Random().Next(1, int.MaxValue)
        };

        // Act
        var addedItem = await dataService.AddItemToListAsync(list.Id, newItem);

        // Assert
        Assert.IsNotNull(addedItem);
        Assert.AreEqual(addedItem.Name, newItem.Name);
        Assert.AreEqual(addedItem.Quantity, newItem.Quantity);
    }

    /// <summary>
    /// Test to ensure that trying to add items to a list which does not exist
    /// will cause a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundExceptionOnAddWhenListDoesNotExist()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);

        // Get a random integer ID which does not exist
        var listId = new Random().Next(int.MinValue, 0);

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.AddItemToListAsync(listId, new ItemDto()));
    }

    /// <summary>
    /// Test to ensure that trying to add an item to a list which already
    /// has an item with the same name causes a <see cref="DuplicateItemException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseDuplicateItemExceptionOnAddWhenItemAlreadyExists()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);

        // Get the name of an item which already exists
        var itemName = ItemsDataServiceTestData.ListWithTwoItems
            .Items
            .First()
            .Name!;

        // Act/Assert
        await Assert.ThrowsExceptionAsync<DuplicateItemException>(async () =>
            await dataService.AddItemToListAsync(
                ItemsDataServiceTestData.ListWithTwoItems.Id,
                new ItemDto
                {
                    Name = itemName
                }));
    }

    /// <summary>
    /// Test to ensure that retrieving a specific item from a list
    /// that exists returns an accurate representation of the item
    /// </summary>
    [TestMethod]
    public async Task ReturnSpecificItemWhenExists()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);
        var item = ItemsDataServiceTestData.ListWithTwoItems.Items.First()!;
        
        // Act
        var foundItem = await dataService.GetItemByIdAsync(
            ItemsDataServiceTestData.ListWithTwoItems.Id, item.Id);
        
        // Assert
        Assert.IsNotNull(foundItem);
        Assert.AreEqual(item.Id, foundItem.Id);
        Assert.AreEqual(item.Name, foundItem.Name);
        Assert.AreEqual(item.Quantity, foundItem.Quantity);
    }

    /// <summary>
    /// Test to ensure that attempting to retrieve a specific item
    /// from a list which does not exist causes a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundExceptionOnGetSpecificWithInvalidList()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().Next(int.MinValue, 0);
        
        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.GetItemByIdAsync(randomNegativeId, randomNegativeId));
    }

    /// <summary>
    /// Test to ensure that attempting to retrieve a specific item which does not
    /// exist from a list causes a <see cref="ItemNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseItemNotFoundExceptionOnGetSpecificWithInvalidItem()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().Next(int.MinValue, 0);

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ItemNotFoundException>(async () =>
            await dataService.GetItemByIdAsync(ItemsDataServiceTestData.ListWithTwoItems.Id, randomNegativeId));
    }

    // TODO: add tests for updating items
    // TODO: add tests for deleting items

    #endregion TEST METHODS
}