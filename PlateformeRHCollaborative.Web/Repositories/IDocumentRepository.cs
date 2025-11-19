namespace PlateformeRHCollaborative.Web.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using PlateformeRHCollaborative.Web.Models;

public interface IDocumentRepository
{
	Task<IEnumerable<DocumentRequest>> GetAllAsync();
	Task<DocumentRequest?> GetByIdAsync(int id);
	Task AddAsync(DocumentRequest entity);
	Task UpdateAsync(DocumentRequest entity);
	Task DeleteAsync(int id);
}



