using ITI_Project.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;

namespace ITI_Project.Data
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            context.Database.Migrate();

            // ---- Departments (add only if missing by name) ----
            var departmentNames = new[]
            {
                "Information Technology", "Human Resources",
                "Accounting", "Sales", "Marketing"
            };
            foreach (var name in departmentNames)
            {
                if (!context.Departments.Any(d => d.Name == name))
                {
                    context.Departments.Add(new Department { Name = name });
                }
            }
            context.SaveChanges();

            // ---- Job Titles (add only if missing by title) ----
            var jobTitles = new[]
            {
                "Software Developer", "Systems Analyst", "HR Manager",
                "Accountant", "Sales Representative", "Marketing Specialist"
            };
            foreach (var title in jobTitles)
            {
                if (!context.JobTitles.Any(j => j.Title == title))
                {
                    context.JobTitles.Add(new JobTitle { Title = title });
                }
            }
            context.SaveChanges();

            // ---- Employees (Arabic names written in English, salaries in EGP) ----
            var employees = new (string Name, string Email, string Phone, DateTime Hire, decimal Salary, string Dept, string Job)[]
            {
                ("Mohamed Ahmed",      "mohamed.ahmed@company.com",   "+20 100 123 4567", new DateTime(2023, 1, 15), 12000m, "Information Technology", "Software Developer"),
                ("Fatma Ali",         "fatma.ali@company.com",        "+20 111 234 5678", new DateTime(2022, 3, 10),  9500m,  "Human Resources",        "HR Manager"),
                ("Ahmed Hassan",      "ahmed.hassan@company.com",     "+20 122 345 6789", new DateTime(2023, 6, 1),  11000m, "Information Technology", "Systems Analyst"),
                ("Mariam Ibrahim",    "mariam.ibrahim@company.com",   "+20 133 456 7890", new DateTime(2021, 9, 20),  8000m,  "Accounting",             "Accountant"),
                ("Omar Khaled",       "omar.khaled@company.com",      "+20 144 567 8901", new DateTime(2024, 2, 5),   7000m,  "Sales",                  "Sales Representative"),
                ("Sara Mahmoud",      "sara.mahmoud@company.com",     "+20 155 678 9012", new DateTime(2022, 11, 12), 8500m,  "Marketing",              "Marketing Specialist"),
                ("Youssef Sameh",     "youssef.sameh@company.com",    "+20 166 789 0123", new DateTime(2023, 4, 18), 12500m, "Information Technology", "Software Developer"),
                ("Nour Adel",         "nour.adel@company.com",        "+20 177 890 1234", new DateTime(2021, 7, 30),  9000m,  "Human Resources",        "HR Manager"),
                ("Hana Tarek",        "hana.tarek@company.com",       "+20 188 901 2345", new DateTime(2024, 1, 22),  8200m,  "Accounting",             "Accountant"),
                ("Karim Fady",        "karim.fady@company.com",       "+20 199 012 3456", new DateTime(2023, 8, 14),  7500m,  "Sales",                  "Sales Representative"),
            };

            var employeesFolder = Path.Combine(env.WebRootPath, "images", "employees");
            Directory.CreateDirectory(employeesFolder);

            foreach (var e in employees)
            {
                if (context.Employees.Any(x => x.Email == e.Email))
                {
                    continue;
                }

                var dept = context.Departments.First(d => d.Name == e.Dept);
                var job = context.JobTitles.First(j => j.Title == e.Job);

                var employee = new Employee
                {
                    FullName = e.Name,
                    Email = e.Email,
                    PhoneNumber = e.Phone,
                    HireDate = e.Hire,
                    Salary = e.Salary,
                    DepartmentId = dept.Id,
                    JobTitleId = job.Id,
                };

                context.Employees.Add(employee);
                context.SaveChanges();

                var fileName = $"emp{employee.Id}.svg";
                var filePath = Path.Combine(employeesFolder, fileName);
                GenerateAvatar(filePath, e.Name);
                employee.ProfileImagePath = "/images/employees/" + fileName;
                context.Employees.Update(employee);
                context.SaveChanges();
            }
        }

        private static void GenerateAvatar(string filePath, string name)
        {
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var initials = (parts.Length > 0 ? char.ToUpperInvariant(parts[0][0]) : '?').ToString()
                         + (parts.Length > 1 ? char.ToUpperInvariant(parts[1][0]).ToString() : "");

            var colors = new[]
            {
                "#4e73df", "#1cc88a", "#36b9cc", "#f6c23e",
                "#e74a3b", "#6f42c1", "#20c997", "#fd7e14"
            };

            var hash = 0;
            foreach (var c in name)
            {
                hash = (hash * 31 + c) % 1000003;
            }
            var color = colors[Math.Abs(hash) % colors.Length];

            var svg = $@"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 120 120' width='120' height='120'>
  <rect width='120' height='120' fill='{color}'/>
  <text x='50%' y='50%' dy='.35em' text-anchor='middle' font-family='Nunito, Segoe UI, Arial, sans-serif' font-size='46' font-weight='700' fill='#ffffff'>{initials}</text>
</svg>";
            File.WriteAllText(filePath, svg);
        }
    }
}
