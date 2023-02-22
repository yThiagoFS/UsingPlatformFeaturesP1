using UsingPlatformFeaturesP1.Platform;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

//Configurando os loggings para requisições HTTP
builder.Services.AddHttpLogging(opts =>
{
    opts.LoggingFields = HttpLoggingFields.RequestMethod | HttpLoggingFields.RequestPath | HttpLoggingFields.ResponseStatusCode;
});

var servicesConfig = builder.Configuration; // usando as configurações (settings)

//builder.Services.Configure<MessageOptions>(servicesConfig.GetSection("Location"));

var servicesEnv = builder.Environment; // acessando as configurações de ambiente ao configurar serviços

var app = builder.Build();

app.UseHttpLogging();

app.UseStaticFiles();

var env = app.Environment;

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider($"{env.ContentRootPath}/Platform/staticfiles"),
    RequestPath = "/files"
});

var pipelineConfig = app.Configuration; // usando as configurações  para configurar as pipelines

var pipelineEnv= app.Environment; // usando as configurações de ambiente ao configurar pipelines

//app.UseMiddleware<LocationMiddleware>();

#region Acessando as configurações e lendo UserSecrets                  // acessando as config de ambiente ao utilizar um middleware
app.MapGet("config", async (HttpContext context, IConfiguration config, IWebHostEnvironment env) =>
{
    string defaultDebug = config["Logging:LogLevel:Default"]; // Lê o valor definido na configuração Default

    await context.Response.WriteAsync($"The config setting is: {defaultDebug}\n The env setting is: {env.EnvironmentName}");

    string wsId = config["WebService:Id"];
    string wsKey = config["WebService:Key"];

    await context.Response.WriteAsync($"\nThe WS ID Is: {wsId}\n The WS Key is: {wsKey}");

});
#endregion

app.Logger.LogDebug("Pipeline configuration starting");

app.MapGet("population/{city?}", Population.Endpoint);

app.Logger.LogDebug("Pipeline configuration completed");

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Hello World");
});

app.Run();
