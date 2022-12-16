// Date Created: 2022/12/16
// Created by: JSW

using Moq;
using Packed.Data.Core.Entities;
using Packed.Data.Core.Repositories;

namespace Packed.Test;

/// <summary>
/// Base class for all Packed tests
/// </summary>
public abstract class PackedTestBase
{
    #region FIELDS

    /// <summary>
    /// Mock unit of work
    /// </summary>
    protected Mock<IPackedUnitOfWork> UnitOfWorkMock = new();

    /// <summary>
    /// Mock list repository
    /// </summary>
    protected Mock<IListRepository> ListRepositoryMock = new();

    /// <summary>
    /// Mock item repository
    /// </summary>
    protected Mock<IRepositoryBase<Item>> ItemRepositoryMock = new();

    /// <summary>
    /// Mock container repository
    /// </summary>
    protected Mock<IRepositoryBase<Container>> ContainerRepositoryMock = new();

    /// <summary>
    /// Mock placement repository
    /// </summary>
    protected Mock<IRepositoryBase<Placement>> PlacementRepositoryMock = new();

    #endregion FIELDS

    #region TEST LIFE CYCLE

    /// <summary>
    /// Initialization which runs prior to each test method
    /// </summary>
    [TestInitialize]
    public virtual void Initialize()
    {
        UnitOfWorkMock
            .Setup(uow => uow.ListRepository)
            .Returns(ListRepositoryMock.Object);

        UnitOfWorkMock
            .Setup(uow => uow.ItemRepository)
            .Returns(ItemRepositoryMock.Object);

        UnitOfWorkMock
            .Setup(uow => uow.ContainerRepository)
            .Returns(ContainerRepositoryMock.Object);

        UnitOfWorkMock
            .Setup(uow => uow.PlacementRepository)
            .Returns(PlacementRepositoryMock.Object);
    }

    #endregion TEST LIFE CYCLE
}