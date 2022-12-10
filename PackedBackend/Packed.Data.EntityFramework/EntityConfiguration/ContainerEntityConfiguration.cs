// Date Created: 2022/12/10
// Created by: JSW

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Packed.Data.Core.Entities;

namespace Packed.Data.EntityFramework.EntityConfiguration;

/// <summary>
/// Entity configuration for container entities
/// </summary>
public class ContainerEntityConfiguration : IEntityTypeConfiguration<Container>
{
    /// <summary>
    /// Configure a container entity
    /// </summary>
    /// <param name="builder">Builder</param>
    public void Configure(EntityTypeBuilder<Container> builder)
    {
        // A container is in exactly one list, but a list can have many containers
        builder
            .HasOne(c => c.List)
            .WithMany(l => l.Containers);

        // A container can have many placements, but each placement describes
        // exactly one container
        builder
            .HasMany(c => c.Placements)
            .WithOne(p => p.Container);
    }
}