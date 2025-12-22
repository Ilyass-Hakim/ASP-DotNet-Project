using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using PlateformeRHCollaborative.Web.Data;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Models;

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public ProfileController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.UserId == user.Id);

        return View(employee);
    }
}
