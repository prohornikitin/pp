using Server.Models;

namespace Server.JsonModels;

public class NewUserTask
{
    public required long InitialMatrixId { get; set; }
    public required string Name { get; set; }
    public required IEnumerable<PolynomPart> Polynom { get; set; } = new List<PolynomPart>();
}