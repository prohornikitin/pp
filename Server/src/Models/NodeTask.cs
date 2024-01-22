namespace GrpcServer.Models;

public class NodeTask {
    public long Id { get; set; }
    public required UserTask Task { get; set; }
    public int column;
}