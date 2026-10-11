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
builder.Services.AddSingleton<ILessonService, LessonService>();
builder.Services.Configure<PianoAppOptions>(
    builder.Configuration.GetSection("PianoApp")
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
