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
}



