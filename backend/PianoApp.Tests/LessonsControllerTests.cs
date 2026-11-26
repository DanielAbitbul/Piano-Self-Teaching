using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using PianoApp.Api.Controllers;
using PianoApp.Api.DTOs;
using PianoApp.Api.Interfaces;

namespace PianoApp.Tests;

public class LessonsControllerTests
{
    [Fact]
    public async Task Get_ReturnsLessonDtos()
    {
        var controller = new LessonsController(new FakeLessonService());

        var result = await controller.Get();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var lessons = Assert.IsAssignableFrom<IEnumerable<LessonDto>>(okResult.Value);
        Assert.Single(lessons);
        Assert.Equal("Test Lesson", lessons.Single().Title);
    }

    [Fact]
    public async Task GetById_WhenLessonDoesNotExist_ReturnsNotFound()
    {
        var controller = new LessonsController(new FakeLessonService());

        var result = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_WhenLessonExists_ReturnsCompleteOrderedStructure()
    {
        var controller = new LessonsController(new FakeLessonService());

        var result = await controller.GetById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var lesson = Assert.IsType<LessonDto>(okResult.Value);
        Assert.Equal("Test Lesson", lesson.Title);
        Assert.Equal([2, 1], lesson.Slides.Select(slide => slide.Id));
        Assert.Single(lesson.Slides[0].Elements);
        Assert.Equal([2, 1], lesson.Slides[1].Elements.Select(element => element.Id));
        Assert.Equal("text", lesson.Slides[1].Elements[1].Type);
        Assert.Equal("First", lesson.Slides[1].Elements[1].Content.GetProperty("text").GetString());
    }

    private sealed class FakeLessonService : ILessonService
    {
        private readonly List<LessonDto> _lessons =
        [
            new LessonDto
            {
                Id = 1,
                CourseId = 1,
                Title = "Test Lesson",
                Description = "A lesson used for testing.",
                Order = 1,
                Slides =
                [
                    new SlideDto
                    {
                        Id = 2,
                        LessonId = 1,
                        Title = "Second slide",
                        Order = 2,
                        Elements =
                        [
                            new SlideElementDto
                            {
                                Id = 3,
                                SlideId = 2,
                                Type = "heading",
                                Order = 1,
                                Content = ParseContent("{\"text\":\"Second slide\"}")
                            }
                        ]
                    },
                    new SlideDto
                    {
                        Id = 1,
                        LessonId = 1,
                        Title = "First slide",
                        Order = 1,
                        Elements =
                        [
                            new SlideElementDto
                            {
                                Id = 2,
                                SlideId = 1,
                                Type = "image",
                                Order = 2,
                                Content = ParseContent("{\"url\":\"image.png\"}")
                            },
                            new SlideElementDto
                            {
                                Id = 1,
                                SlideId = 1,
                                Type = "text",
                                Order = 1,
                                Content = ParseContent("{\"text\":\"First\"}")
                            }
                        ]
                    }
                ]
            }
        ];

        public Task<List<LessonDto>> GetAllLessonsAsync() => Task.FromResult(_lessons);

        private static JsonElement ParseContent(string content)
        {
            using var document = JsonDocument.Parse(content);
            return document.RootElement.Clone();
        }

        public Task<LessonDto?> GetLessonByIdAsync(int id) =>
            Task.FromResult(_lessons.FirstOrDefault(lesson => lesson.Id == id));

        public Task<List<LessonDto>> GetLessonsByCourseIdAsync(int courseId) =>
            Task.FromResult(_lessons.Where(lesson => lesson.CourseId == courseId).ToList());
    }
}
