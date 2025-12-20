using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Data;
using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Services;

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize]
public class PerformanceController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IPerformanceService _performanceService;

    public PerformanceController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IPerformanceService performanceService)
    {
        _context = context;
        _userManager = userManager;
        _performanceService = performanceService;
    }

    // List evaluations
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var roles = await _userManager.GetRolesAsync(await _userManager.GetUserAsync(User));

        IQueryable<PerformanceEvaluation> query = _context.PerformanceEvaluations
            .Include(e => e.Employee);

        if (roles.Contains("RH"))
        {
            // RH sees everything
        }
        else if (roles.Contains("Manager"))
        {
            // Managers see evaluations they created
            query = query.Where(e => e.ManagerId == userId);
        }
        else
        {
            // Employees see their own evaluations (only validated ones maybe?)
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee == null) return NotFound();
            query = query.Where(e => e.EmployeeId == employee.Id && e.Status == EvaluationStatus.Validated);
        }

        return View(await query.OrderByDescending(e => e.EvaluationDate).ToListAsync());
    }

    // RH: Configure criteria
    [Authorize(Roles = "RH")]
    public async Task<IActionResult> ManageCriteria()
    {
        return View(await _context.EvaluationCriteria.ToListAsync());
    }

    [Authorize(Roles = "RH")]
    [HttpPost]
    public async Task<IActionResult> UpdateCriteria(List<EvaluationCriteria> criteria)
    {
        foreach (var item in criteria)
        {
            var dbItem = await _context.EvaluationCriteria.FindAsync(item.Id);
            if (dbItem != null)
            {
                dbItem.Weight = item.Weight;
                dbItem.Name = item.Name;
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(ManageCriteria));
    }

    // Manager: Start evaluation
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> Create(int employeeId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null) return NotFound();

        var criteria = await _context.EvaluationCriteria.ToListAsync();
        var model = new PerformanceEvaluation
        {
            EmployeeId = employeeId,
            Employee = employee,
            ManagerId = _userManager.GetUserId(User) ?? "",
            EvaluationDate = DateTime.Now,
            Details = criteria.Select(c => new EvaluationDetail { EvaluationCriteriaId = c.Id, Criteria = c }).ToList()
        };

        return View(model);
    }

    [Authorize(Roles = "Manager")]
    [HttpPost]
    public async Task<IActionResult> Create(PerformanceEvaluation evaluation, List<int> criteriaIds, List<int> scores)
    {
        evaluation.ManagerId = _userManager.GetUserId(User) ?? "";
        evaluation.EvaluationDate = DateTime.Now;
        evaluation.Status = EvaluationStatus.Submitted;

        _context.PerformanceEvaluations.Add(evaluation);
        await _context.SaveChangesAsync();

        for (int i = 0; i < criteriaIds.Count; i++)
        {
            var detail = new EvaluationDetail
            {
                PerformanceEvaluationId = evaluation.Id,
                EvaluationCriteriaId = criteriaIds[i],
                Score = scores[i]
            };
            _context.EvaluationDetails.Add(detail);
        }
        await _context.SaveChangesAsync();

        // Calculate score and bonus
        await _performanceService.CalculateScoreAsync(evaluation.Id);
        await _performanceService.CalculateBonusAsync(evaluation.Id);

        return RedirectToAction(nameof(Index));
    }

    // Details View
    public async Task<IActionResult> Details(int id)
    {
        var evaluation = await _context.PerformanceEvaluations
            .Include(e => e.Employee)
            .Include(e => e.Details)
            .ThenInclude(d => d.Criteria)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evaluation == null) return NotFound();

        return View(evaluation);
    }

    // RH: Validate Bonus
    [Authorize(Roles = "RH")]
    [HttpPost]
    public async Task<IActionResult> ValidateBonus(int id, BonusStatus status, decimal? adjustedAmount, string? comments)
    {
        var evaluation = await _context.PerformanceEvaluations.FindAsync(id);
        if (evaluation == null) return NotFound();

        evaluation.BonusStatus = status;
        if (adjustedAmount.HasValue) evaluation.CalculatedBonus = adjustedAmount.Value;
        evaluation.RHComments = comments;
        evaluation.Status = EvaluationStatus.Validated;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
