using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Repositories;

namespace PlateformeRHCollaborative.Web.Services;

public class TeleworkService
{
	private readonly ITeleworkRepository _repo;

	public TeleworkService(ITeleworkRepository repo)
	{
		_repo = repo;
	}

	public Task<IEnumerable<Telework>> GetAllAsync() => _repo.GetAllAsync();
	public Task<Telework?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
	public Task AddAsync(Telework entity) => _repo.AddAsync(entity);
	public Task UpdateAsync(Telework entity) => _repo.UpdateAsync(entity);
	public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
}



