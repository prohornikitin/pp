using Server.Models;
using Server.GrpcServices;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Server.Other;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder
.Services.AddGrpc()
.Services.AddControllers()
.AddJsonOptions(options => {
    options
        .JsonSerializerOptions
        .Converters
        .Add(new JsonStringEnumConverter(new PascalCaseJsonNamingPolicy(), allowIntegerValues: false));
})
.Services.AddDbContext<TheOnlyDbContext>(opt => opt.UseInMemoryDatabase("db"))
.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = null;
})
.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = null;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ComputingNodeService>();
app.MapControllers();



app.Run();
