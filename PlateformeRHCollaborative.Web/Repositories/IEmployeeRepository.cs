using PlateformeRHCollaborative.Web.Models;

namespace PlateformeRHCollaborative.Web.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee?> GetByUserIdAsync(string userId);
    Task<int> GetTeamSizeAsync(int managerId);
    Task<int> ResetAllBalancesAsync(int amount);
    Task<IEnumerable<Employee>> GetTeamMembersAsync(int managerId);
    Task<int> GetTotalCountAsync();
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<IEnumerable<Employee>> GetEmployeesWithLowBalanceAsync(int threshold);
    Task<IEnumerable<Employee>> GetEmployeesWithoutRecentLeaveAsync(int year);
    Task UpdateAsync(Employee employee);
}
