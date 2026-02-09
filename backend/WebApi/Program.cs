using System.Data;
using System.Text;
using AgendaPlus.Application;
using AgendaPlus.Application.Common.Behaviors;
using AgendaPlus.Application.Interfaces.Repositories;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Infrastructure;
using AgendaPlus.Infrastructure.Repositories;
using AgendaPlus.Infrastructure.Services;
using AgendaPlus.Infrastructure.Settings;
using AgendaPlus.WebApi.Consumers;
using AgendaPlus.WebApi.Workers;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention());

// Registrar IDbConnection para Dapper
builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));

// Registrar IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Registrar ICurrentUserService
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Registrar serviços de autenticação e token
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Registrar Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();

// Registrar Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Configurar Settings
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Configurar MassTransit com RabbitMQ
var rabbitMQSettings = builder.Configuration.GetSection("RabbitMQSettings").Get<RabbitMQSettings>();
builder.Services.AddMassTransit(x =>
{
    // Registrar consumers
    x.AddConsumer<ForgotPasswordEmailConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMQSettings!.HostName, (ushort)rabbitMQSettings.Port, "/", h =>
        {
            h.Username(rabbitMQSettings.Username);
            h.Password(rabbitMQSettings.Password);
        });

        // Configurar endpoints dos consumers (MassTransit cria filas automaticamente)
        cfg.ConfigureEndpoints(context);
    });
});

// Registrar Background Worker apenas para processar Outbox
builder.Services.AddHostedService<OutboxProcessorWorker>();

builder.Services.AddValidatorsFromAssembly(typeof(AssemblyReference).Assembly);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AssemblyReference).Assembly);

    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ??
                                                            throw new InvalidOperationException()))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["accessToken"];
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();