namespace TesterMarten.Models;

public abstract class ContainerBase
{
    public int ID { get; set; }
    public int? cID { get; set; }
    public string shortID { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}