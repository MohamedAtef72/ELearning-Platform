using System;
using Learning_World.Data;
using Learning_World.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddScoped<CoursesRepository, CoursesRepository>();
builder.Services.AddScoped<MyLearningRepository, MyLearningRepository>();
builder.Services.AddScoped<EnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<UserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<LearnRepository, LearnRepository>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddIdentity<User, IdentityRole<int>>(
               options =>
               {
                   options.Password.RequireDigit = true;
                   options.Password.RequireLowercase = true;
                   options.Password.RequireUppercase = true;
                   options.Password.RequireNonAlphanumeric = true;
                   options.Password.RequiredLength = 4;
               }
               ).AddEntityFrameworkStores<ElearningPlatformContext>()
               .AddDefaultTokenProviders();

builder.Services.AddDbContext<ElearningPlatformContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString")));

builder.Services.AddScoped<LearnRepository, LearnRepository>();

var app = builder.Build();

app.UseExceptionHandler("/Home/Error");

app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // Default route
    _ = endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Main}/{action=Index}/{id?}");

    // Module route for rendering the full page
    _ = endpoints.MapControllerRoute(
        name: "module",
        pattern: "Learn/index/{id}/module/{moduleId}",
        defaults: new { controller = "Learn", action = "Index" });

    // Partial view route for AJAX requests (lesson content)
    _ = endpoints.MapControllerRoute(
        name: "modulePartial",
        pattern: "Learn/PartsPartialView/{moduleId}",
        defaults: new { controller = "Learn", action = "PartsPartialView" });

    _ = endpoints.MapControllerRoute(
        name: "lessonDisplayPartial",
        pattern: "Learn/lesson/{moduleId}/{lessonType}/{lessonId}",
        defaults: new { controller = "Learn", action = "LessonsPartialView" });
});
app.Run();
