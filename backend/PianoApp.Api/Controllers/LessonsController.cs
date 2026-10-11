using Microsoft.AspNetCore.Mvc;
using PianoApp.Api.Models;
using PianoApp.Api.Services;
using PianoApp.Api.Interfaces;
using Microsoft.Extensions.Options;
using PianoApp.Api.Options;

namespace PianoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;
    private readonly ILogger<LessonsController> _logger;
    private readonly PianoAppOptions _options;

    public LessonsController(
        ILessonService lessonService,
        ILogger<LessonsController> logger,
        IOptions<PianoAppOptions> options)
    {
        _lessonService = lessonService;
        _logger = logger;
        _options = options.Value;
    }

    [HttpGet]
    public IEnumerable<Lesson> Get()
    {
        _logger.LogInformation(
            "{ApplicationName}: Retrieving lessons. Configured page size: {PageSize}",
            _options.ApplicationName,
            _options.LessonsPerPage
        );

        return _lessonService.GetAllLessons();
    }

    [HttpGet("{id}")]
    public ActionResult<Lesson> GetById(int id)
    {
        _logger.LogInformation("Retrieving lesson with ID {LessonId}", id);

        var lesson = _lessonService.GetLessonById(id);

        if (lesson is null)
        {
            _logger.LogWarning("Lesson {LessonId} was not found", id);

            return NotFound();
        }

        _logger.LogInformation("Successfully retrieved lesson {LessonId}", id);

        return Ok(lesson);
    }
}