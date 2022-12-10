// Date Created: 2022/12/10
// Created by: JSW

using Microsoft.EntityFrameworkCore;
using Packed.API.Services;
using Packed.Data.Core.Repositories;
using Packed.Data.EntityFramework;
using Packed.Data.EntityFramework.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Postgres data access via EF Core
builder.Services.AddDbContext<PackedDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PackedDatabase")));

// Add repositories
builder.Services.AddScoped<IListRepository, ListRepository>();

// Add data service
builder.Services.AddScoped<IPackedDataService, PackedDataService>();

// Build app
var app = builder.Build();

// Configure middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Run app
app.Run();