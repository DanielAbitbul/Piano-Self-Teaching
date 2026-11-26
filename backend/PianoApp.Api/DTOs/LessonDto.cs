namespace PianoApp.Api.DTOs;

public class LessonDto
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Order { get; set; }

    public List<SlideDto> Slides { get; set; } = [];
}
