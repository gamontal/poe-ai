var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient("ComputerVisionClient", client =>
{
    client.BaseAddress = new Uri("https://vision-playground-001.cognitiveservices.azure.com/");
});

builder.Services.AddHttpClient("OpenAiClient", client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/v1/");
});

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
