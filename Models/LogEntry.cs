namespace VTFCreater.Models;

public class LogEntry
{
    public required string Level { get; init; }

    public required string Message { get; init; }

    public required string Timestamp { get; init; }

    public string DisplayText => $"[{Timestamp}] [{Level}] {Message}";
}
