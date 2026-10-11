using Microsoft.AspNetCore.Mvc;
using PianoApp.Api.Controllers;
using PianoApp.Api.Interfaces;
using PianoApp.Api.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using PianoApp.Api.Options;

namespace PianoApp.Tests;

public class LessonsControllerTests
{
    [Fact]
    public void GetById_WhenLessonDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var service = new FakeLessonService();
        var controller = new LessonsController(
            service,
            NullLogger<LessonsController>.Instance,
            Options.Create(new PianoAppOptions
            {
                ApplicationName = "Piano App Tests",
                LessonsPerPage = 10
            })
        );

        // Act
        var result = controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void GetById_WhenLessonExists_ReturnsLesson()
    {
        // Arrange
        var service = new FakeLessonService();
        var controller = new LessonsController(
            service,
            NullLogger<LessonsController>.Instance,
            Options.Create(new PianoAppOptions
            {
                ApplicationName = "Piano App Tests",
                LessonsPerPage = 10
            })
        );

        // Act
        var result = controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var lesson = Assert.IsType<Lesson>(okResult.Value);

        Assert.Equal(1, lesson.Id);
        Assert.Equal("Test Lesson", lesson.Title);
    }

    private class FakeLessonService : ILessonService
    {
        public IEnumerable<Lesson> GetAllLessons()
        {
            return [];
        }

        public Lesson? GetLessonById(int id)
        {
            if (id == 1)
            {
                return new Lesson
                {
                    Id = 1,
                    Title = "Test Lesson",
                    Description = "A lesson used for testing."
                };
            }

            return null;
        }
    }
}