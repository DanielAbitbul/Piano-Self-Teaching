
using Microsoft.EntityFrameworkCore;
using PianoApp.Api.Data;
using PianoApp.Api.Models;
using PianoApp.Api.Services;

namespace PianoApp.Tests;

public class LessonServiceTests
{
    [Fact]
    public async Task GetAllLessons_ReturnsLessonsInOrder()
    {
        // Arrange
        using var context = CreateContext();

        context.Courses.AddRange(
            new Course
            {
                Id = 1,
                Title = "Piano Fundamentals",
                Description = "Learn piano fundamentals."
            },
            new Course
            {
                Id = 2,
                Title = "Music Theory",
                Description = "Learn music theory."
            }
        );

        context.Lessons.AddRange(
            new Lesson
            {
                Id = 1,
                CourseId = 1,
                Title = "First Lesson",
                Description = "First test lesson.",
                Order = 1
            },
            new Lesson
            {
                Id = 2,
                CourseId = 1,
                Title = "Second Lesson",
                Description = "Second test lesson.",
                Order = 2
            },
            new Lesson
            {
                Id = 3,
                CourseId = 2,
                Title = "Third Lesson",
                Description = "Third test lesson.",
                Order = 3
            }
        );

        await context.SaveChangesAsync();

        var service = new LessonService(context);

        // Act
        var lessons = await service.GetAllLessonsAsync();

        // Assert
        Assert.Equal(3, lessons.Count);

        Assert.Equal(
            [1, 2, 3],
            lessons.Select(lesson => lesson.Id)
        );
    }

    [Fact]
    public async Task GetLessonsByCourseId_ReturnsMatchingLessonsInOrder()
    {
        // Arrange
        using var context = CreateContext();

        context.Courses.AddRange(
            new Course
            {
                Id = 1,
                Title = "Piano Fundamentals",
                Description = "Learn piano fundamentals."
            },
            new Course
            {
                Id = 2,
                Title = "Music Theory",
                Description = "Learn music theory."
            }
        );

        context.Lessons.AddRange(
            new Lesson
            {
                Id = 1,
                CourseId = 1,
                Title = "First Lesson",
                Description = "First test lesson.",
                Order = 1
            },
            new Lesson
            {
                Id = 2,
                CourseId = 1,
                Title = "Second Lesson",
                Description = "Second test lesson.",
                Order = 2
            },
            new Lesson
            {
                Id = 3,
                CourseId = 2,
                Title = "Third Lesson",
                Description = "Third test lesson.",
                Order = 3
            }
        );

        await context.SaveChangesAsync();

        var service = new LessonService(context);

        // Act
        var lessons = await service.GetLessonsByCourseIdAsync(1);

        // Assert
        Assert.Equal(2, lessons.Count);
        Assert.All(lessons, lesson => Assert.Equal(1, lesson.CourseId));

        Assert.Equal(
            [1, 2],
            lessons.Select(lesson => lesson.Id)
        );
    }

    [Fact]
    public async Task GetLessonById_ReturnsSlidesAndElementsInOrder()
    {
        using var context = CreateContext();
        context.Courses.Add(new Course
        {
            Id = 1,
            Title = "Piano Fundamentals",
            Description = "Learn piano fundamentals."
        });

        context.Lessons.Add(new Lesson
        {
            Id = 1,
            CourseId = 1,
            Title = "Ordered Content",
            Description = "A lesson with slides and elements.",
            Order = 1,
            Slides =
            [
                new Slide
                {
                    Id = 2,
                    LessonId = 1,
                    Title = "Second slide",
                    Order = 2,
                    Elements =
                    [
                        new SlideElement
                        {
                            Id = 3,
                            SlideId = 2,
                            Type = "audio",
                            Order = 1,
                            Content = "{\"url\":\"example.mp3\"}"
                        }
                    ]
                },
                new Slide
                {
                    Id = 1,
                    LessonId = 1,
                    Title = "First slide",
                    Order = 1,
                    Elements =
                    [
                        new SlideElement
                        {
                            Id = 2,
                            SlideId = 1,
                            Type = "image",
                            Order = 2,
                            Content = "{\"url\":\"example.png\"}"
                        },
                        new SlideElement
                        {
                            Id = 1,
                            SlideId = 1,
                            Type = "text",
                            Order = 1,
                            Content = "{\"text\":\"First\"}"
                        }
                    ]
                }
            ]
        });
        await context.SaveChangesAsync();

        var service = new LessonService(context);
        var lesson = await service.GetLessonByIdAsync(1);

        Assert.NotNull(lesson);
        Assert.Equal("Ordered Content", lesson.Title);
        Assert.Equal([1, 2], lesson.Slides.Select(slide => slide.Id));
        Assert.Equal([1, 2], lesson.Slides[0].Elements.Select(element => element.Id));
        Assert.Equal("First", lesson.Slides[0].Elements[0].Content.GetProperty("text").GetString());
        Assert.Single(lesson.Slides[1].Elements);
    }

    private static PianoAppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PianoAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PianoAppDbContext(options);
    }
}
