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
		return await _db.Leaves.AsNoTracking().ToListAsync();
	}

	public async Task<Leave?> GetByIdAsync(int id)
	{
		return await _db.Leaves.FindAsync(id);
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
}



