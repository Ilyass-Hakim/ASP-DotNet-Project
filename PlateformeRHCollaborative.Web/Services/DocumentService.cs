using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Repositories;

namespace PlateformeRHCollaborative.Web.Services;

public class DocumentService
{
	private readonly IDocumentRepository _repo;

	public DocumentService(IDocumentRepository repo)
	{
		_repo = repo;
	}

	public Task<IEnumerable<DocumentRequest>> GetAllAsync() => _repo.GetAllAsync();
	public Task<DocumentRequest?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);
	public Task AddAsync(DocumentRequest entity) => _repo.AddAsync(entity);
	public Task UpdateAsync(DocumentRequest entity) => _repo.UpdateAsync(entity);
	public Task DeleteAsync(int id) => _repo.DeleteAsync(id);
}



