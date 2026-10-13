using DotNetEnv;
using JobPortalApp.Api.Extensions;
using JobPortalApp.Repository.Extensions;
using JobPortalApp.Service;
using JobPortalApp.Service.Extensions;
using JobPortalApp.Shared.Extensions;
using JobPortalApp.Shared.Responses;
using JobPortalApp.Shared.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

Env.TraversePath().NoClobber().Load(Path.Combine(AppContext.BaseDirectory, ".env"));

var builder = WebApplication.CreateBuilder(args);

string[] requiredSettings =
[
    "ConnectionStrings:SqlServer",
    "ConnectionStrings:Redis",
    "ConnectionStrings:RabbitMQ",
    "TokenOption:SecurityKey",
    "CloudinarySettings:CloudName",
    "CloudinarySettings:ApiKey",
    "CloudinarySettings:ApiSecret"
];
var missingSettings = requiredSettings
    .Where(key => string.IsNullOrWhiteSpace(builder.Configuration[key]))
    .ToArray();
if (missingSettings.Length > 0)
{
    throw new InvalidOperationException(
        $"Missing configuration: {string.Join(", ", missingSettings)}. " +
        "Copy .env.example to .env in the solution root and fill in the values.");
}

builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(entry => entry.Errors)
            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Invalid request." : error.ErrorMessage)
            .Distinct()
            .ToList();

        return new BadRequestObjectResult(ServiceResult.Fail(errors));
    };
});

builder.Services.AddRepositoryExtensions(builder.Configuration)
    .AddServiceExtensions(typeof(ServiceAssembly),builder.Configuration)
    .AddSharedExtension(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddVersioningExtension();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

var apiVersionSet = app.AddVersionSetExtension();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.InitializeDatabaseAsync();

app.Run();
