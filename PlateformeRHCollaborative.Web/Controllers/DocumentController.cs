using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Services;
using PlateformeRHCollaborative.Web.Data;

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize]
public class DocumentController : Controller
{
    private readonly DocumentService _service;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public DocumentController(DocumentService service, UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _service = service;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _service.GetAllAsync();
        return View(items);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(DocumentRequest model)
    {
        if (!ModelState.IsValid) return View(model);
        await _service.AddAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, DocumentRequest model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);
        await _service.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> GenerateCertificate()
    {
        var userId = _userManager.GetUserId(User);
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
        
        string fileName = $"Attestation_Travail_{DateTime.Now:yyyyMMdd}.txt";
        string content;

        if (employee != null)
        {
            content = $"ATTESTATION DE TRAVAIL\n\n" +
                      $"Nous soussignés, CollabRH, certifions que M./Mme {employee.Nom}, " +
                      $"occupant le poste de {employee.Poste}, est actuellement en poste dans notre entreprise.\n\n" +
                      $"Fait à Paris, le : {DateTime.Now:dd/MM/yyyy}\n\n" +
                      $"Direction des Ressources Humaines";
        }
        else
        {
            content = "Erreur : Profil employé non trouvé. Veuillez contacter les RH.";
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        return File(bytes, "text/plain", fileName);
    }

    [HttpGet]
    public async Task<IActionResult> GeneratePaySlip()
    {
        var userId = _userManager.GetUserId(User);
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);

        string fileName = $"Fiche_Paie_{DateTime.Now:yyyyMM}.txt";
        string content;

        if (employee != null)
        {
            decimal pointsBonus = employee.PerformancePoints * 5.0m;
            decimal grossSalary = employee.SalaryBase + pointsBonus;
            decimal socialContributions = grossSalary * 0.22m; // Simulate 22% contributions
            decimal netSalary = grossSalary - socialContributions;

            content = $"FICHE DE PAIE - {DateTime.Now:MMMM yyyy}\n\n" +
                      $"Employé : {employee.Nom}\n" +
                      $"Poste : {employee.Poste}\n" +
                      $"Email : {employee.Email}\n" +
                      $"----------------------------------------\n" +
                      $"Salaire de base      : {employee.SalaryBase:N2} €\n" +
                      $"Primes Performance   : {pointsBonus:N2} € ({employee.PerformancePoints} pts)\n" +
                      $"----------------------------------------\n" +
                      $"TOTAL BRUT           : {grossSalary:N2} €\n" +
                      $"Cotisations sociales : -{socialContributions:N2} €\n" +
                      $"----------------------------------------\n" +
                      $"NET À PAYER          : {netSalary:N2} €\n\n" +
                      $"Document généré le {DateTime.Now:dd/MM/yyyy HH:mm}";
        }
        else
        {
            content = "Erreur : Profil employé non trouvé.";
        }

        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        return File(bytes, "text/plain", fileName);
    }
}


