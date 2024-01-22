using Google.Protobuf.WellKnownTypes;
using GrpcServer.Models;
using GrpcServer.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder
.Services.AddGrpc()
.Services.AddControllers()
.Services
.AddDbContext<UserTaskContext>(opt => opt.UseInMemoryDatabase(nameof(UserTask)))
.AddDbContext<MatrixContext>(opt => opt.UseInMemoryDatabase(nameof(Matrix)));
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TheOnlyService>();
app.MapControllers();


app.Run();
