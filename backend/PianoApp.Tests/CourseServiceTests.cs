
using Microsoft.EntityFrameworkCore;
using PianoApp.Api.Data;
using PianoApp.Api.Models;
using PianoApp.Api.Services;

namespace PianoApp.Tests;

public class CourseServiceTests
{
    [Fact]
    public async Task GetAllCourses_ReturnsCourses()
    {
        // Arrange
        using var context = CreateContext();

        context.Courses.Add(new Course
        {
            Id = 1,
            Title = "Piano Fundamentals",
            Description = "Learn the essential piano fundamentals."
        });

        await context.SaveChangesAsync();

        var service = new CourseService(context);

        // Act
        var courses = await service.GetAllCoursesAsync();

        // Assert
        Assert.Single(courses);
        Assert.Equal("Piano Fundamentals", courses[0].Title);
    }

    [Fact]
    public async Task GetCourseById_ReturnsCourse_WhenCourseExists()
    {
        // Arrange
        using var context = CreateContext();

        context.Courses.Add(new Course
        {
            Id = 1,
            Title = "Piano Fundamentals",
            Description = "Learn the essential piano fundamentals."
        });

        await context.SaveChangesAsync();

        var service = new CourseService(context);

        // Act
        var course = await service.GetCourseByIdAsync(1);

        // Assert
        Assert.NotNull(course);
        Assert.Equal("Piano Fundamentals", course.Title);
    }

    [Fact]
    public async Task GetCourseById_ReturnsNull_WhenCourseDoesNotExist()
    {
        // Arrange
        using var context = CreateContext();
        var service = new CourseService(context);

        // Act
        var course = await service.GetCourseByIdAsync(999);

        // Assert
        Assert.Null(course);
    }

    private static PianoAppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PianoAppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PianoAppDbContext(options);
    }
}
