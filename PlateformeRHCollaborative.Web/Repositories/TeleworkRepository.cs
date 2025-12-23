using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Data;
using PlateformeRHCollaborative.Web.Models;

namespace PlateformeRHCollaborative.Web.Repositories;

public class TeleworkRepository : ITeleworkRepository
{
	private readonly ApplicationDbContext _db;

	public TeleworkRepository(ApplicationDbContext db)
	{
		_db = db;
	}

	public async Task<IEnumerable<Telework>> GetAllAsync()
	{
		// Charger également l'employé lié pour permettre l'affichage du nom / email
		return await _db.Teleworks
			.Include(t => t.Employee)
			.AsNoTracking()
			.ToListAsync();
	}

	public async Task<Telework?> GetByIdAsync(int id)
	{
		// Inclure systématiquement l'employé pour les traitements métier côté manager
		return await _db.Teleworks
			.Include(t => t.Employee)
			.FirstOrDefaultAsync(t => t.Id == id);
	}

	public async Task AddAsync(Telework entity)
	{
		await _db.Teleworks.AddAsync(entity);
		await _db.SaveChangesAsync();
	}

	public async Task UpdateAsync(Telework entity)
	{
		_db.Teleworks.Update(entity);
		await _db.SaveChangesAsync();
	}

	public async Task DeleteAsync(int id)
	{
		var item = await _db.Teleworks.FindAsync(id);
		if (item != null)
		{
			_db.Teleworks.Remove(item);
			await _db.SaveChangesAsync();
		}
	}

    public async Task<bool> HasOverlapAsync(int employeeId, DateTime start, DateTime end)
    {
        return await _db.Teleworks
            .AnyAsync(t => t.EmployeeId == employeeId 
                        && t.Status != "Rejected" 
                        && t.Status != "Cancelled"
                        && t.StartDate <= end 
                        && t.EndDate >= start);
    }

    public async Task<int> GetApprovedCountForTeamAsync(int managerId, DateTime start, DateTime end)
    {
        return await _db.Teleworks
            .Include(t => t.Employee)
            .Where(t => t.Employee.ManagerId == managerId 
                        && t.Status == "Approved" 
                        && t.StartDate <= end 
                        && t.EndDate >= start)
            .Select(t => t.EmployeeId)
            .Distinct()
            .CountAsync();
    }
    public async Task<IEnumerable<Telework>> GetRecentRequestsByEmployeeIdAsync(int employeeId, int count)
    {
        return await _db.Teleworks
            .Where(t => t.EmployeeId == employeeId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Telework>> GetRecentRequestsGlobalAsync(int count)
    {
        return await _db.Teleworks
            .Include(t => t.Employee)
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Telework>> GetPendingTeleworksByManagerIdAsync(int managerId)
    {
        return await _db.Teleworks
            .Include(t => t.Employee)
            .Where(t => t.Status == "Pending" && t.Employee.ManagerId == managerId)
            .OrderBy(t => t.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Telework>> GetApprovedTeleworksForDateAsync(DateTime date)
    {
        return await _db.Teleworks
            .Include(t => t.Employee)
            .Where(t => t.Status == "Approved" 
                        && t.StartDate <= date 
                        && t.EndDate >= date)
            .ToListAsync();
    }

    public async Task<int> GetTotalTeleworkingCountAsync(DateTime date)
    {
        return await _db.Teleworks
            .CountAsync(t => t.Status == "Approved" && t.StartDate <= date && t.EndDate >= date);
    }
}



