using EP.API.Extensions;
using EP.API.Middleware;
using EP.Application.Extensions;
using EP.Infrastructure.Extensions;
using EP.Shared.Extensions;
using Serilog;
using Serilog.Events;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Logging.AddSerilog(new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/Information.txt", LogEventLevel.Information, rollingInterval: RollingInterval.Day)
    .WriteTo.File("Logs/Error.txt", LogEventLevel.Error, rollingInterval: RollingInterval.Day)
    .CreateLogger());

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.ConfigurationServices(builder.Configuration, builder.Environment);
builder.Services.AddAuthorizationConfiguration();
builder.Services.ApiServicesCollection();
builder.Services.ApplicationServiceExtensions();
builder.Services.AddGenericRepository();

builder.Services.AddSwaggerConfiguration();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeConfig:SecretKey").Get<string>();
builder.Services.AddSeederService();

var app = builder.Build();
await app.Seed(app.Services.CreateScope().ServiceProvider);
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAPI");
        c.InjectStylesheet("/swagger-ui/SwaggerDark.css");
    });
}
app.UseStaticFiles();
app.UseLoggerMiddleware();
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<EmailConfirmedMiddleware>();

app.MapControllers();

await app.RunAsync();