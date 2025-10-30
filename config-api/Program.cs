using config_api.Dtos;
using config_api.Mappers;
using config_api.Validador;
using config_domain;
using config_infraestructura;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Database connection
builder.Services.AddDbContext<Contexto>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection"));
});

//Repositorio
builder.Services.AddScoped<IRepositorio, Repositorio>();

//AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

//Validator
builder.Services.AddScoped<IValidator<EntornoDto>, EntornoDtoValidador>();
builder.Services.AddScoped<IValidator<VariableDto>, VariableDtoValidador>();

var app = builder.Build();

// Migraciones
using (var scope = app.Services.CreateScope())
{
    Contexto context = scope.ServiceProvider.GetRequiredService<Contexto>();
    context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
