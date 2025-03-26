using JobPortalApp.Repository.Extensions;
using JobPortalApp.Service;
using JobPortalApp.Service.Extensions;
using JobPortalApp.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRepositoryExtensions(builder.Configuration)
    .AddServiceExtensions(typeof(ServiceAssembly))
    .AddSharedExtension(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
