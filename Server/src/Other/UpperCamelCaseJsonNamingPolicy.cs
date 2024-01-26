using System.Text.Json;

namespace Server.Other;
public class PascalCaseJsonNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        var lowerCamelCase = CamelCase.ConvertName(name);
        return char.ToUpper(lowerCamelCase[0]) + lowerCamelCase.Substring(1);
    }
}