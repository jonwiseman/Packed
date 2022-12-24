// Date Created: 2022/12/24
// Created by: JSW

using Packed.Data.Core.Entities;

namespace Packed.Test.PlacementTests;

/// <summary>
/// Test data for PlacementsDataService tests
/// </summary>
public static class PlacementsDataServiceTestData
{
    #region CONSTANT TEST DATA

    /// <summary>
    /// A list which exists and has two items and one container.
    /// The first item has one placement, while the second item has no placements.
    /// </summary>
    public static readonly List ListWithTwoItems = new()
    {
        Id = 2,
        Description = "List with two items",
        Items = new List<Item>()
        {
            new()
            {
                Id = 1,
                ListId = 2,
                Name = "First Item",
                Quantity = 1,
                Placements = new List<Placement>()
                {
                    new()
                    {
                        Id = 1,
                        ItemId = 1,
                        ContainerId = 1
                    }
                }
            },
            new()
            {
                Id = 2,
                ListId = 2,
                Name = "Second Item",
                Quantity = 2,
                Placements = new List<Placement>()
            }
        },
        Containers = new List<Container>()
        {
            new()
            {
                Id = 1,
                ListId = 2,
                Name = "First Container",
                Placements = new List<Placement>()
                {
                    new()
                    {
                        Id = 1,
                        ItemId = 1,
                        ContainerId = 1
                    }
                }
            }
        }
    };

    #endregion CONSTANT TEST DATA

    #region DYNAMIC TEST DATA

    /// <summary>
    /// Items to use in tests
    /// </summary>
    public static IEnumerable<object[]> ItemData
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    ListWithTwoItems.Items.First()
                },
                new object[]
                {
                    ListWithTwoItems.Items.Last()
                }
            };
        }
    }

    /// <summary>
    /// All placements
    /// </summary>
    public static IEnumerable<object[]> PlacementData
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    ListWithTwoItems.Items.First().Placements.Single()
                }
            };
        }
    }

    #endregion DYNAMIC TEST DATA
}