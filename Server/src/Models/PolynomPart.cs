using Microsoft.EntityFrameworkCore;

namespace Server.Models;

[Owned]
public class PolynomPart
{
    public int Power { get; set; }
    public int Coefficient { get; set; }
}