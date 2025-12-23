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
    public async Task<IEnumerable<Employee>> GetTeamMembersAsync(int managerId)
    {
        return await _db.Employees
            .Where(e => e.ManagerId == managerId && e.IsActive)
            .OrderBy(e => e.Nom)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _db.Employees.CountAsync(e => e.IsActive);
    }

    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _db.Employees
            .Include(e => e.Manager)
            .Where(e => e.IsActive)
            .OrderBy(e => e.Nom)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesWithLowBalanceAsync(int threshold)
    {
        return await _db.Employees
            .Where(e => e.IsActive && e.SoldeConges < threshold)
            .OrderBy(e => e.SoldeConges)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetEmployeesWithoutRecentLeaveAsync(int year)
    {
        // This is complex in pure EF without joining Leaves table manually or navigation property usage
        // Assuming we want employees who didn't take leave in `year`.
        // Inverse logic: Get all employees, filter locally or subquery.
        // Subquery approach:
        return await _db.Employees
            .Where(e => e.IsActive && !_db.Leaves.Any(l => l.EmployeeId == e.Id 
                                                         && l.Status == "Approved" 
                                                         && l.StartDate.Year == year))
            .ToListAsync();
    }
    public async Task UpdateAsync(Employee employee)
    {
        _db.Employees.Update(employee);
        await _db.SaveChangesAsync();
    }
}
