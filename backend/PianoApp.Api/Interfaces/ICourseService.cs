using PianoApp.Api.DTOs;

namespace PianoApp.Api.Interfaces;

public interface ICourseService
{
    Task<List<CourseDto>> GetAllCoursesAsync();
    Task<CourseDto?> GetCourseByIdAsync(int id);
}
