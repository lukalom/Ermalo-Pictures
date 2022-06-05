using BackgroundProcessing.Extensions;
using EP.Infrastructure.Extensions;
using Serilog;
using Serilog.Events;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddSerilog(new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/Information.txt", LogEventLevel.Information, rollingInterval: RollingInterval.Day)
    .WriteTo.File("Logs/Error.txt", LogEventLevel.Error, rollingInterval: RollingInterval.Day)
    .CreateLogger());

builder.Services.AddRazorPages();
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.ConfigurationServices(builder.Configuration);
builder.Services.BackgroundProcessServiceExtensions();
builder.Services.AddGenericRepository();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeConfig:SecretKey").Get<string>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

await app.RunAsync();
