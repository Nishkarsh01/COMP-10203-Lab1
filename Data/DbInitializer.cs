using Lab1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lab1.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Logging for tracking
            Console.WriteLine("Starting database seeding...");

            // Retrieve passwords securely from environment variables or configuration
            var managerPassword = Environment.GetEnvironmentVariable("ManagerPassword");
            var employeePassword = Environment.GetEnvironmentVariable("EmployeePassword");

            // Fallback if environment variables are not set (optional)
            if (string.IsNullOrEmpty(managerPassword) || string.IsNullOrEmpty(employeePassword))
            {
                throw new InvalidOperationException("Manager and Employee passwords must be set in environment variables.");
            }

            // Seed roles: Manager and Employee
            string[] roleNames = { "Manager", "Employee" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    Console.WriteLine($"Creating role: {roleName}");
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (roleResult.Succeeded)
                    {
                        Console.WriteLine($"Role {roleName} created successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Error creating role {roleName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    Console.WriteLine($"Role {roleName} already exists.");
                }
            }

            // Seed Manager user
            if (userManager.Users.All(u => u.UserName != "manager@example.com"))
            {
                var manager = new ApplicationUser
                {
                    UserName = "manager@example.com",
                    Email = "manager@example.com",
                    FirstName = "Manager",
                    LastName = "User",
                    PhoneNumber = "1234567890",
                    BirthDate = new DateTime(1980, 1, 1)
                };

                Console.WriteLine("Creating Manager user...");
                var result = await userManager.CreateAsync(manager, managerPassword);
                if (result.Succeeded)
                {
                    Console.WriteLine("Manager user created successfully.");
                    await userManager.AddToRoleAsync(manager, "Manager");
                    Console.WriteLine("Manager user added to Manager role.");
                }
                else
                {
                    Console.WriteLine($"Error creating Manager user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine("Manager user already exists.");
            }

            // Seed Employee user
            if (userManager.Users.All(u => u.UserName != "employee@example.com"))
            {
                var employee = new ApplicationUser
                {
                    UserName = "employee@example.com",
                    Email = "employee@example.com",
                    FirstName = "Employee",
                    LastName = "User",
                    PhoneNumber = "0987654321",
                    BirthDate = new DateTime(1990, 1, 1)
                };

                Console.WriteLine("Creating Employee user...");
                var result = await userManager.CreateAsync(employee, employeePassword);
                if (result.Succeeded)
                {
                    Console.WriteLine("Employee user created successfully.");
                    await userManager.AddToRoleAsync(employee, "Employee");
                    Console.WriteLine("Employee user added to Employee role.");
                }
                else
                {
                    Console.WriteLine($"Error creating Employee user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine("Employee user already exists.");
            }

            Console.WriteLine("Database seeding completed.");
        }
    }
}
