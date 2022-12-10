// Date Created: 2022/12/10
// Created by: JSW

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packed.Data.Core.Entities;

namespace Packed.Data.EntityFramework.EntityConfiguration;

/// <summary>
/// Entity configuration for <see cref="List"/> entities
/// </summary>
public class ListEntityConfiguration : IEntityTypeConfiguration<List>
{
    /// <summary>
    /// Configure a list entity
    /// </summary>
    /// <param name="builder">Builder</param>
    public void Configure(EntityTypeBuilder<List> builder)
    {
        // Each list has many items, but an item belongs to only one list
        builder
            .HasMany(l => l.Items)
            .WithOne(i => i.List);

        // Each list has many containers, but each container belongs to only one list
        builder
            .HasMany(l => l.Containers)
            .WithOne(c => c.List);
    }
}