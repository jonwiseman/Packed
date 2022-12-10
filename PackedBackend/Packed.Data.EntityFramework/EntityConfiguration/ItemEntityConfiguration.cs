// Date Created: 2022/12/10
// Created by: JSW

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packed.Data.Core.Entities;

namespace Packed.Data.EntityFramework.EntityConfiguration;

/// <summary>
/// Entity configuration for <see cref="Item"/> entities
/// </summary>
public class ItemEntityConfiguration : IEntityTypeConfiguration<Item>
{
    /// <summary>
    /// Configure an item entity
    /// </summary>
    /// <param name="builder">Builder</param>
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        // Each item belongs to exactly one list, but a list can have several items
        builder
            .HasOne(i => i.List)
            .WithMany(l => l.Items);

        // Each item can have many placements, but each placement describes only one item
        builder
            .HasMany(i => i.Placements)
            .WithOne(p => p.Item);
    }
}