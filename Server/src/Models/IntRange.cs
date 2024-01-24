using Microsoft.EntityFrameworkCore;

namespace Server.Models;

[Owned]
public class IntRange
{
    public required int Start {get; set;}
    public required int End {get; set;}

    public bool IsEmpty() => End == Start;
}