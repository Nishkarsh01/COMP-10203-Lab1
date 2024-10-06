using Lab1.Data;
using Lab1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Lab1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


            // Configure the DbContext and Identity services
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Register Identity and configure it to use ApplicationUser instead of the default IdentityUser
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                // Configure Password Requirements
                options.Password.RequireDigit = true;  // Requires at least one number (0-9)
                options.Password.RequireLowercase = true;  // Requires at least one lowercase letter
                options.Password.RequireUppercase = true;  // Requires at least one uppercase letter
                options.Password.RequireNonAlphanumeric = false;  // No need for a special character
                options.Password.RequiredLength = 6;  // Minimum password length
                options.Password.RequiredUniqueChars = 1;  // Requires at least 1 unique character

                // Sign-in options
                options.SignIn.RequireConfirmedEmail = false;  // Disable email confirmation for now
                options.SignIn.RequireConfirmedAccount = false;  // No account confirmation required

                // Lockout options (you can adjust these as needed)
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddRoles<IdentityRole>() // Add role support
            .AddEntityFrameworkStores<ApplicationDbContext>(); // Configure EF with ApplicationDbContext

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            var configuraton = app.Services.GetService<IConfiguration>();

            var hosting = app.Services.GetService<IWebHostEnvironment>();

            var secrets = configuraton.GetSection("Secrets").Get<AppSecrets>();

            DbInitializer.appSecrets = secrets;

            // Seed database with roles and users
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    // Log before seeding
                    logger.LogInformation("Seeding database with roles and users...");

                    // Call the seed method
                    await DbInitializer.InitializeAsync(userManager, roleManager);

                    // Log after successful seeding
                    logger.LogInformation("Database seeding completed successfully.");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
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
            app.MapRazorPages();

            app.Run();
        }
    }
}
