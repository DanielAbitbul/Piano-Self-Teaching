using PianoApp.Api.Models;

namespace PianoApp.Api.Interfaces;

public interface ILessonService
{
    IEnumerable<Lesson> GetAllLessons();

    Lesson? GetLessonById(int id);
}