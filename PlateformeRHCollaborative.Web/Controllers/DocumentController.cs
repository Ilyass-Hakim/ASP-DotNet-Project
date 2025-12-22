using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Services;
using PlateformeRHCollaborative.Web.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
=======
using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Services;
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d

namespace PlateformeRHCollaborative.Web.Controllers;

[Authorize]
public class DocumentController : Controller
{
    private readonly DocumentService _service;
<<<<<<< HEAD
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _context;

    public DocumentController(DocumentService service, UserManager<IdentityUser> userManager, ApplicationDbContext context)
    {
        _service = service;
        _userManager = userManager;
        _context = context;
=======

    public DocumentController(DocumentService service)
    {
        _service = service;
>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
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
<<<<<<< HEAD
    [HttpGet]
    public async Task<IActionResult> GenerateCertificate()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var userId = _userManager.GetUserId(User);
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
        
        if (employee == null) return NotFound("Profil employé non trouvé.");

        string fileName = $"Attestation_Travail_{DateTime.Now:yyyyMMdd}.pdf";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("CollabRH").SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);
                            col.Item().Text("Direction des Ressources Humaines").FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                        row.ConstantItem(100).AlignRight().Text("ATTESTATION").FontSize(20).SemiBold().FontColor(Colors.Grey.Lighten2);
                    });

                page.Content()
                    .PaddingVertical(2, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(20);

                        column.Item().Text("ATTESTATION DE TRAVAIL").Bold().FontSize(20).AlignCenter().Underline();
                        
                        column.Item().PaddingTop(20).Text(text =>
                        {
                            text.Span("Nous soussignés, ");
                            text.Span("CollabRH").Bold();
                            text.Span(", sis à Paris, certifions que :");
                        });

                        column.Item().Background(Colors.Grey.Lighten4).Padding(10).Column(col => 
                        {
                            col.Item().Text(t => { t.Span("M./Mme : ").SemiBold(); t.Span($"{employee.Nom}"); });
                            col.Item().Text(t => { t.Span("Poste : ").SemiBold(); t.Span($"{employee.Poste}"); });
                            col.Item().Text(t => { t.Span("Département : ").SemiBold(); t.Span($"{employee.Department}"); });
                        });

                        column.Item().Text(text =>
                        {
                            text.Span("Est employé(e) au sein de notre société depuis le ");
                            // Modèle fictif date embauche si null
                            text.Span(DateTime.Now.AddMonths(-6).ToString("dd/MM/yyyy")).Bold(); 
                            text.Span(" et occupe actuellement ses fonctions à temps plein.");
                        });

                        column.Item().Text("Cette attestation est délivrée à la demande de l'intéressé(e) pour servir et valoir ce que de droit.");

                        column.Item().PaddingTop(30).Text($"Fait à Paris, le {DateTime.Now:dd/MM/yyyy}").AlignRight();

                        column.Item().PaddingTop(50).Column(col =>
                        {
                            col.Item().Text("Pour la Direction, des Ressources Humaines").AlignRight();
                            col.Item().Text("Signature").FontSize(10).FontColor(Colors.Grey.Medium).AlignRight();
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("CollabRH - SAS au capital de 50 000 € - RCS Paris B 123 456 789");
                        x.Span($" - Page {x.CurrentPageNumber()}");
                    });
            });
        });

        // Generate PDF
        var stream = new MemoryStream(document.GeneratePdf());
        return File(stream, "application/pdf", fileName);
    }

    [HttpGet]
    public async Task<IActionResult> GeneratePaySlip()
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var userId = _userManager.GetUserId(User);
        var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);

        if (employee == null) return NotFound("Profil employé non trouvé.");

        string fileName = $"Fiche_Paie_{DateTime.Now:yyyyMM}.pdf";
        
        decimal grossSalary = employee.SalaryBase;
        decimal socialContributions = grossSalary * 0.22m;
        decimal netSalary = grossSalary - socialContributions;
        var month = DateTime.Now.ToString("MMMM yyyy").ToUpper();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("CollabRH").Bold().FontSize(20).FontColor(Colors.Blue.Darken2);
                            col.Item().Text("123 Avenue des Champs-Elysées, 75008 Paris").FontSize(9);
                            col.Item().Text("SIRET : 123 456 789 00012").FontSize(9);
                        });
                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("BULLETIN DE PAIE").Bold().FontSize(18).FontColor(Colors.Grey.Darken3);
                            col.Item().Text($"Période : {month}").FontSize(12);
                        });
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(col =>
                    {
                        // Employee Info Box
                        col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Employé : ").SemiBold(); t.Span(employee.Nom).Bold(); });
                                c.Item().Text(t => { t.Span("Matricule : ").SemiBold(); t.Span($"EMP-{employee.Id:D6}"); });
                                c.Item().Text(t => { t.Span("Sécurité Sociale : ").SemiBold(); t.Span("1 85 05 75 123 456 78"); });
                            });
                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Département : ").SemiBold(); t.Span(employee.Department); });
                                c.Item().Text(t => { t.Span("Poste : ").SemiBold(); t.Span(employee.Poste); });
                            });
                        });
                        
                        col.Item().Height(20);

                        // Table
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Description
                                columns.RelativeColumn(1); // Base
                                columns.RelativeColumn(1); // Taux
                                columns.RelativeColumn(1); // Montant
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Rubrique");
                                header.Cell().Element(CellStyle).AlignRight().Text("Base");
                                header.Cell().Element(CellStyle).AlignRight().Text("Taux");
                                header.Cell().Element(CellStyle).AlignRight().Text("Montant");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            // Rows
                            table.Cell().Element(RowStyle).Text("Salaire de Base");
                            table.Cell().Element(RowStyle).AlignRight().Text($"{grossSalary:N2}");
                            table.Cell().Element(RowStyle).AlignRight().Text("");
                            table.Cell().Element(RowStyle).AlignRight().Text($"{grossSalary:N2}");

                            table.Cell().Element(RowStyle).Text("Cotisations Sociales (Est.)");
                            table.Cell().Element(RowStyle).AlignRight().Text($"{grossSalary:N2}");
                            table.Cell().Element(RowStyle).AlignRight().Text("22.00 %");
                            table.Cell().Element(RowStyle).AlignRight().Text($"-{socialContributions:N2}");
                            
                            // Net
                             table.Cell().ColumnSpan(4).PaddingTop(10).BorderTop(1).BorderColor(Colors.Black).PaddingBottom(5).Row(row => 
                             {
                                 row.RelativeItem().Text("NET À PAYER").Bold().FontSize(14);
                                 row.RelativeItem().AlignRight().Text($"{netSalary:N2} €").Bold().FontSize(14).FontColor(Colors.Blue.Darken2);
                             });

                            static IContainer RowStyle(IContainer container)
                            {
                                return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                            }
                        });

                        col.Item().Height(30);
                        col.Item().Background(Colors.Grey.Lighten4).Padding(10).Text("Pour faire valoir vos droits à la retraite, conservez ce bulletin de paie sans limitation de durée.").FontSize(9).Italic();
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("CollabRH - Document généré le ");
                        x.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
            });
        });

        // Generate PDF
        var stream = new MemoryStream(document.GeneratePdf());
        return File(stream, "application/pdf", fileName);
    }
}




=======
}


>>>>>>> 99db1a64cfe1641f1f5fdfba5b7e2f15e348909d
