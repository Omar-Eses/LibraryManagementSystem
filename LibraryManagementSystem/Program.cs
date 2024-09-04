using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<LMSContext> (opt => 
    opt.UseNpgsql(builder.Configuration.GetConnectionString("LibraryManagementSystemContext") ?? throw new InvalidOperationException("Connection string 'LibraryManagementSystemContext' not found.")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
