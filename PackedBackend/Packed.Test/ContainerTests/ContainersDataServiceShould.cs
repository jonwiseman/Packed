// Date Created: 2022/12/27
// Created by: JSW

using Moq;
using Packed.API.Core.DTOs;
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
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.GetContainersAsync(randomNegativeId));
    }

    /// <summary>
    /// Test to ensure that new containers can be added to lists
    /// </summary>
    [DataTestMethod]
    [DynamicData(nameof(ListData), dynamicDataDeclaringType: typeof(ContainersDataServiceTestData))]
    public async Task AddNewContainers(List list)
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);
        var newContainer = new ContainerDto()
        {
            Name = "NEW CONTAINER"
        };

        // Act
        var returnedContainer = await dataService.AddContainerAsync(list.Id, newContainer);

        // Assert
        Assert.IsNotNull(returnedContainer);
        Assert.AreEqual(newContainer.Name, returnedContainer.Name);
        UnitOfWorkMock
            .Verify(uow => uow.ContainerRepository.Create(It.IsAny<Container>()));
        UnitOfWorkMock
            .Verify(uow => uow.SaveChangesAsync());
    }

    /// <summary>
    /// Test to ensure that a new container can't be added to a list
    /// which does not exist
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundExceptionWhenAddingToInvalidList()
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.AddContainerAsync(randomNegativeId, new ContainerDto()));
    }

    /// <summary>
    /// Test to ensure that attempting to add a container to a list
    /// which already has a container with the same name causes a <see cref="DuplicateContainerException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseDuplicateContainerExceptionWhenAddingDuplicatedName()
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);
        var newContainer = new ContainerDto()
        {
            Name = ListWithTwoContainers.Containers.First().Name!
        };

        // Act/Assert
        await Assert.ThrowsExceptionAsync<DuplicateContainerException>(async () =>
            await dataService.AddContainerAsync(ListWithTwoContainers.Id, newContainer));
    }

    /// <summary>
    /// Test to ensure that a specific container can be retrieved from
    /// a list
    /// </summary>
    [DataTestMethod]
    [DynamicData(nameof(ContainerData), dynamicDataDeclaringType: typeof(ContainersDataServiceTestData))]
    public async Task RetrieveSpecificContainer(Container container)
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);

        // Act
        var returnedContainer = await dataService.GetContainerByIdAsync(container.ListId, container.Id);

        // Assert
        Assert.IsNotNull(returnedContainer);
        Assert.AreEqual(container.Name, returnedContainer.Name);
    }

    /// <summary>
    /// Test to ensure that trying to retrieve a container from a list which does
    /// not exist causes a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundExceptionWhenRetrievingSpecificContainerFromInvalidList()
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.GetContainerByIdAsync(randomNegativeId, randomNegativeId));
    }

    /// <summary>
    /// Test to ensure that trying to retrieve a container which does not exist
    /// causes a <see cref="ContainerNotFoundException"/>
    /// </summary>
    [DataTestMethod]
    [DynamicData(nameof(ListData), dynamicDataDeclaringType: typeof(ContainersDataServiceTestData))]
    public async Task RaiseContainerNotFoundExceptionWhenRetrievingInvalidContainer(List list)
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);
        var invalidId = list.Containers?.Any() ?? false
            ? list.Containers.Max(c => c.Id) + 1
            : 0;

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ContainerNotFoundException>(async () =>
            await dataService.GetContainerByIdAsync(list.Id, invalidId));
    }

    /// <summary>
    /// Test to ensure that existing containers can be updated
    /// </summary>
    [DataTestMethod]
    [DynamicData(nameof(ContainerData), dynamicDataDeclaringType: typeof(ContainersDataServiceTestData))]
    public async Task UpdateExistingContainer(Container container)
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);
        var containerToUpdate = new ContainerDto()
        {
            Name = container.Name + "UPDATED"
        };

        // Act
        var returnedContainer =
            await dataService.UpdateContainerAsync(container.ListId, container.Id, containerToUpdate);

        // Assert
        Assert.IsNotNull(returnedContainer);
        Assert.AreEqual(containerToUpdate.Name, returnedContainer.Name);
        UnitOfWorkMock.Verify(uow =>
            uow.ContainerRepository.Update(It.IsAny<Container>()));
        UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync());
    }

    /// <summary>
    /// Test to ensure that attempting to update a container in a list
    /// which does not exist causes a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundOnUpdateWithInvalidList()
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.UpdateContainerAsync(randomNegativeId, randomNegativeId,
                new ContainerDto()));
    }

    /// <summary>
    /// Test to ensure that attempting to update a container
    /// which doesn't exist causes a <see cref="ContainerNotFoundException"/>
    /// </summary>
    [DataTestMethod]
    [DynamicData(nameof(ListData), dynamicDataDeclaringType: typeof(ContainersDataServiceTestData))]
    public async Task RaiseContainerNotFoundOnUpdateWithInvalidContainer(List list)
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);
        var invalidContainerId = list.Containers.Any()
            ? list.Containers.Max(c => c.Id) + 1
            : 0;

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ContainerNotFoundException>(async () =>
            await dataService.UpdateContainerAsync(list.Id, invalidContainerId,
                new ContainerDto()));
    }

    /// <summary>
    /// Test to ensure that attempting to update a container to have the same
    /// name as another container in the same list causes a <see cref="DuplicateContainerException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseDuplicateContainerOnUpdateWhenDuplicatingName()
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);
        var containerToUpdate = new ContainerDto()
        {
            Name = ListWithTwoContainers.Containers.Last().Name
        };

        // Act/Assert
        await Assert.ThrowsExceptionAsync<DuplicateContainerException>(async () =>
            await dataService.UpdateContainerAsync(ListWithTwoContainers.Id,
                ListWithTwoContainers.Containers.First().Id, containerToUpdate));
    }

    /// <summary>
    /// Test to ensure that containers can be deleted
    /// </summary>
    [DataTestMethod]
    [DynamicData(nameof(ContainerData), dynamicDataDeclaringType: typeof(ContainersDataServiceTestData))]
    public async Task DeleteContainers(Container container)
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);

        // Act
        await dataService.DeleteContainerAsync(container.ListId, container.Id);

        // Assert
        UnitOfWorkMock.Verify(uow =>
            uow.ContainerRepository.Delete(It.IsAny<Container>()));
        UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync());
    }

    /// <summary>
    /// Test to ensure that attempting to delete a container from
    /// a list which does not exist causes a <see cref="ListNotFoundException"/>
    /// </summary>
    [TestMethod]
    public async Task RaiseListNotFoundOnDeleteWithInvalidList()
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);
        var randomNegativeId = new Random().GetRandomNegativeId();

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ListNotFoundException>(async () =>
            await dataService.DeleteContainerAsync(randomNegativeId, randomNegativeId));
    }

    /// <summary>
    /// Test to ensure that attempting to delete a container which does not exist
    /// causes a <see cref="ContainerNotFoundException"/>
    /// </summary>
    [DataTestMethod]
    [DynamicData(nameof(ListData), dynamicDataDeclaringType: typeof(ContainersDataServiceTestData))]
    public async Task RaiseContainerNotFoundOnDeleteWithInvalidContainer(List list)
    {
        // Arrange
        var dataService = new PackedContainersDataService(UnitOfWorkMock.Object);
        var invalidContainerId = list.Containers.Any()
            ? list.Containers.Max(c => c.Id) + 1
            : 0;

        // Act/Assert
        await Assert.ThrowsExceptionAsync<ContainerNotFoundException>(async () =>
            await dataService.DeleteContainerAsync(list.Id, invalidContainerId));
    }

    #endregion TEST METHODS
}