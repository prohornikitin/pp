namespace GrpcServer.Models;

public class Polynom
{
    public class Part
    {
        public int power;
        public int coefficient;
    }
    public long Id { get; set; }
    public required List<Part> parts;
}