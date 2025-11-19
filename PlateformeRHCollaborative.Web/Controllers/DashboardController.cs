using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize(Roles = "RH")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}





