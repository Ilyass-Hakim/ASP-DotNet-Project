using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PlateformeRHCollaborative.Web.Repositories;
using PlateformeRHCollaborative.Web.Services;

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly DashboardService _dashboardService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IEmployeeRepository _employeeRepo; // Keep for specific actions like Reset

    public DashboardController(DashboardService dashboardService, UserManager<IdentityUser> userManager, IEmployeeRepository employeeRepo)
    {
        _dashboardService = dashboardService;
        _userManager = userManager;
        _employeeRepo = employeeRepo;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Account");
        
        if (User.IsInRole("Directeur"))
        {
            var vm = await _dashboardService.GetDirectorDashboardAsync(userId);
            return View("Directeur", vm);
        }
        else if (User.IsInRole("RH"))
        {
            var vm = await _dashboardService.GetRHDashboardAsync(userId);
            return View("RH", vm);
        }
        else if (User.IsInRole("Manager"))
        {
            var vm = await _dashboardService.GetManagerDashboardAsync(userId);
            return View("Manager", vm);
        }
        else
        {
            // Default to Employe
            var vm = await _dashboardService.GetEmployeeDashboardAsync(userId);
            return View("Employe", vm);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "RH")]
    public async Task<IActionResult> ResetBalances()
    {
        int count = await _employeeRepo.ResetAllBalancesAsync(25);
        TempData["SuccessMessage"] = $"Les soldes de {count} employés ont été réinitialisés à 25 jours.";
        return RedirectToAction("Index");
    }
}




