namespace Server.Models;

public class UserTask
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public TaskState State { get; set; } = TaskState.WorkInProgress;
    public required IEnumerable<PolynomPart> Polynom { get; set; } = new List<PolynomPart>();
    public required IntRange UnscheduledColumns { get; set; }

    public required long InitialMatrixId { get; set; }
    public required Matrix InitialMatrix { get; set; }
    
    public required long ResultMatrixId { get; set; }
    public required Matrix Result { get; set; }
}