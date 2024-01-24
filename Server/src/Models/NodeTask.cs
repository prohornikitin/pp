namespace Server.Models;

public class NodeTask {
    public long Id { get; set; }
    public TaskState State { get; set; } = TaskState.WorkInProgress;

    public required long UserTaskId { get; set; }
    public required UserTask UserTask { get; set; }
    public required int column { get; set; }
}