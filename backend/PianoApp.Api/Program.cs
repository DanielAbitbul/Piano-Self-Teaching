using Microsoft.EntityFrameworkCore;
using PianoApp.Api.Data;
using PianoApp.Api.Services;
using PianoApp.Api.Interfaces;
using PianoApp.Api.Options;


var builder = WebApplication.CreateBuilder(args);
var applicationName = builder.Configuration["PianoApp:ApplicationName"];

Console.WriteLine($"Starting {applicationName}");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<PianoAppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PianoApp")
    )
);
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.Configure<PianoAppOptions>(
    builder.Configuration.GetSection("PianoApp")
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<PianoAppDbContext>();
    await dbContext.Database.MigrateAsync();
    await LearnCatalogueSeeder.SeedAsync(dbContext);

    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
