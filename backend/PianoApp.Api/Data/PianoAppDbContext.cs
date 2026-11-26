
using Microsoft.EntityFrameworkCore;
using PianoApp.Api.Models;

namespace PianoApp.Api.Data;

public class PianoAppDbContext : DbContext
{
    public PianoAppDbContext(DbContextOptions<PianoAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Slide> Slides => Set<Slide>();
    public DbSet<SlideElement> SlideElements => Set<SlideElement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lesson>()
            .HasOne(lesson => lesson.Course)
            .WithMany(course => course.Lessons)
            .HasForeignKey(lesson => lesson.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Slide>()
            .HasOne(slide => slide.Lesson)
            .WithMany(lesson => lesson.Slides)
            .HasForeignKey(slide => slide.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SlideElement>(entity =>
        {
            entity.HasOne(slideElement => slideElement.Slide)
                .WithMany(slide => slide.Elements)
                .HasForeignKey(slideElement => slideElement.SlideId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(slideElement => slideElement.Content)
                .HasColumnType("jsonb")
                .IsRequired();
        });
    }
}
