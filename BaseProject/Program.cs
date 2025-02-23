using BaseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaseProject;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbConnection")));

        builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<AppIdentityDbContext>();

        var app = builder.Build();

        using var scope = app.Services.CreateScope();

        var identityDbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        identityDbContext.Database.Migrate();

        if (!userManager.Users.Any())
        {
            userManager.CreateAsync(new AppUser { UserName = "user1", Email = "user1@gmail.com" }, "Pass123!").Wait();
            userManager.CreateAsync(new AppUser { UserName = "user2", Email = "user2@gmail.com" }, "Pass123*").Wait();
            userManager.CreateAsync(new AppUser { UserName = "user3", Email = "user3@gmail.com" }, "Pass12*").Wait();
            userManager.CreateAsync(new AppUser { UserName = "user4", Email = "user4@gmail.com" }, "Pass1*-").Wait();
            userManager.CreateAsync(new AppUser { UserName = "user5", Email = "user5@gmail.com" }, "Password12*").Wait();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
