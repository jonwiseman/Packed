// Date Created: 2022/12/19
// Created by: JSW

using Moq;
using Packed.API.Core.Exceptions;
using Packed.API.Core.Services;
using Packed.Data.Core.Entities;
using static Packed.Test.ContainerTests.ContainersDataServiceTestData;

namespace Packed.Test.ContainerTests;

/// <summary>
/// Test cases for <see cref="PackedContainersDataService"/>
/// </summary>
[TestClass]
public class ContainersDataServiceShould : PackedTestBase
{
    #region TEST LIFE CYCLE

    /// <summary>
    ///Initialization run before each test method
    /// </summary>
    [TestInitialize]
    public override void Initialize()
    {
        base.Initialize();

        // Set up list with no containers
        ListRepositoryMock
            .Setup(r =>
                r.GetListByIdAsync(ListWithNoContainers.Id))
            .ReturnsAsync(ListWithNoContainers);

        // Set up list with containers
        ListRepositoryMock
            .Setup(r =>
                r.GetListByIdAsync(ListWithTwoContainers.Id))
            .ReturnsAsync(ListWithTwoContainers);
    }

    #endregion TEST LIFE CYCLE

    #region TEST METHODS

    /// <summary>
    /// Test to ensure that all containers are retrieved correctly from lists which exist
    /// </summary>
    [DataTestMethod]
    [DynamicData(nameof(ListData), dynamicDataDeclaringType: typeof(ContainersDataServiceTestData))]
    public async Task RetrieveAllContainers(List list)
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);

        // Act
        var returnedContainers = await dataService.GetContainersAsync(list.Id);

        // Assert
        Assert.IsNotNull(returnedContainers);
        Assert.AreEqual(list.Containers.Count, returnedContainers.Count);

        foreach (var container in returnedContainers)
        {
            var foundContainer = list.Containers.Single(c => c.Id == container.Id);
            Assert.AreEqual(foundContainer.Name, container.Name);
        }
    }

    /// <summary>
    /// Test to ensure that attempting to retrieve containers for a list which does not
    /// exist will cause a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundExceptionWhenListDoesNotExist()
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().Next(int.MinValue, 0);

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.GetContainersAsync(randomNegativeId));
    }

    #endregion TEST METHODS
}