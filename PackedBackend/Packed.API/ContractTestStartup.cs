// Date Created: 2022/12/28
// Created by: JSW

using Microsoft.AspNetCore.Mvc;
using Moq;
using Packed.API.Core.Services;
using Packed.API.Factories;
using Packed.API.Middleware;
using Packed.Data.Core.Entities;
using Packed.Data.Core.Repositories;

namespace Packed.API;

/// <summary>
/// Startup to be used when running contract tests
/// </summary>
public class ContractTestStartup
{
    /// <summary>
    /// Configure DI service container
    /// </summary>
    /// <param name="services">Service collection</param>
    public void ConfigureServices(IServiceCollection services)
    {
        // Add controllers
        services.AddControllers();

        // Suppress the default model state invalid filter. We'll use our own instead
        services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });

        // Create mocks
        var unitOfWorkMock = new Mock<IPackedUnitOfWork>();
        var listRepositoryMock = new Mock<IListRepository>();
        var itemRepositoryMock = new Mock<IRepositoryBase<Item>>();
        var containerRepositoryMock = new Mock<IRepositoryBase<Container>>();
        var placementRepositoryMock = new Mock<IRepositoryBase<Placement>>();

        // Hook up mock repositories to units of work
        unitOfWorkMock
            .Setup(uow => uow.ListRepository)
            .Returns(listRepositoryMock.Object);
        unitOfWorkMock
            .Setup(uow => uow.ItemRepository)
            .Returns(itemRepositoryMock.Object);
        unitOfWorkMock
            .Setup(uow => uow.ContainerRepository)
            .Returns(containerRepositoryMock.Object);
        unitOfWorkMock
            .Setup(uow => uow.PlacementRepository)
            .Returns(placementRepositoryMock.Object);

        // Place mock unit of work into services container
        services.AddSingleton(_ => unitOfWorkMock.Object);

        // Place the actual mocks into the services container so they can be configured by provider state middleware
        services.AddSingleton(_ => unitOfWorkMock);
        services.AddSingleton(_ => listRepositoryMock);

        // Add data services
        services.AddScoped<IPackedListsDataService, PackedListsDataService>();
        services.AddScoped<IPackedItemsDataService, PackedItemsDataService>();
        services.AddScoped<IPackedContainersDataService, PackedContainersDataService>();
        services.AddScoped<IPackedPlacementsDataService, PackedPlacementsDataService>();

        // Add API error factory
        services.AddSingleton<ApiErrorFactoryBase, ApiErrorFactory>();
    }

    /// <summary>
    /// Configure request pipeline
    /// </summary>
    /// <param name="app">App</param>
    /// <param name="env">Environment</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Add routing middleware
        app.UseRouting();

        // Add authorization middleware
        app.UseAuthorization();

        // Add provider state setup middleware
        app.UseMiddleware<ProviderStateMiddleware>();

        // Add API endpoints as final piece in the pipeline
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}