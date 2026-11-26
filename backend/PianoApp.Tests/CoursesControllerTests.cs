
using Microsoft.AspNetCore.Mvc;
using PianoApp.Api.Controllers;
using PianoApp.Api.DTOs;
using PianoApp.Api.Interfaces;
using PianoApp.Api.Models;

namespace PianoApp.Tests;

public class CoursesControllerTests
{
    [Fact]
    public async Task Get_ReturnsAllCourses()
    {
        // Arrange
        var controller = new CoursesController(
            new FakeCourseService(),
            new FakeLessonService()
        );

        // Act
        var result = await controller.Get();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var courses = Assert.IsAssignableFrom<IEnumerable<CourseDto>>(okResult.Value)
            .ToList();

        Assert.Equal(2, courses.Count);
        Assert.Equal(1, courses[0].Id);
        Assert.Equal("Test Course", courses[0].Title);
        Assert.Equal(2, courses[1].Id);
        Assert.Equal("Empty Course", courses[1].Title);
    }

    [Fact]
    public async Task GetById_WhenCourseExists_ReturnsCourse()
    {
        // Arrange
        var controller = new CoursesController(
            new FakeCourseService(),
            new FakeLessonService()
        );

        // Act
        var result = await controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var course = Assert.IsType<CourseDto>(okResult.Value);

        Assert.Equal(1, course.Id);
        Assert.Equal("Test Course", course.Title);
    }

    [Fact]
    public async Task GetById_WhenCourseDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var controller = new CoursesController(
            new FakeCourseService(),
            new FakeLessonService()
        );

        // Act
        var result = await controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetLessons_ReturnsLessonsForCourse()
    {
        // Arrange
        var controller = new CoursesController(
            new FakeCourseService(),
            new FakeLessonService()
        );

        // Act
        var result = await controller.GetLessons(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var lessons = Assert.IsAssignableFrom<IEnumerable<LessonDto>>(okResult.Value);

        Assert.Equal(2, lessons.Count());
        Assert.All(lessons, lesson => Assert.Equal(1, lesson.CourseId));
    }

    [Fact]
    public async Task GetLessons_ReturnsEmptyList_WhenCourseHasNoLessons()
    {
        // Arrange
        var controller = new CoursesController(
            new FakeCourseService(),
            new FakeLessonService()
        );

        // Act
        var result = await controller.GetLessons(2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var lessons = Assert.IsAssignableFrom<IEnumerable<LessonDto>>(okResult.Value);

        Assert.Empty(lessons);
    }

    [Fact]
    public async Task GetLessons_ReturnsNotFound_WhenCourseDoesNotExist()
    {
        // Arrange
        var controller = new CoursesController(
            new FakeCourseService(),
            new FakeLessonService()
        );

        // Act
        var result = await controller.GetLessons(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    private class FakeCourseService : ICourseService
    {
        private readonly List<CourseDto> _courses =
        [
            new CourseDto
            {
                Id = 1,
                Title = "Test Course",
                Description = "A course used for testing."
            },
            new CourseDto
            {
                Id = 2,
                Title = "Empty Course",
                Description = "A course without lessons."
            }
        ];

        public Task<List<CourseDto>> GetAllCoursesAsync()
        {
            return Task.FromResult(_courses);
        }

        public Task<CourseDto?> GetCourseByIdAsync(int id)
        {
            var course = _courses.FirstOrDefault(course => course.Id == id);
            return Task.FromResult(course);
        }
    }

    private class FakeLessonService : ILessonService
    {
        private readonly List<LessonDto> _lessons =
        [
            new LessonDto
            {
                Id = 1,
                CourseId = 1,
                Title = "Test Lesson",
                Description = "A lesson used for testing.",
                Order = 1
            },
            new LessonDto
            {
                Id = 2,
                CourseId = 1,
                Title = "Second Test Lesson",
                Description = "Another lesson used for testing.",
                Order = 2
            }
        ];

        public Task<List<LessonDto>> GetAllLessonsAsync()
        {
            return Task.FromResult(_lessons);
        }

        public Task<LessonDto?> GetLessonByIdAsync(int id)
        {
            var lesson = _lessons.FirstOrDefault(lesson => lesson.Id == id);
            return Task.FromResult(lesson);
        }

        public Task<List<LessonDto>> GetLessonsByCourseIdAsync(int courseId)
        {
            var lessons = _lessons
                .Where(lesson => lesson.CourseId == courseId)
                .OrderBy(lesson => lesson.Order)
                .ToList();

            return Task.FromResult(lessons);
        }
    }
}
