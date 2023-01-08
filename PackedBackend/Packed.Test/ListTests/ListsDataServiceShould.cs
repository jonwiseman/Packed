// Date Created: 2022/12/27
// Created by: JSW

using Moq;
using Packed.API.Core.Exceptions;
using Packed.API.Core.Services;

namespace Packed.Test.ListTests;

/// <summary>
/// Test cases for <see cref="PackedListsDataService"/>
/// </summary>
[TestClass]
public class ListsDataServiceShould : PackedTestBase
{
    #region TEST LIFE CYCLE

    /// <summary>
    /// Initialize tests by setting up mocks
    /// </summary>
    [TestInitialize]
    public override void Initialize()
    {
        base.Initialize();

        // Set up list repository to return a specific list for a specific ID
        // All other IDs will return null
        ListRepositoryMock
            .Setup(r =>
                r.GetListByIdAsync(ListsDataServiceTestData.ListWhichExists.Id))
            .ReturnsAsync(ListsDataServiceTestData.ListWhichExists);
    }

    #endregion TEST LIFE CYCLE

    #region TEST METHODS

    /// <summary>
    /// Test to ensure that when the data store returns null when retrieving all lists,
    /// the data service is able to return an empty list to avoid null reference exceptions
    /// </summary>
    [TestMethod]
    public async Task ReturnEmptyListWhenListsNull()
    {
        // Arrange
        var dataService = new PackedListsDataService(UnitOfWorkMock.Object);

        // Act
        var foundLists = (await dataService.GetAllListsAsync()).ToList();

        // Assert
        Assert.IsNotNull(foundLists);
        Assert.AreEqual(0, foundLists.Count);
    }

    /// <summary>
    /// Test to ensure that retrieving a specific list which does actually exist
    /// returns an accurate representation of that list
    /// </summary>
    [TestMethod]
    public async Task ReturnSpecificList()
    {
        // Arrange
        var dataService = new PackedListsDataService(UnitOfWorkMock.Object);

        // Act
        var foundList = await dataService.GetListByIdAsync(ListsDataServiceTestData.ListWhichExists.Id);

        // Assert
        Assert.IsNotNull(foundList);
        Assert.AreEqual(ListsDataServiceTestData.ListWhichExists.Id, foundList.Id);
        Assert.AreEqual(ListsDataServiceTestData.ListWhichExists.Description, foundList.Description);
    }

    /// <summary>
    /// Test to ensure that attempting to retrieve a list which does not exist
    /// throws a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundExceptionWhenListDoesNotExist()
    {
        // Arrange
        var dataService = new PackedListsDataService(UnitOfWorkMock.Object);
        var randomListId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.GetListByIdAsync(randomListId));
    }

    #endregion TEST METHODS
}