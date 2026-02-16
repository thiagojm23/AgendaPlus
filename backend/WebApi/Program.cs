using System.Data;
using System.Text;
using AgendaPlus.Application;
using AgendaPlus.Application.Common.Behaviors;
using AgendaPlus.Application.Common.Interfaces;
using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Application.QueryBuilders;
using AgendaPlus.Infrastructure;
using AgendaPlus.Infrastructure.Services;
using AgendaPlus.Infrastructure.Settings;
using AgendaPlus.WebApi.Consumers;
using AgendaPlus.WebApi.Middlewares;
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

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AgendaPlus API", Version = "v1" });
    
    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention());

// Registrar IApplicationDbContext
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

// Registrar IDbConnection para Dapper
builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connectionString));

// Registrar IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Registrar ICurrentUserService
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Registrar serviços de autenticação e token
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Registrar Query Builders
builder.Services.AddScoped<UserQueryBuilder>();
builder.Services.AddScoped<AuthTokenQueryBuilder>();
builder.Services.AddScoped<OutboxMessageQueryBuilder>();
builder.Services.AddScoped<TenantQueryBuilder>();
builder.Services.AddScoped<ResourceQueryBuilder>();
builder.Services.AddScoped<BookingQueryBuilder>();
builder.Services.AddScoped<AvailabilityPatternsQueryBuilder>();
builder.Services.AddScoped<AvailabilityExceptionsQueryBuilder>();

// Registrar Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Configurar Settings
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Configurar MassTransit com RabbitMQ
var rabbitMqSettings = builder.Configuration.GetSection("RabbitMQSettings").Get<RabbitMqSettings>();
builder.Services.AddMassTransit(x =>
{
    // Registrar consumers
    x.AddConsumer<ForgotPasswordEmailConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqSettings!.HostName, (ushort)rabbitMqSettings.Port, "/", h =>
        {
            h.Username(rabbitMqSettings.Username);
            h.Password(rabbitMqSettings.Password);
        });

        // Configurar endpoints dos consumers (MassTransit cria filas automaticamente)
        // cfg.ConfigureEndpoints(context);

        cfg.ReceiveEndpoint("forgot-password-email", e =>
        {
            /*
            Remove a exchange intermediária
            e.ConfigureConsumeTopology = false;

            Faz bind direto da mensagem para a fila
            e. Bind<ForgotPasswordEmailMessage>();
            */

            e.ConfigureConsumer<ForgotPasswordEmailConsumer>(context);
        });
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
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgendaPlus API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();