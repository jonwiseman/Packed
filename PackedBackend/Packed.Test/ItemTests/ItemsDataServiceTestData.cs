// Date Created: 2022/12/27
// Created by: JSW

using Packed.Data.Core.Entities;

namespace Packed.Test.ItemTests;

/// <summary>
/// Test data for tests relating to Items Data Service
/// </summary>
public static class ItemsDataServiceTestData
{
    #region CONSTANT TEST DATA

    /// <summary>
    /// A list which exists but has no items
    /// </summary>
    public static readonly List ListWithNoItems = new()
    {
        Id = 1,
        Description = "List with no items",
        Items = new List<Item>(),
        Containers = new List<Container>()
    };

    /// <summary>
    /// A list which exists and has two items
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
            },
            new()
            {
                Id = 2,
                ListId = 2,
                Name = "Second Item",
                Quantity = 2,
                Placements = new List<Placement>()
            }
        }
    };

    #endregion CONSTANT TEST DATA

    #region DYNAMIC TEST DATA

    /// <summary>
    /// Test data for lists
    /// </summary>
    public static IEnumerable<object[]> ListData
    {
        get
        {
            return new[]
            {
                new object[]
                {
                    ListWithNoItems
                },
                new object[]
                {
                    ListWithTwoItems
                }
            };
        }
    }

    #endregion DYNAMIC TEST DATA
}