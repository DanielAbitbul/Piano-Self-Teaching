namespace PianoApp.Api.DTOs;

public class SlideDto
{
    public int Id { get; set; }

    public int LessonId { get; set; }

    public string? Title { get; set; }

    public int Order { get; set; }

    public List<SlideElementDto> Elements { get; set; } = [];
}
