
using Microsoft.AspNetCore.Mvc;
using PianoApp.Api.DTOs;
using PianoApp.Api.Interfaces;

namespace PianoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ILessonService _lessonService;

    public CoursesController(
        ICourseService courseService,
        ILessonService lessonService)
    {
        _courseService = courseService;
        _lessonService = lessonService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> Get()
    {
        var courses = await _courseService.GetAllCoursesAsync();
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetById(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        return Ok(course);
    }

    [HttpGet("{id}/lessons")]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetLessons(int id)
    {
        var course = await _courseService.GetCourseByIdAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        var lessons = await _lessonService.GetLessonsByCourseIdAsync(id);

        return Ok(lessons);
    }
}
