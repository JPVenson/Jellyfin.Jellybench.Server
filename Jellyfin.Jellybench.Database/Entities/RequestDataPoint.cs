namespace Jellyfin.Jellybench.Database.Entities;

public record RequestDataPoint
{
    public int RequestDataPointId { get; set; }
    public string Name { get; set; }
    public int ResolutionX { get; set; }
    public int ResolutionY { get; set; }
    public float Framerate { get; set; }
    public int Bitrate { get; set; }
    public string InputCodec { get; set; }
    public string OutputCodec { get; set; }
    public string ExampleDataUrl { get; set; }
    public bool IsEnabled { get; set; }

    public virtual ICollection<DataPoint> DataPoints { get; set; }
}