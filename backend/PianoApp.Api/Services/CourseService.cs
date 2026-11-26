using Microsoft.EntityFrameworkCore;
using PianoApp.Api.Data;
using PianoApp.Api.DTOs;
using PianoApp.Api.Interfaces;
using PianoApp.Api.Models;

namespace PianoApp.Api.Services;

public class CourseService : ICourseService
{
    private readonly PianoAppDbContext _context;

    public CourseService(PianoAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CourseDto>> GetAllCoursesAsync()
    {
        return await _context.Courses
            .AsNoTracking()
            .Select(course => new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description
            })
            .ToListAsync();
    }

    public async Task<CourseDto?> GetCourseByIdAsync(int id)
    {
        return await _context.Courses
            .AsNoTracking()
            .Where(course => course.Id == id)
            .Select(course => new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description
            })
            .FirstOrDefaultAsync();
    }
}
