using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Repositories;

namespace PlateformeRHCollaborative.Web.Services;

public class LeaveService
{
	private readonly ILeaveRepository _repo;

	public LeaveService(ILeaveRepository repo)
	{
		_repo = repo;
	}

	public Task<IEnumerable<Leave>> GetAllAsync() => _repo.GetAllAsync();
	public Task<Leave?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
	public Task AddAsync(Leave entity) => _repo.AddAsync(entity);
	public Task UpdateAsync(Leave entity) => _repo.UpdateAsync(entity);
	public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
}



