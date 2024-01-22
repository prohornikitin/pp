namespace GrpcServer.Models;
public class UserTask
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public UserTaskState State;
    public required Polynom polynom;
    public required Matrix inputMatrix;
    public Matrix? result;
}