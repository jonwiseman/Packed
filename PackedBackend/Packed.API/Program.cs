// Date Created: 2022/12/10
// Created by: JSW

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packed.API.Factories;
using Packed.API.Services;
using Packed.Data.Core.Repositories;
using Packed.Data.EntityFramework;
using Packed.Data.EntityFramework.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Suppress the default model state invalid filter. We'll use our own instead
builder.Services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });

// Add Postgres data access via EF Core
builder.Services.AddDbContext<PackedDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PackedDatabase")));

// Add unit of work
builder.Services.AddScoped<IPackedUnitOfWork, PackedUnitOfWork>();

// Add data service
builder.Services.AddScoped<IPackedDataService, PackedDataService>();

// Add API error factory
builder.Services.AddSingleton<ApiErrorFactoryBase, ApiErrorFactory>();

// Build app
var app = builder.Build();

// Configure middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Run app
app.Run();