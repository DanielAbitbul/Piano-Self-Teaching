using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PianoApp.Api.Data;
using PianoApp.Api.Models;

namespace PianoApp.Tests;

public class LearnCatalogueSeederTests
{
    [Fact]
    public async Task SeedAsync_CanRunMoreThanOnceWithoutCreatingDuplicates()
    {
        var databaseName = Guid.NewGuid().ToString();
        var databaseRoot = new InMemoryDatabaseRoot();

        await using (var firstContext = CreateContext(databaseName, databaseRoot))
        {
            await LearnCatalogueSeeder.SeedAsync(firstContext);
        }

        await using (var secondContext = CreateContext(databaseName, databaseRoot))
        {
            await LearnCatalogueSeeder.SeedAsync(secondContext);

            var course = await secondContext.Courses
                .Include(existingCourse => existingCourse.Lessons)
                    .ThenInclude(lesson => lesson.Slides)
                        .ThenInclude(slide => slide.Elements)
                .SingleAsync();

            Assert.Equal("Piano Fundamentals", course.Title);
            Assert.Equal(
                [
                    "Getting to Know the Piano",
                    "Understanding Musical Notation",
                    "Rhythm and Timing",
                    "Scales and Intervals",
                    "Chords and Harmony",
                    "Playing with Both Hands"
                ],
                course.Lessons.OrderBy(lesson => lesson.Order).Select(lesson => lesson.Title));
            Assert.All(course.Lessons, lesson => Assert.False(string.IsNullOrWhiteSpace(lesson.Description)));

            var firstLesson = course.Lessons.Single(lesson => lesson.Title == "Getting to Know the Piano");
            var slides = firstLesson.Slides.OrderBy(slide => slide.Order).ToList();
            Assert.Equal(2, slides.Count);
            Assert.Equal([1, 2], slides.Select(slide => slide.Order));
            Assert.All(slides, slide => Assert.Equal(2, slide.Elements.Count));
            Assert.Equal(
                ["heading", "text"],
                slides[0].Elements.OrderBy(element => element.Order).Select(element => element.Type));

            foreach (var element in slides.SelectMany(slide => slide.Elements))
            {
                using var document = JsonDocument.Parse(element.Content);
                Assert.Equal(JsonValueKind.Object, document.RootElement.ValueKind);
            }

            Assert.Equal(6, await secondContext.Lessons.CountAsync());
            Assert.Equal(2, await secondContext.Slides.CountAsync());
            Assert.Equal(4, await secondContext.SlideElements.CountAsync());
        }
    }

    [Fact]
    public async Task SeedAsync_FillsPartialCatalogueAndPreservesUnrelatedElementAtSeedOrder()
    {
        var databaseName = Guid.NewGuid().ToString();
        var databaseRoot = new InMemoryDatabaseRoot();
        await using var context = CreateContext(databaseName, databaseRoot);

        context.Courses.Add(new Course
        {
            Title = "Piano Fundamentals",
            Description = "An existing course description.",
            Lessons =
            [
                new Lesson
                {
                    Title = "Getting to Know the Piano",
                    Description = "An existing lesson description.",
                    Order = 1,
                    Slides =
                    [
                        new Slide
                        {
                            Title = "The Piano at a Glance",
                            Order = 1,
                            Elements =
                            [
                                new SlideElement
                                {
                                    Type = "heading",
                                    Order = 1,
                                    Content = "{ \"text\" : \"A keyboard of repeating patterns\" }"
                                },
                                new SlideElement
                                {
                                    Type = "image",
                                    Order = 2,
                                    Content = "{\"url\":\"my-image.png\"}"
                                }
                            ]
                        }
                    ]
                }
            ]
        });
        await context.SaveChangesAsync();

        await LearnCatalogueSeeder.SeedAsync(context);
        await LearnCatalogueSeeder.SeedAsync(context);

        var course = await context.Courses
            .Include(existingCourse => existingCourse.Lessons)
                .ThenInclude(lesson => lesson.Slides)
                    .ThenInclude(slide => slide.Elements)
            .SingleAsync();
        var firstLesson = course.Lessons.Single(lesson => lesson.Title == "Getting to Know the Piano");
        var firstSlide = firstLesson.Slides.Single(slide => slide.Title == "The Piano at a Glance");

        Assert.Equal("An existing course description.", course.Description);
        Assert.Equal("An existing lesson description.", firstLesson.Description);
        Assert.Equal(6, course.Lessons.Count);
        Assert.Equal(2, firstLesson.Slides.Count);
        Assert.Equal(5, firstLesson.Slides.Sum(slide => slide.Elements.Count));

        var unrelatedElement = Assert.Single(firstSlide.Elements, element => element.Type == "image");
        Assert.Equal(2, unrelatedElement.Order);
        Assert.Equal("{\"url\":\"my-image.png\"}", unrelatedElement.Content);

        var seededHeading = Assert.Single(firstSlide.Elements, element => element.Type == "heading");
        Assert.Equal("{ \"text\" : \"A keyboard of repeating patterns\" }", seededHeading.Content);
        Assert.Single(firstSlide.Elements, element =>
            element.Type == "text" &&
            element.Content.StartsWith("{\"text\":\"A piano keyboard is made of white and black keys.", StringComparison.Ordinal));
    }

    private static PianoAppDbContext CreateContext(string databaseName, InMemoryDatabaseRoot databaseRoot)
    {
        var options = new DbContextOptionsBuilder<PianoAppDbContext>()
            .UseInMemoryDatabase(databaseName, databaseRoot)
            .Options;

        return new PianoAppDbContext(options);
    }
}
