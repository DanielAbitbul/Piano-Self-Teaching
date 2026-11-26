using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using PianoApp.Api.Data;
using PianoApp.Api.DTOs;
using PianoApp.Api.Interfaces;
using PianoApp.Api.Models;

namespace PianoApp.Api.Services;

public class LessonService : ILessonService
{
    private readonly PianoAppDbContext _context;

    public LessonService(PianoAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LessonDto>> GetAllLessonsAsync()
    {
        var lessons = await _context.Lessons
            .AsNoTracking()
            .OrderBy(lesson => lesson.Order)
            .Include(lesson => lesson.Slides)
                .ThenInclude(slide => slide.Elements)
            .ToListAsync();

        return lessons.Select(ToDto).ToList();
    }

    public async Task<LessonDto?> GetLessonByIdAsync(int id)
    {
        var lesson = await _context.Lessons
            .AsNoTracking()
            .Where(lesson => lesson.Id == id)
            .Include(lesson => lesson.Slides)
                .ThenInclude(slide => slide.Elements)
            .FirstOrDefaultAsync();

        return lesson is null ? null : ToDto(lesson);
    }

    public async Task<List<LessonDto>> GetLessonsByCourseIdAsync(int courseId)
    {
        var lessons = await _context.Lessons
            .AsNoTracking()
            .Where(lesson => lesson.CourseId == courseId)
            .OrderBy(lesson => lesson.Order)
            .Include(lesson => lesson.Slides)
                .ThenInclude(slide => slide.Elements)
            .ToListAsync();

        return lessons.Select(ToDto).ToList();
    }

    private static LessonDto ToDto(Lesson lesson)
    {
        return new LessonDto
        {
            Id = lesson.Id,
            CourseId = lesson.CourseId,
            Title = lesson.Title,
            Description = lesson.Description,
            Order = lesson.Order,
            Slides = lesson.Slides
                .OrderBy(slide => slide.Order)
                .Select(slide => new SlideDto
                {
                    Id = slide.Id,
                    LessonId = slide.LessonId,
                    Title = slide.Title,
                    Order = slide.Order,
                    Elements = slide.Elements
                        .OrderBy(element => element.Order)
                        .Select(element => new SlideElementDto
                        {
                            Id = element.Id,
                            SlideId = element.SlideId,
                            Type = element.Type,
                            Order = element.Order,
                            Content = ParseContent(element.Content)
                        })
                        .ToList()
                })
                .ToList()
        };
    }

    private static JsonElement ParseContent(string content)
    {
        using var document = JsonDocument.Parse(content);
        return document.RootElement.Clone();
    }
}
