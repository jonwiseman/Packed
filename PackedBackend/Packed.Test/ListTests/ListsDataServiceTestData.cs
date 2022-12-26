// Date Created: 2022/12/16
// Created by: JSW

using Packed.Data.Core.Entities;

namespace Packed.Test.ListTests;

/// <summary>
/// Test data for List Data Service tests
/// </summary>
public static class ListsDataServiceTestData
{
    /// <summary>
    /// A list which actually exists
    /// </summary>
    public static readonly List ListWhichExists = new()
    {
        Id = 1,
        Description = "First list",
        Items = new List<Item>(),
        Containers = new List<Container>()
    };
}