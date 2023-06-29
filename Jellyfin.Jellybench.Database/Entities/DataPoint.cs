namespace Jellyfin.Jellybench.Database.Entities;

public record DataPoint
{
    public long DataPointId { get; set; }
    public string Name { get; set; }
    public int NumberOfStreams { get; set; }
    public int IdBenchRun { get; set; }
    public BenchRun BenchRun { get; set; }
    public int IdRequestDataPoint { get; set; }
    public RequestDataPoint RequestDataPoint { get; set; }
}