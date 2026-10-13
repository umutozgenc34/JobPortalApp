using DotNetEnv;
using JobPortalApp.Repository.Extensions;
using JobPortalApp.Service;
using JobPortalApp.Service.Extensions;
using JobPortalApp.Shared.Extensions;
using JobPortalApp.Shared.Versioning;


Env.TraversePath().NoClobber().Load();

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

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddRepositoryExtensions(builder.Configuration)
    .AddServiceExtensions(typeof(ServiceAssembly),builder.Configuration)
    .AddSharedExtension(builder.Configuration);

builder.Services.AddVersioningExtension();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var apiVersionSet = app.AddVersionSetExtension();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
