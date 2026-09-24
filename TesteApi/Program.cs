using TesteApi.Interfaces;
using TesteApi.Repository;
using static Dietcode.Database.Orm.Builder;

var builder = WebApplication.CreateBuilder(args);

//Secrets
builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

BuilderStart(builder.Services);

builder.Services.AddScoped<IBancoRepository, BancoRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
