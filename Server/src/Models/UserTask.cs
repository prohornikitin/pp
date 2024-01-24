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
    
    public Matrix? Result { get; set; } = null;
}