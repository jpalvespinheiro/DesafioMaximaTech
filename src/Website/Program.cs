using Adm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);
var appsettings = JsonConvert.DeserializeObject<JObject>(File.ReadAllText("appsettings.json"))!;
var apiuri = new Uri(appsettings["ApiAddress"]!.Value<string>()!);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IHttpClientFactory>(new HttpClientFactory(apiuri));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run("https://localhost:7035");
