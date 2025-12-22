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
<<<<<<< HEAD
		// Charger également l'employé lié pour permettre l'affichage du nom / email
		return await _db.Teleworks
			.Include(t => t.Employee)
			.AsNoTracking()
			.ToListAsync();
=======
		return await _db.Teleworks.AsNoTracking().ToListAsync();
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
	}

	public async Task<Telework?> GetByIdAsync(int id)
	{
<<<<<<< HEAD
		// Inclure systématiquement l'employé pour les traitements métier côté manager
		return await _db.Teleworks
			.Include(t => t.Employee)
			.FirstOrDefaultAsync(t => t.Id == id);
=======
		return await _db.Teleworks.FindAsync(id);
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
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
<<<<<<< HEAD

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
=======
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
}



