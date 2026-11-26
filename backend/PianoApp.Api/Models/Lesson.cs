namespace PianoApp.Api.Models;

public class Lesson
{
    public int Id { get; set; }

    public int CourseId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int Order { get; set; }

    public Course? Course { get; set; }

    public ICollection<Slide> Slides { get; set; } = new List<Slide>();
}
