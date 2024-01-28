namespace NodeClient;
public record Config(
    string Ip,
    string Port,
    string MatricesDir
)
{
    public string FullAddress => $"http://{Ip}:{Port}";
}