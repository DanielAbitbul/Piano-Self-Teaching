using System.Text.Json;

namespace PianoApp.Api.Models;

public class SlideElement
{
    private string _content = "{}";

    public int Id { get; set; }

    public int SlideId { get; set; }

    public string Type { get; set; } = string.Empty;

    public int Order { get; set; }

    // Keep the payload opaque while validating every assignment before it reaches jsonb.
    public string Content
    {
        get => _content;
        set
        {
            using var document = JsonDocument.Parse(value);
            _content = value;
        }
    }

    public Slide? Slide { get; set; }
}
