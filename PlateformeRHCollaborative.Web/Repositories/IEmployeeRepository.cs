using PlateformeRHCollaborative.Web.Models;

namespace PlateformeRHCollaborative.Web.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByUserIdAsync(string userId);
    Task<int> GetTeamSizeAsync(int managerId);
    Task<int> ResetAllBalancesAsync(int amount);
}
