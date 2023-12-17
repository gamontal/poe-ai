using SnapPoet.Clients;
using SnapPoet.Models.Configurations;
using SnapPoet.Services;
using SnapPoet.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions<AzureAIConfig>()
	.Configure<IConfiguration>((settings, configuration) => configuration.GetSection("AzureAI").Bind(settings));

builder.Services.AddOptions<OpenAIConfig>()
	.Configure<IConfiguration>((settings, configuration) => configuration.GetSection("OpenAI").Bind(settings));

builder.Services.AddHttpClient<ComputerVisionClient>();
builder.Services.AddHttpClient<OpenAIClient>();

builder.Services.AddSingleton<IImageProcessorService, ImageProcessorService>();
builder.Services.AddSingleton<IPoemGeneratorService, PoemGeneratorService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html"); ;

app.Run();
