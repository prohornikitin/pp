using Server.Models;
using Server.GrpcServices;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Server.Other;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder
.Services.AddGrpc()
.Services.AddControllers()
.AddJsonOptions(options => {
    options
        .JsonSerializerOptions
        .Converters
        .Add(new JsonStringEnumConverter(new UpperCamelCaseJsonNamingPolicy(), allowIntegerValues: false));
})
.Services.AddDbContext<TheOnlyDbContext>(opt => opt.UseInMemoryDatabase("db"));
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ComputingNodeService>();
app.MapControllers();


app.Run();
