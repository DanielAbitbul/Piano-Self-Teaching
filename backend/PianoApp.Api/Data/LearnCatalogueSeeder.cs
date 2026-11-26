using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PianoApp.Api.Models;

namespace PianoApp.Api.Data;

public static class LearnCatalogueSeeder
{
    private const string CourseTitle = "Piano Fundamentals";
    private const string FirstLessonTitle = "Getting to Know the Piano";

    private static readonly (string Title, string Description)[] InitialLessons =
    [
        (FirstLessonTitle, "Explore the piano keyboard, its key groups, and the basic parts of the instrument."),
        ("Understanding Musical Notation", "Learn how the staff, clefs, note names, and written symbols guide your playing."),
        ("Rhythm and Timing", "Build a steady pulse and learn how note values organize music in time."),
        ("Scales and Intervals", "Discover step patterns, major scales, and the distances between notes."),
        ("Chords and Harmony", "Learn how notes combine into chords and how chords create harmonic movement."),
        ("Playing with Both Hands", "Coordinate simple patterns between your left and right hands."),
    ];

    private sealed record SeedSlide(string Title, (string Type, string Content)[] Elements);

    private static readonly SeedSlide[] FirstLessonSlides =
    [
        new("The Piano at a Glance",
        [
            ("heading", "{\"text\":\"A keyboard of repeating patterns\"}"),
            ("text", "{\"text\":\"A piano keyboard is made of white and black keys. The black keys repeat in groups of two and three, giving you visual landmarks as you move across the instrument.\"}"),
        ]),
        new("Finding Your Way Around the Keys",
        [
            ("heading", "{\"text\":\"Find the note C\"}"),
            ("text", "{\"text\":\"Look for a group of two black keys. The white key immediately to the left of that pair is C. From there, the white-key names continue D, E, F, G, A, B, then repeat.\"}"),
        ]),
    ];

    public static async Task SeedAsync(
        PianoAppDbContext context,
        CancellationToken cancellationToken = default)
    {
        // Repeated sequential calls are safe; concurrent startup instances can race without database uniqueness constraints.
        var course = await context.Courses
            .Include(existingCourse => existingCourse.Lessons)
                .ThenInclude(lesson => lesson.Slides)
                    .ThenInclude(slide => slide.Elements)
            .FirstOrDefaultAsync(existingCourse => existingCourse.Title == CourseTitle, cancellationToken);

        if (course is null)
        {
            course = new Course
            {
                Title = CourseTitle,
                Description = "Build a foundation in piano technique, notation, rhythm, and harmony."
            };
            context.Courses.Add(course);
        }

        for (var lessonIndex = 0; lessonIndex < InitialLessons.Length; lessonIndex++)
        {
            var (title, description) = InitialLessons[lessonIndex];
            if (course.Lessons.Any(lesson => lesson.Title == title))
            {
                continue;
            }

            course.Lessons.Add(new Lesson
            {
                Course = course,
                Title = title,
                Description = description,
                Order = lessonIndex + 1
            });
        }

        await context.SaveChangesAsync(cancellationToken);

        var firstLesson = course.Lessons.First(lesson => lesson.Title == FirstLessonTitle);
        for (var slideIndex = 0; slideIndex < FirstLessonSlides.Length; slideIndex++)
        {
            var seedSlide = FirstLessonSlides[slideIndex];
            var slide = firstLesson.Slides.FirstOrDefault(existingSlide => existingSlide.Title == seedSlide.Title);

            if (slide is null)
            {
                slide = new Slide
                {
                    Lesson = firstLesson,
                    Title = seedSlide.Title,
                    Order = slideIndex + 1
                };
                firstLesson.Slides.Add(slide);
            }

            var slideElements = seedSlide.Elements;
            for (var elementIndex = 0; elementIndex < slideElements.Length; elementIndex++)
            {
                var (type, content) = slideElements[elementIndex];
                if (slide.Elements.Any(element => MatchesSeedElement(element, type, content)))
                {
                    continue;
                }

                slide.Elements.Add(new SlideElement
                {
                    Slide = slide,
                    Type = type,
                    Order = elementIndex + 1,
                    Content = content
                });
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static bool MatchesSeedElement(SlideElement element, string type, string content)
    {
        if (element.Type != type)
        {
            return false;
        }

        using var existingContent = JsonDocument.Parse(element.Content);
        using var seedContent = JsonDocument.Parse(content);
        return JsonElement.DeepEquals(existingContent.RootElement, seedContent.RootElement);
    }
}
