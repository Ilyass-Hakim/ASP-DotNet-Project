using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Data;
using PlateformeRHCollaborative.Web.Models;

namespace PlateformeRHCollaborative.Web.Repositories;

public class DocumentRepository : IDocumentRepository
{
	private readonly ApplicationDbContext _db;

	public DocumentRepository(ApplicationDbContext db)
	{
		_db = db;
	}

	public async Task<IEnumerable<DocumentRequest>> GetAllAsync()
	{
		return await _db.DocumentRequests.AsNoTracking().ToListAsync();
	}

	public async Task<DocumentRequest?> GetByIdAsync(int id)
	{
		return await _db.DocumentRequests.FindAsync(id);
	}

	public async Task AddAsync(DocumentRequest entity)
	{
		await _db.DocumentRequests.AddAsync(entity);
		await _db.SaveChangesAsync();
	}

	public async Task UpdateAsync(DocumentRequest entity)
	{
		_db.DocumentRequests.Update(entity);
		await _db.SaveChangesAsync();
	}

	public async Task DeleteAsync(int id)
	{
		var item = await _db.DocumentRequests.FindAsync(id);
		if (item != null)
		{
			_db.DocumentRequests.Remove(item);
			await _db.SaveChangesAsync();
		}
	}
}



