namespace Server.Models;

public class NodeTask {
    public long Id { get; set; }
    public required UserTask UserTask { get; set; }
    public required int column { get; set; }
    public Matrix? result { get; set; }
}