using API.Interfaces;
using API.Services;
using Azure.Communication.Email;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o =>
{
    o.AddPolicy("CorsPolicy", p =>
    {
        if (builder.Environment.IsDevelopment())
            p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        else
            p.WithOrigins("https://authservice8-fvgjaehwh5f8d9dq.swedencentral-01.azurewebsites.net")
             .AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddMemoryCache();
builder.Services.AddSingleton(_ => new EmailClient(builder.Configuration["ACS:ConnectionString"]));

builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Email Service Provider API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.MapControllers();
app.Run();
