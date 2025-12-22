using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Data;
using PlateformeRHCollaborative.Web.Models;

namespace PlateformeRHCollaborative.Web.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _db;

    public EmployeeRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _db.Employees
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee?> GetByUserIdAsync(string userId)
    {
        return await _db.Employees
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.UserId == userId);
    }

    public async Task<int> GetTeamSizeAsync(int managerId)
    {
        return await _db.Employees
            .CountAsync(e => e.ManagerId == managerId && e.IsActive);
    }

    public async Task<int> ResetAllBalancesAsync(int amount)
    {
        // ExecuteRawSql est plus performant pour une mise à jour de masse
        return await _db.Database.ExecuteSqlRawAsync("UPDATE Employees SET SoldeConges = {0}", amount);
    }
}
