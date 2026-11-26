using PianoApp.Api.DTOs;

namespace PianoApp.Api.Interfaces;

public interface ILessonService
{
    Task<List<LessonDto>> GetAllLessonsAsync();
    Task<LessonDto?> GetLessonByIdAsync(int id);
    Task<List<LessonDto>> GetLessonsByCourseIdAsync(int courseId);
}
