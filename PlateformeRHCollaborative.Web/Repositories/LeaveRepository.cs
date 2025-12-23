using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Data;
using PlateformeRHCollaborative.Web.Models;

namespace PlateformeRHCollaborative.Web.Repositories;

public class LeaveRepository : ILeaveRepository
{
	private readonly ApplicationDbContext _db;

	public LeaveRepository(ApplicationDbContext db)
	{
		_db = db;
	}

	public async Task<IEnumerable<Leave>> GetAllAsync()
	{
		// Charger également l'employé lié pour permettre l'affichage du nom / email
		return await _db.Leaves
			.Include(l => l.Employee)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<Leave?> GetByIdAsync(int id)
	{
		// Inclure systématiquement l'employé pour les traitements métier côté manager
		return await _db.Leaves
			.Include(l => l.Employee)
			.FirstOrDefaultAsync(l => l.Id == id);
	}

	public async Task AddAsync(Leave entity)
	{
		await _db.Leaves.AddAsync(entity);
		await _db.SaveChangesAsync();
	}

	public async Task UpdateAsync(Leave entity)
	{
		_db.Leaves.Update(entity);
		await _db.SaveChangesAsync();
	}

	public async Task DeleteAsync(int id)
	{
		var item = await _db.Leaves.FindAsync(id);
		if (item != null)
		{
			_db.Leaves.Remove(item);
			await _db.SaveChangesAsync();
		}
	}

    public async Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end)
    {
        return await _db.Leaves
            .AnyAsync(l => l.EmployeeId == employeeId 
                        && l.Status != "Rejected" 
                        && l.Status != "Cancelled"
                        && l.StartDate <= end 
                        && l.EndDate >= start);
    }

    public async Task<int> GetApprovedCountForTeamAsync(int managerId, DateTime start, DateTime end)
    {
        return await _db.Leaves
            .Include(l => l.Employee)
            .Where(l => l.Employee.ManagerId == managerId 
                        && l.Status == "Approved" 
                        && l.StartDate <= end 
                        && l.EndDate >= start)
            .Select(l => l.EmployeeId)
            .Distinct()
            .CountAsync();
    }

    public async Task<IEnumerable<Leave>> GetApprovedLeavesByEmployeeIdAsync(int employeeId)
    {
        // On récupère uniquement les congés approuvés pour l'historique du solde
        return await _db.Leaves
            .Where(l => l.EmployeeId == employeeId && l.Status == "Approved")
            .OrderByDescending(l => l.StartDate)
            .ToListAsync();
    }
    public async Task<IEnumerable<Leave>> GetLeavesByStatusAsync(string status)
    {
        return await _db.Leaves
            .Include(l => l.Employee)
            .Where(l => l.Status == status)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetPendingCountAsync(int? managerId = null)
    {
        var query = _db.Leaves.Where(l => l.Status == "Pending");
        
        if (managerId.HasValue)
        {
            // Pending requests for this manager's team
            // Assuming workflow: Employee -> Manager (Validator)
            // But we need to look at Employee.ManagerId
            query = query.Where(l => l.Employee.ManagerId == managerId.Value);
        }

        return await query.CountAsync();
    }

    public async Task<int> GetApprovedCountAsync()
    {
        return await _db.Leaves.CountAsync(l => l.Status == "Approved");
    }

    public async Task<int> GetTotalDaysTakenAsync()
    {
        // Approximation: Sum of durations is complex with business days logic in SQL
        // Returning count of requests for now, or let service calculate.
        // Better: Fetch approved leaves this year and sum
        var currentYear = DateTime.Now.Year;
        // This logic is better handled in service with business days calc, 
        // but for a simple "Global Days Taken" metric we can sum (EndDate - StartDate).Days + 1
        // Simplified SQL sum:
        return await _db.Leaves
            .Where(l => l.Status == "Approved" && l.StartDate.Year == currentYear)
            .SumAsync(l => EF.Functions.DateDiffDay(l.StartDate, l.EndDate) + 1);
    }

    public async Task<IEnumerable<Leave>> GetRecentRequestsByEmployeeIdAsync(int employeeId, int count)
    {
        return await _db.Leaves
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Leave>> GetRecentRequestsGlobalAsync(int count)
    {
        return await _db.Leaves
            .Include(l => l.Employee)
            .OrderByDescending(l => l.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Leave>> GetPendingLeavesByManagerIdAsync(int managerId)
    {
        return await _db.Leaves
            .Include(l => l.Employee)
            .Where(l => l.Status == "Pending" && l.Employee.ManagerId == managerId)
            .OrderBy(l => l.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Leave>> GetApprovedLeavesForDateAsync(DateTime date)
    {
        return await _db.Leaves
            .Include(l => l.Employee)
            .Where(l => l.Status == "Approved" 
                        && l.StartDate <= date 
                        && l.EndDate >= date)
            .ToListAsync();
    }
}



