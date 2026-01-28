using Infotecs2026;
using Infotecs2026.Share.Application.Interfaces;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();


    builder.Services.AddScoped<IApplicationDbContext, ApplicationContext>();
}

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();