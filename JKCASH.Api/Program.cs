using JKCASH.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using JKCASH.Application.Interfaces;
using JKCASH.Infrastructure.Repositorios;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string conexao = builder.Configuration
    .GetConnectionString("JKCASH")
    ?? throw new InvalidOperationException(
        "A conexão JKCASH não foi configurada");

builder.Services.AddDbContextFactory<JKCASHDbContext>(
    opcoes => opcoes.UseSqlServer(conexao));

builder.Services.AddScoped<IContaRepositorio>(servicos =>
{
    var fabrica = servicos
    .GetRequiredService<IDbContextFactory<JKCASHDbContext>>();
    return new ContaRepositorioEf(fabrica.CreateDbContext);
});

builder.Services.AddControllers();
builder.Services.AddSingleton(new JKCASH.Infrastructure.Backup.BackupRepositorioSql(conexao));
builder.Services.AddSingleton(new JKCASH.Infrastructure.Backup.ExportacaoSql(conexao));
builder.Services.AddProblemDetails();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
// Nesta versão o servidor atende somente esta máquina. Acesso em rede exige autenticação.
app.Use(async (contexto, proximo) =>
{
    var endereco = contexto.Connection.RemoteIpAddress;
    if (endereco is null || !System.Net.IPAddress.IsLoopback(endereco))
    {
        contexto.Response.StatusCode = StatusCodes.Status403Forbidden;
        return;
    }
    await proximo();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
