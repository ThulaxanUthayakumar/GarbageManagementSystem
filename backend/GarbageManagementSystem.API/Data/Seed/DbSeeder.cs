using GarbageManagementSystem.API.Models.Entities;
using GarbageManagementSystem.API.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GarbageManagementSystem.API.Data.Seed;

/// <summary>
/// Populates the database with roles, a default admin, sample residents and
/// sample transactional data so the app is usable and demo-able immediately
/// after the first migration. Every step checks "does this already exist?"
/// first, so it is safe to run on every application startup.
/// </summary>
public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await SeedRolesAsync(roleManager);
        await SeedAdminAsync(userManager);
        var residents = await SeedResidentsAsync(context, userManager);
        var categories = await SeedWasteCategoriesAsync(context);
        await SeedCollectionRequestsAsync(context, residents, categories);
        await SeedSchedulesAsync(context, categories);
        await SeedComplaintsAsync(context, residents);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in UserRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "[email protected]";

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "System Administrator",
            EmailConfirmed = true,
            PhoneNumber = "+94770000000",
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(admin, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, UserRoles.Admin);
        }
    }

    private static async Task<List<Resident>> SeedResidentsAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        if (await context.Residents.AnyAsync())
        {
            return await context.Residents.ToListAsync();
        }

        var seedData = new[]
        {
            new { FullName = "Aarav Sharma", Email = "[email protected]", Phone = "+94771234501", Address = "12 Palm Grove Street", City = "Colombo", State = "Western", Zip = "00100", Zone = "Zone A" },
            new { FullName = "Meera Fernando", Email = "[email protected]", Phone = "+94771234502", Address = "45 Lakeview Road", City = "Colombo", State = "Western", Zip = "00200", Zone = "Zone B" },
            new { FullName = "Kavi Perera", Email = "[email protected]", Phone = "+94771234503", Address = "8 Hillcrest Avenue", City = "Colombo", State = "Western", Zip = "00300", Zone = "Zone A" },
            new { FullName = "Nadia Silva", Email = "[email protected]", Phone = "+94771234504", Address = "21 Garden Lane", City = "Colombo", State = "Western", Zip = "00400", Zone = "Zone C" },
            new { FullName = "Ruwan De Zoysa", Email = "[email protected]", Phone = "+94771234505", Address = "3 Ocean Terrace", City = "Colombo", State = "Western", Zip = "00500", Zone = "Zone B" }
        };

        var residents = new List<Resident>();

        foreach (var seed in seedData)
        {
            var user = new ApplicationUser
            {
                UserName = seed.Email,
                Email = seed.Email,
                FullName = seed.FullName,
                PhoneNumber = seed.Phone,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, "Resident@123");
            if (!result.Succeeded)
            {
                continue;
            }

            await userManager.AddToRoleAsync(user, UserRoles.Resident);

            var resident = new Resident
            {
                ApplicationUserId = user.Id,
                Address = seed.Address,
                City = seed.City,
                State = seed.State,
                ZipCode = seed.Zip,
                Zone = seed.Zone,
                IsActive = true,
                DateJoined = DateTime.UtcNow.AddDays(-Random.Shared.Next(30, 240))
            };

            context.Residents.Add(resident);
            residents.Add(resident);
        }

        await context.SaveChangesAsync();
        return residents;
    }

    private static async Task<List<WasteCategory>> SeedWasteCategoriesAsync(ApplicationDbContext context)
    {
        if (await context.WasteCategories.AnyAsync())
        {
            return await context.WasteCategories.ToListAsync();
        }

        var categories = new List<WasteCategory>
        {
            new() { Name = "Plastic", Description = "Bottles, containers, packaging and other plastic waste.", IconName = "recycling" },
            new() { Name = "Paper", Description = "Newspapers, cardboard, office paper and cartons.", IconName = "description" },
            new() { Name = "Glass", Description = "Glass bottles, jars and broken glassware.", IconName = "liquor" },
            new() { Name = "Organic", Description = "Food scraps, garden waste and other biodegradable material.", IconName = "compost" },
            new() { Name = "Metal", Description = "Cans, scrap metal and other metallic waste.", IconName = "hardware" },
            new() { Name = "Electronic Waste", Description = "Old electronics, batteries and small appliances.", IconName = "devices" }
        };

        context.WasteCategories.AddRange(categories);
        await context.SaveChangesAsync();
        return categories;
    }

    private static async Task SeedCollectionRequestsAsync(ApplicationDbContext context, List<Resident> residents, List<WasteCategory> categories)
    {
        if (await context.CollectionRequests.AnyAsync() || residents.Count == 0 || categories.Count == 0)
        {
            return;
        }

        var statuses = new[] { RequestStatus.Pending, RequestStatus.InProgress, RequestStatus.Completed, RequestStatus.Cancelled };
        var requests = new List<CollectionRequest>();

        for (var i = 0; i < 18; i++)
        {
            var resident = residents[i % residents.Count];
            var category = categories[i % categories.Count];
            var status = statuses[i % statuses.Length];
            var createdAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 60));

            requests.Add(new CollectionRequest
            {
                ResidentId = resident.Id,
                WasteCategoryId = category.Id,
                Description = $"Please collect {category.Name.ToLowerInvariant()} waste from my residence.",
                PickupDate = createdAt.AddDays(Random.Shared.Next(1, 5)),
                Status = status,
                CreatedAt = createdAt,
                CompletedDate = status == RequestStatus.Completed ? createdAt.AddDays(Random.Shared.Next(1, 4)) : null
            });
        }

        context.CollectionRequests.AddRange(requests);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSchedulesAsync(ApplicationDbContext context, List<WasteCategory> categories)
    {
        if (await context.CollectionSchedules.AnyAsync())
        {
            return;
        }

        var zones = new[] { "Zone A", "Zone B", "Zone C" };
        var schedules = new List<CollectionSchedule>();

        for (var i = 0; i < 9; i++)
        {
            schedules.Add(new CollectionSchedule
            {
                Zone = zones[i % zones.Length],
                WasteCategoryId = categories.Count > 0 ? categories[i % categories.Count].Id : null,
                ScheduledDate = DateTime.UtcNow.Date.AddDays((i - 3) * 3),
                ScheduledTime = "08:00 AM - 11:00 AM",
                CollectorName = i % 2 == 0 ? "GreenCycle Waste Services" : "City Sanitation Crew",
                Status = i < 3 ? ScheduleStatus.Completed : ScheduleStatus.Scheduled,
                Notes = "Please place bins outside the gate by 7:30 AM."
            });
        }

        context.CollectionSchedules.AddRange(schedules);
        await context.SaveChangesAsync();
    }

    private static async Task SeedComplaintsAsync(ApplicationDbContext context, List<Resident> residents)
    {
        if (await context.Complaints.AnyAsync() || residents.Count == 0)
        {
            return;
        }

        var statuses = new[] { ComplaintStatus.Open, ComplaintStatus.InProgress, ComplaintStatus.Resolved, ComplaintStatus.Closed };
        var subjects = new[]
        {
            "Missed pickup on scheduled day",
            "Overflowing community bin",
            "Damaged bin needs replacement",
            "Collection truck arrives too early",
            "Waste left on the street after collection",
            "Wrong waste category collected"
        };

        var complaints = new List<Complaint>();

        for (var i = 0; i < subjects.Length; i++)
        {
            var resident = residents[i % residents.Count];
            var status = statuses[i % statuses.Length];
            var createdAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 25));
            var isResolved = status is ComplaintStatus.Resolved or ComplaintStatus.Closed;

            complaints.Add(new Complaint
            {
                ResidentId = resident.Id,
                Subject = subjects[i],
                Description = $"{subjects[i]}. Please look into this as soon as possible, it has happened more than once.",
                Status = status,
                CreatedAt = createdAt,
                ResolvedDate = isResolved ? createdAt.AddDays(Random.Shared.Next(1, 5)) : null,
                AdminRemarks = isResolved ? "Issue has been addressed by the collection team." : null
            });
        }

        context.Complaints.AddRange(complaints);
        await context.SaveChangesAsync();
    }
}
