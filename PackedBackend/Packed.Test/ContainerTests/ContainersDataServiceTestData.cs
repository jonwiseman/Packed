// Date Created: 2022/12/19
// Created by: JSW

using Packed.Data.Core.Entities;

namespace Packed.Test.ContainerTests;

/// <summary>
/// Test data for Containers Data Service tests
/// </summary>
public static class ContainersDataServiceTestData
{
    #region CONSTANT TEST DATA

    /// <summary>
    /// A list which exists but has no containers
    /// </summary>
    public static readonly List ListWithNoContainers = new()
    {
        Id = 1,
        Description = "List with no containers",
        Containers = new List<Container>()
    };

    /// <summary>
    /// A list which exists and has two containers
    /// </summary>
    public static readonly List ListWithTwoContainers = new()
    {
        Id = 2,
        Description = "List with two containers",
        Containers = new List<Container>()
        {
            new()
            {
                Id = 1,
                ListId = 2,
                Name = "First Container",
                Placements = new List<Placement>()
            },
            new()
            {
                Id = 2,
                ListId = 2,
                Name = "Second Container",
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
                    ListWithNoContainers
                },
                new object[]
                {
                    ListWithTwoContainers
                }
            };
        }
    }

    #endregion DYNAMIC TEST DATA
}