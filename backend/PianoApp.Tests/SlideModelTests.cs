using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PianoApp.Api.Data;
using PianoApp.Api.Models;

namespace PianoApp.Tests;

public class SlideModelTests
{
    [Fact]
    public void SlideElementContent_RejectsInvalidNullAndEmptyJson()
    {
        var element = new SlideElement { Type = "text", Content = "{\"text\":\"Hello\"}" };

        Assert.ThrowsAny<JsonException>(() => element.Content = "{ invalid }");
        Assert.Throws<ArgumentNullException>(() => element.Content = null!);
        Assert.ThrowsAny<JsonException>(() => element.Content = string.Empty);
        Assert.Equal("{\"text\":\"Hello\"}", element.Content);
    }

    [Fact]
    public void SlideModel_UsesJsonbAndCascadeRelationships()
    {
        using var context = CreateContext();
        var model = context.Model;

        var contentProperty = model.FindEntityType(typeof(SlideElement))!
            .FindProperty(nameof(SlideElement.Content))!;
        Assert.Equal("jsonb", contentProperty.FindAnnotation("Relational:ColumnType")?.Value);

        var slideForeignKey = model.FindEntityType(typeof(Slide))!
            .GetForeignKeys()
            .Single();
        Assert.Equal(DeleteBehavior.Cascade, slideForeignKey.DeleteBehavior);
        Assert.Contains(slideForeignKey.Properties, property => property.Name == nameof(Slide.LessonId));

        var elementForeignKey = model.FindEntityType(typeof(SlideElement))!
            .GetForeignKeys()
            .Single();
        Assert.Equal(DeleteBehavior.Cascade, elementForeignKey.DeleteBehavior);
        Assert.Contains(elementForeignKey.Properties, property => property.Name == nameof(SlideElement.SlideId));
    }

    private static PianoAppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PianoAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PianoAppDbContext(options);
    }
}
