namespace PianoApp.Api.Models;

public class Slide
{
    public int Id { get; set; }

    public int LessonId { get; set; }

    public string? Title { get; set; }

    public int Order { get; set; }

    public Lesson? Lesson { get; set; }

    public ICollection<SlideElement> Elements { get; set; } = new List<SlideElement>();
}
