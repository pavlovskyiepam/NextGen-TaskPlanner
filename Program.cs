using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;
using TaskPlanner.Services;
using TaskPlanner;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Add explicit controller discovery
builder.Services.AddControllersWithViews();

// Add localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add localization services for Razor Pages with SharedResource
builder.Services.AddMvc()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });

// Register task service
builder.Services.AddScoped<ITaskService, FileTaskService>();

// Configure supported cultures
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("uk")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    
    // Add culture providers in order of preference
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add request localization middleware
app.UseRequestLocalization();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

// Add explicit routing for CultureController
app.MapControllerRoute(
    name: "culture",
    pattern: "Culture/{action=Index}/{id?}",
    defaults: new { controller = "Culture" });

app.Run();
