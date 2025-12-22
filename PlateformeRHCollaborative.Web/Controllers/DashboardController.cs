using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using PlateformeRHCollaborative.Web.Repositories;
=======
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize(Roles = "RH")]
public class DashboardController : Controller
{
<<<<<<< HEAD
    private readonly IEmployeeRepository _employeeRepo;

    public DashboardController(IEmployeeRepository employeeRepo)
    {
        _employeeRepo = employeeRepo;
    }

=======
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
    public IActionResult Index()
    {
        return View();
    }
<<<<<<< HEAD

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetBalances()
    {
        int count = await _employeeRepo.ResetAllBalancesAsync(25);
        TempData["SuccessMessage"] = $"Les soldes de {count} employés ont été réinitialisés à 25 jours.";
        return RedirectToAction("Index");
    }
=======
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
}





