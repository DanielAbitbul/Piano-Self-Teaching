using PianoApp.Api.Models;
using PianoApp.Api.Interfaces;

namespace PianoApp.Api.Services;

public class LessonService : ILessonService
{
    private static readonly List<Lesson> Lessons =
    [
        new Lesson
        {
            Id = 1,
            Title = "The Piano Keyboard",
            Description = "Learn how notes are arranged on the piano keyboard."
        },
        new Lesson
        {
            Id = 2,
            Title = "Notes and Octaves",
            Description = "Learn note names and how the same notes repeat across the keyboard."
        },
        new Lesson
        {
            Id = 3,
            Title = "The C Major Scale",
            Description = "Learn the notes and structure of the C major scale."
        }
    ];

    public IEnumerable<Lesson> GetAllLessons()
    {
        return Lessons;
    }

    public Lesson? GetLessonById(int id)
    {
        return Lessons.FirstOrDefault(lesson => lesson.Id == id);
    }
}