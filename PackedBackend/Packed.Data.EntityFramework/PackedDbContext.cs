// Date Created: 2022/12/10
// Created by: JSW

using Microsoft.EntityFrameworkCore;
using Packed.Data.Core.Entities;
using Packed.Data.EntityFramework.EntityConfiguration;

namespace Packed.Data.EntityFramework;

/// <summary>
/// Class representing the Packed application database context
/// </summary>
public class PackedDbContext : DbContext
{
    #region CONSTRUCTOR

    /// <summary>
    /// Create a new DB context
    /// </summary>
    /// <param name="options">Options</param>
    public PackedDbContext(DbContextOptions<PackedDbContext> options)
        : base(options)
    {
    }

    #endregion CONSTRUCTOR

    #region METHODS

    /// <summary>
    /// Apply explicit model configuration
    /// </summary>
    /// <param name="modelBuilder">Model builder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply specific entity configurations
        modelBuilder.ApplyConfiguration(new ListEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ItemEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ContainerEntityConfiguration());
        modelBuilder.ApplyConfiguration(new PlacementEntityConfiguration());
    }

    #endregion METHODS

    #region PROPERTIES

    /// <summary>
    /// All lists
    /// </summary>
    public DbSet<List> Lists { get; set; } = null!;

    /// <summary>
    /// All items
    /// </summary>
    public DbSet<Item> Items { get; set; } = null!;

    /// <summary>
    /// All containers
    /// </summary>
    public DbSet<Container> Containers { get; set; } = null!;

    /// <summary>
    /// All placements
    /// </summary>
    public DbSet<Placement> Placements { get; set; } = null!;

    #endregion PROPERTIES
}