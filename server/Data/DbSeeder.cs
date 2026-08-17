using server.Models;

namespace server.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Roles.Any() || context.Users.Any() || context.Locations.Any())
        {
            // Databasen har redan data - hoppa över seedning
            return;
        }

        var adminRole = new Role { Name = "Admin" };
        var technicianRole = new Role { Name = "Technician" };
        var viewerRole = new Role { Name = "Viewer" };

        context.Roles.AddRange(adminRole, technicianRole, viewerRole);
        context.SaveChanges();

        var location = new Location
        {
            Name = "Main Factory",
            Address = "Industrigatan 1, Malmö"
        };

        context.Locations.Add(location);
        context.SaveChanges();

        var adminUser = new User
        {
            Name = "Admin User",
            Email = "admin@industrialinsight.local",
            PasswordHash = "placeholder-hash",
            RoleId = adminRole.RoleId
        };

        var technicianUser = new User
        {
            Name = "Test Technician",
            Email = "tech@industrialinsight.local",
            PasswordHash = "placeholder-hash",
            RoleId = technicianRole.RoleId
        };

        context.Users.AddRange(adminUser, technicianUser);
        context.SaveChanges();

        var machine = new Machine
        {
            Name = "CNC-01",
            Status = "Operational",
            Runtime = 0,
            LocationId = location.LocationId
        };

        context.Machines.Add(machine);
        context.SaveChanges();
    }
}
