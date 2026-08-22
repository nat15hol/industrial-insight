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

        var technicianRole = new Role { Name = "Technician" };
        var managerRole = new Role { Name = "Manager" };

        context.Roles.AddRange(technicianRole, managerRole);
        context.SaveChanges();

        var location = new Location
        {
            Name = "Main Factory",
            Address = "Industrigatan 1, Malmö"
        };

        context.Locations.Add(location);
        context.SaveChanges();

        var managerUser = new User
        {
            Name = "Manager User",
            Email = "manager@industrialinsight.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
            RoleId = managerRole.RoleId
        };

        var technicianUser = new User
        {
            Name = "Test Technician",
            Email = "tech@industrialinsight.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Technician123!"),
            RoleId = technicianRole.RoleId
        };

        context.Users.AddRange(managerUser, technicianUser);
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
