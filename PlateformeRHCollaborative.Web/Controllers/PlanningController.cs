using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize]
public class PlanningController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}





