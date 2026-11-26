using Microsoft.AspNetCore.Mvc;
using PianoApp.Api.DTOs;
using PianoApp.Api.Interfaces;

namespace PianoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LessonDto>>> Get()
    {
        var lessons = await _lessonService.GetAllLessonsAsync();
        return Ok(lessons);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LessonDto>> GetById(int id)
    {
        var lesson = await _lessonService.GetLessonByIdAsync(id);

        if (lesson == null)
        {
            return NotFound();
        }

        return Ok(lesson);
    }
}
