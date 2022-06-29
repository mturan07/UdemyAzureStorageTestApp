using AzureStorageLibrary;
using AzureStorageLibrary.Services;
using MvcWebApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

ConnectionStrings.AzureConnectionString = builder.Configuration.GetSection("AzureConnectionStrings")["StorageConStr"];
builder.Services.AddScoped(typeof(INoSqlStorage<>), typeof(TableStorage<>));
builder.Services.AddSingleton<IBlobStorage, BlobStorage>();
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("https://localhost:7216/")
                .AllowAnyHeader()
                .WithMethods("GET", "POST")
                .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseCors();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<NotificationHub>("/NotificationHub");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
