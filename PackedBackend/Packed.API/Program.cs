// Date Created: 2022/12/10
// Created by: JSW

using Microsoft.EntityFrameworkCore;
using Packed.Data.Core.Repositories;
using Packed.Data.EntityFramework;
using Packed.Data.EntityFramework.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Postgres data access via EF Core
builder.Services.AddDbContext<PackedDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PackedDatabase")));

// Add list repository
builder.Services.AddScoped<IListRepository, ListRepository>();

// Build app
var app = builder.Build();

// Configure middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Run app
app.Run();