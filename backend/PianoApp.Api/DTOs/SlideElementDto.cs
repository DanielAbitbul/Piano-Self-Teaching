using System.Text.Json;

namespace PianoApp.Api.DTOs;

public class SlideElementDto
{
    public int Id { get; set; }

    public int SlideId { get; set; }

    public string Type { get; set; } = string.Empty;

    public int Order { get; set; }

    public JsonElement Content { get; set; }
}
