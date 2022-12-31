// Date Created: 2022/12/30
// Created by: JSW

using System;
using System.Collections.Generic;
using Packed.Data.Core.Entities;

namespace Packed.ContractTest.Shared
{
    /// <summary>
    /// Shared data for contract tests
    /// </summary>
    public static class ContractTestData
    {
        #region MODEL DATA

        /// <summary>
        /// Placement for use in contract tests
        /// </summary>
        public static readonly Placement StandardPlacement = new Placement()
        {
            Id = 1,
            ItemId = 1,
            ContainerId = 1
        };

        /// <summary>
        /// Standard item to use in contract tests
        /// </summary>
        public static readonly Item StandardItem = new Item()
        {
            Id = 1,
            ListId = 1,
            Name = "First item",
            Quantity = 1,
            Placements = new List<Placement>
            {
                StandardPlacement
            }
        };

        /// <summary>
        /// Container for use in contract tests
        /// </summary>
        public static readonly Container StandardContainer = new Container()
        {
            Id = 1,
            ListId = 1,
            Name = "First container",
            Placements = new List<Placement>
            {
                new Placement()
                {
                    Id = 1,
                    ItemId = 1,
                    ContainerId = 1
                }
            }
        };

        /// <summary>
        /// Standard list to be used in contract tests
        /// </summary>
        public static readonly List StandardList = new List()
        {
            Id = 1,
            Description = "First list",
            Items = new List<Item>
            {
                StandardItem
            },
            Containers = new List<Container>
            {
                StandardContainer
            }
        };

        #endregion MODEL DATA

        #region ERRORS

        /// <summary>
        /// GUID to use when generating pacts
        /// </summary>
        /// <remarks>
        /// Declared as a constant to avoid the appearance of tests changing despite there
        /// being no actual changes
        /// </remarks>
        public static readonly Guid ErrorGuid = Guid.Parse("e456e134-0ac8-427e-9e1f-fe573af0e681");

        /// <summary>
        /// Time at which an error occurred
        /// </summary>
        public static readonly string ErrorTime = "2022-12-31T18:05:10.3047738Z";

        #endregion ERRORS
    }
}