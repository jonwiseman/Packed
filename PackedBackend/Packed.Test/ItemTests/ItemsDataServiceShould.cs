// Date Created: 2022/12/27
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
        var listId = new Random().GetRandomNegativeId();

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
        var listId = new Random().GetRandomNegativeId();

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
    /// Test to ensure that names are unique only inside a single list.
    /// In other words, a name can be re-used in multiple different lists
    /// </summary>
    [TestMethod]
    public async Task AllowTwoOfSameNameInTwoDifferentLists()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);

        // Get the name of an item which already exists
        var itemName = ItemsDataServiceTestData.ListWithTwoItems
            .Items
            .First()
            .Name!;

        // Create a new item with random quantity
        var newItem = new ItemDto
        {
            Name = itemName,
            Quantity = new Random().Next(1, int.MaxValue)
        };

        // Act
        var returnedItem = await dataService.AddItemToListAsync(
            ItemsDataServiceTestData.ListWithNoItems.Id,
            newItem);

        // Assert
        Assert.IsNotNull(returnedItem);
        Assert.AreEqual(itemName, returnedItem.Name);
        Assert.AreEqual(newItem.Quantity, returnedItem.Quantity);
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
        var randomNegativeId = new Random().GetRandomNegativeId();

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
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ItemNotFoundException>(async () =>
            await dataService.GetItemByIdAsync(ItemsDataServiceTestData.ListWithTwoItems.Id, randomNegativeId));
    }

    /// <summary>
    /// Test to ensure that an existing item can be updated
    /// </summary>
    [TestMethod]
    public async Task UpdateExistingItem()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);

        // The item we'll update
        var itemToUpdate = ItemsDataServiceTestData.ListWithTwoItems.Items.First()!;

        // Create input DTO and change some properties
        var updatedItem = new ItemDto(itemToUpdate)
        {
            Name = itemToUpdate.Name + "UPDATED",
            Quantity = itemToUpdate.Quantity + 1
        };

        // Act
        var returnedItem = await dataService.UpdateItemAsync(ItemsDataServiceTestData.ListWithTwoItems.Id,
            itemToUpdate.Id, updatedItem);

        // Assert
        Assert.IsNotNull(returnedItem);
        Assert.AreEqual(itemToUpdate.Id, returnedItem.Id);
        Assert.AreEqual(updatedItem.Name, returnedItem.Name);
        Assert.AreEqual(updatedItem.Quantity, returnedItem.Quantity);
    }

    /// <summary>
    /// Test to ensure that only updating an item's quantity doesn't cause a <see cref="DuplicateItemException"/>
    /// </summary>
    [TestMethod]
    public async Task UpdateItemWhenUpdatingOnlyQuantity()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);

        // The item we'll update
        var itemToUpdate = ItemsDataServiceTestData.ListWithTwoItems.Items.First()!;

        // Create input DTO and change only the quantity
        var updatedItem = new ItemDto(itemToUpdate)
        {
            Quantity = itemToUpdate.Quantity + 1
        };

        // Act
        var returnedItem = await dataService.UpdateItemAsync(ItemsDataServiceTestData.ListWithTwoItems.Id,
            itemToUpdate.Id, updatedItem);

        // Assert
        Assert.IsNotNull(returnedItem);
        Assert.AreEqual(itemToUpdate.Id, returnedItem.Id);
        Assert.AreEqual(updatedItem.Name, returnedItem.Name);
        Assert.AreEqual(updatedItem.Quantity, returnedItem.Quantity);
    }

    /// <summary>
    /// Test to ensure that attempting to set an item's quantity to any value lower than the
    /// current count of placements causes a <see cref="ItemQuantityException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseItemQuantityExceptionWhenReducingQuantityBelowPlacementCount()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);

        // The item we'll update
        var itemToUpdate = ItemsDataServiceTestData.ListWithTwoItems.Items.First()!;

        // Create input DTO and change only the quantity
        var updatedItem = new ItemDto(itemToUpdate)
        {
            Quantity = (itemToUpdate.Placements?.Count ?? 0) - 1
        };

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ItemQuantityException>(async () =>
            await dataService.UpdateItemAsync(ItemsDataServiceTestData.ListWithTwoItems.Id,
                itemToUpdate.Id, updatedItem));
    }

    /// <summary>
    /// Test to ensure that attempting to update an item in a list
    /// which does not exist causes a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundExceptionWhenListNotFound()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.UpdateItemAsync(randomNegativeId,
                randomNegativeId, new ItemDto()));
    }

    /// <summary>
    /// Test to ensure that attempting to update an item which doesn't exist
    /// causes a <see cref="ItemNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseItemNotFoundExceptionWhenItemNotFound()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ItemNotFoundException>(async () =>
            await dataService.UpdateItemAsync(ItemsDataServiceTestData.ListWithTwoItems.Id,
                randomNegativeId, new ItemDto()));
    }

    /// <summary>
    /// Test to ensure that attempting to update an item to use the name of another item
    /// in the same list causes a <see cref="DuplicateItemException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseDuplicateItemExceptionWhenDuplicatingItemName()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);

        // The item we'll update
        var itemToUpdate = ItemsDataServiceTestData.ListWithTwoItems.Items.First()!;
        var nameToDuplicate = ItemsDataServiceTestData.ListWithTwoItems.Items.Last()!.Name;

        // Create input DTO and change some properties
        var updatedItem = new ItemDto(itemToUpdate)
        {
            Name = nameToDuplicate,
            Quantity = itemToUpdate.Quantity + 1
        };

        await Assert.ThrowsExceptionAsync<DuplicateItemException>(async () =>
            await dataService.UpdateItemAsync(itemToUpdate.ListId, itemToUpdate.Id, updatedItem));
    }

    /// <summary>
    /// Test to ensure that items which exist get deleted
    /// </summary>
    [TestMethod]
    public async Task DeleteExistingItems()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);
        var itemToDelete = ItemsDataServiceTestData.ListWithTwoItems.Items.First()!.Id;

        // Act
        await dataService.DeleteItemAsync(ItemsDataServiceTestData.ListWithTwoItems.Id, itemToDelete);

        // Assert
        UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync());
        UnitOfWorkMock.Verify(uow => uow.ItemRepository.Delete(It.IsAny<Item>()));
    }

    /// <summary>
    /// Test to ensure that trying to delete an item from a list
    /// which does not exists causes a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundExceptionWhenDeletingFromInvalidList()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.DeleteItemAsync(randomNegativeId, randomNegativeId));
    }

    /// <summary>
    /// Test to ensure that trying to delete an item which does not exist
    /// causes a <see cref="ItemNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseItemNotFoundExceptionWhenDeletingInvalidItem()
    {
        // Arrange
        var dataService = new PackedItemsDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        await Assert.ThrowsExceptionAsync<ItemNotFoundException>(async () =>
            await dataService.DeleteItemAsync(ItemsDataServiceTestData.ListWithTwoItems.Id, randomNegativeId));
    }

    #endregion TEST METHODS
}