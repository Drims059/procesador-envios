using Microsoft.EntityFrameworkCore;
using ProcesadorEnvios.Models;
using ProcesadorEnvios.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//Descomentar las 2 lineas de abajo para usar base de datos en memoria
// builder.Services.AddDbContext<EnviosContext>(opt => opt.UseInMemoryDatabase("ListaEnvios"));
// builder.Services.AddDbContext<OperadoresContext>(opt => opt.UseInMemoryDatabase("ListaOperadores"));
// builder.Services.AddDbContext<SuscriptoresWebhookContext>(opt => opt.UseInMemoryDatabase("ListaSuscriptores"));

//Tambien comentar esta parte para poder usar la base de datos en memoria
builder.Services.AddDbContext<EnviosContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddDbContext<OperadoresContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddDbContext<SuscriptoresWebhookContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configuracion para Validar Autenticacion.
builder.Services.AddAuthentication(options  =>
{
	options.DefaultAuthenticateScheme  =  JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme  =  JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options  =>
{
	options.Authority = "https://dev-pmt16h97.us.auth0.com/";
    options.Audience = "https://api.procesadorenvios.com";
});
//Configuracion para Validar Autorización. 
builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("read:procesadorenvios", policy => policy.Requirements.Add(new HasScopeRequirement("read:procesadorenvios", "https://dev-pmt16h97.us.auth0.com/")));
        options.AddPolicy("write:procesadorenvios", policy => policy.Requirements.Add(new HasScopeRequirement("write:procesadorenvios", "https://dev-pmt16h97.us.auth0.com/")));
    });
    
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

var app = builder.Build();

app.UseRouting();

//Habilitamos Autenticacion.
app.UseAuthentication();


//Habilitamos Autorización. 
app.UseAuthorization();



app.UseMiddleware<ErrorHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/internal/swagger.json", "v1");
    });
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/internal/swagger.json", async context =>
        {
            await context.Response.WriteAsync(await File.ReadAllTextAsync("openapi.json"));
        });
    });
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
