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
<<<<<<< HEAD
		// Charger également l'employé lié pour permettre l'affichage du nom / email
		return await _db.Leaves
			.Include(l => l.Employee)
			.AsNoTracking()
			.ToListAsync();
=======
		return await _db.Leaves.AsNoTracking().ToListAsync();
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
	}

	public async Task<Leave?> GetByIdAsync(int id)
	{
<<<<<<< HEAD
		// Inclure systématiquement l'employé pour les traitements métier côté manager
		return await _db.Leaves
			.Include(l => l.Employee)
			.FirstOrDefaultAsync(l => l.Id == id);
=======
		return await _db.Leaves.FindAsync(id);
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
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
<<<<<<< HEAD

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
=======
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
}



