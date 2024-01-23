using Server.Models;

namespace Server.ViewModels;

public class UserTaskMinimalInfo
{
    public required string Name { get; set; }
    public IEnumerable<PolynomPart> Polynom { get; set; } = new List<PolynomPart>();
    public required long InitialMatrixId;
}