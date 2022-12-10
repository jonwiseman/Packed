// Date Created: 2022/12/10
// Created by: JSW

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packed.Data.Core.Entities;

namespace Packed.Data.EntityFramework.EntityConfiguration;

/// <summary>
/// Entity configuration for <see cref="Placement"/> entities
/// </summary>
public class PlacementEntityConfiguration : IEntityTypeConfiguration<Placement>
{
    /// <summary>
    /// Configure a placement entity
    /// </summary>
    /// <param name="builder">Builder</param>
    public void Configure(EntityTypeBuilder<Placement> builder)
    {
        // Each placement describes exactly one item, but each item can have multiple placements
        builder
            .HasOne(p => p.Item)
            .WithMany(i => i.Placements);

        // Each placement uses exactly one container, but a container can have multiple placements
        builder
            .HasOne(p => p.Container)
            .WithMany(c => c.Placements);
    }
}