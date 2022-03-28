using BeeLive.Core.Repositories;
using BeeLive.NoiseData.Modules;
using BeeLive.Persistence;
using BeeLive.Persistence.Repositories;
using Microsoft.Extensions.Hosting.WindowsServices;
using NLog;
using NLog.Extensions.Logging;

var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};

var builder = WebApplication.CreateBuilder(options);

builder.Host.UseWindowsService();
// Add services to the container.
builder.Services.AddControllers();
//private static Logger logger = LogManager.GetCurrentClassLogger();

var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables()
            .Build();

LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
//NLog.Config.LoggingConfiguration nlogConfig = new NLogLoggingConfiguration(config.GetSection("NLog"));
//LogManager.LoadConfiguration(System.String.Concat(Directory.GetCurrentDirectory(), "\\nlog.config"));
//builder.Host.UseWindowsService();
var serviceProviderSingle = new ServiceCollection()
           .AddLogging(builder => { builder.AddConsole(); builder.AddNLog(); })
           .BuildServiceProvider();

builder.Services.Configure<ElasticSearchSettings>(x => config.GetSection("Elastic").Bind(x));

builder.Services.AddSingleton<ElasticSearchContext>();

builder.Services.AddNoiseData();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.UseStaticFiles();

app.Run();