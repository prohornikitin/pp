using Server.Models;
using Server.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder
.Services.AddGrpc()
.Services.AddControllers()
.Services.AddDbContext<TheOnlyDbContext>(opt => opt.UseInMemoryDatabase("db"));
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ComputingNodeService>();
app.MapControllers();


app.Run();
