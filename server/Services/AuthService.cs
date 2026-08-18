using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Models;

namespace server.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (existingUser)
        {
            return null;
        }

        var technicianRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == "Technician");

        if (technicianRole == null)
        {
            return null;
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = technicianRole.RoleId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }
}
