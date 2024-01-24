using Server.Models;

namespace Server.JsonModels;

public class UserTaskMinimalInfo
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public TaskState Status { get; set; }
    public IEnumerable<PolynomPart> Polynom { get; set; } = new List<PolynomPart>();
}