using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Models;
using PlateformeRHCollaborative.Web.Services;
using PlateformeRHCollaborative.Web.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
                page.Margin(2.5f, Unit.Centimetre); // Marges généreuses standard administratif
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial")); // Police standard

                // Pas d'en-tête, on commence direct par le titre pour un style officiel
                
                page.Content()
                    .PaddingTop(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        // Titre Centré
                        column.Item().Text("ATTESTATION DE TRAVAIL").Bold().FontSize(22).AlignCenter().Underline();
                        
                        column.Item().Height(50); // Espacement

                        // Paragraphe d'intro
                        column.Item().PaddingTop(20).Text(text =>
                        {
                            text.Span("Nous soussignés, ");
                            text.Span("TechnoSolutions Maroc").Bold();
                            text.Span(", sis à Casablanca, certifions que :");
                        });

                        column.Item().Height(30);

                        // Bloc Employé
                        column.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(20).Column(col => 
                        {
                            col.Spacing(10);
                            col.Item().Text(t => { t.Span("M./Mme : ").SemiBold(); t.Span($"{employee.Nom}").FontSize(14).Bold(); });
                            col.Item().Text(t => { t.Span("Département : ").SemiBold(); t.Span($"{employee.Department}"); });
                        });

                        column.Item().Height(30);

                        // Corps du texte
                        column.Item().Text(text =>
                        {
                            text.Span("Est employé(e) au sein de notre société depuis le ");
                            // Date d'embauche réelle ou simulée
                            text.Span(employee.HireDate.ToString("dd/MM/yyyy")).Bold(); 
                            text.Span(" et occupe actuellement ses fonctions à temps plein.");
                        });

                        column.Item().Height(20);

                        column.Item().Text("Cette attestation est délivrée à la demande de l'intéressé(e) pour servir et valoir ce que de droit.");

                        // Date et Signature
                        column.Item().PaddingTop(60).AlignRight().Text($"Fait à Casablanca, le {DateTime.Now:dd/MM/yyyy}");

                        column.Item().PaddingTop(40).AlignRight().Column(col =>
                        {
                            col.Item().Text("Pour la Direction des Ressources Humaines").Bold();
                            col.Item().PaddingTop(50).Text("Signature").FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                    });

                // Pas de footer commercial, juste une ligne discrète ou rien
                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("TechnoSolutions Maroc - Patente 12345678 - RC Casablanca 98765").FontSize(8).FontColor(Colors.Grey.Medium);
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
        
        // Calculs Marocains Simplifiés
        decimal salaireBrut = employee.SalaryBase;
        
        // CNSS (4.48% plafonné à 6000 DH de base = max 268.80 DH)
        // Pour simplifier ici on applique le taux sur le brut sans plafond, ou avec plafond si on veut être précis.
        // Restons simple mais réaliste : 4.48%
        decimal cnssRate = 0.0448m;
        decimal cnssAmount = salaireBrut * cnssRate; 

        // AMO (2.26% sur le brut total)
        decimal amoRate = 0.0226m;
        decimal amoAmount = salaireBrut * amoRate;

        // IR (Impôt sur le Revenu) - Estimation simplifiée (ex: 10% après déductions sociales)
        // Salaire Net Imposable (SNI) = Brut - (CNSS + AMO)
        decimal sni = salaireBrut - cnssAmount - amoAmount;
        decimal irRate = 0.10m; // Taux moyen arbitraire pour l'exemple
        decimal irAmount = sni * irRate;

        decimal totalDeductions = cnssAmount + amoAmount + irAmount;
        decimal netAPayer = salaireBrut - totalDeductions;

        var month = DateTime.Now.ToString("MMMM yyyy", new System.Globalization.CultureInfo("fr-FR")).ToUpper();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                // Header Entrprise
                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("TechnoSolutions Maroc").Bold().FontSize(20).FontColor(Colors.Blue.Darken2);
                            col.Item().Text("45 Boulevard Zerktouni, 20000 Casablanca").FontSize(10);
                            col.Item().Text("IF : 12345678 | CNSS : 9876543 | ICE : 001234567890123").FontSize(9).FontColor(Colors.Grey.Darken1);
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
                                c.Item().Text(t => { t.Span("Employé : ").SemiBold(); t.Span(employee.Nom.ToUpper()).Bold(); });
                                c.Item().Text(t => { t.Span("Matricule : ").SemiBold(); t.Span($"EMP-{employee.Id:D6}"); });
                                c.Item().Text(t => { t.Span("CNSS : ").SemiBold(); t.Span("123456789"); }); 
                            });
                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text(t => { t.Span("Département : ").SemiBold(); t.Span(employee.Department); });
                                // Poste supprimé
                            });
                        });
                        
                        col.Item().Height(20);

                        // Table
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Rubrique
                                columns.RelativeColumn(1); // Base
                                columns.RelativeColumn(1); // Taux
                                columns.RelativeColumn(1); // Gain
                                columns.RelativeColumn(1); // Retenue
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Rubrique");
                                header.Cell().Element(CellStyle).AlignRight().Text("Base");
                                header.Cell().Element(CellStyle).AlignRight().Text("Taux");
                                header.Cell().Element(CellStyle).AlignRight().Text("Gains");
                                header.Cell().Element(CellStyle).AlignRight().Text("Retenues");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold().FontSize(10)).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            // Style Helper
                            IContainer RowStyle(IContainer container) => container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten3);

                            // 1. Salaire de Base
                            table.Cell().Element(RowStyle).Text("Salaire de Base");
                            table.Cell().Element(RowStyle).AlignRight().Text($"{salaireBrut:N2}");
                            table.Cell().Element(RowStyle).AlignRight().Text("");
                            table.Cell().Element(RowStyle).AlignRight().Text($"{salaireBrut:N2}");
                            table.Cell().Element(RowStyle).AlignRight().Text("");

                            // 2. CNSS
                            table.Cell().Element(RowStyle).Text("CNSS (Cotisations Sociales)");
                            table.Cell().Element(RowStyle).AlignRight().Text($"{salaireBrut:N2}");
                            table.Cell().Element(RowStyle).AlignRight().Text("4.48 %");
                            table.Cell().Element(RowStyle).AlignRight().Text("");
                            table.Cell().Element(RowStyle).AlignRight().Text($"{cnssAmount:N2}");

                            // 3. AMO
                            table.Cell().Element(RowStyle).Text("AMO (Assurance Maladie)");
                            table.Cell().Element(RowStyle).AlignRight().Text($"{salaireBrut:N2}");
                            table.Cell().Element(RowStyle).AlignRight().Text("2.26 %");
                            table.Cell().Element(RowStyle).AlignRight().Text("");
                            table.Cell().Element(RowStyle).AlignRight().Text($"{amoAmount:N2}");

                            // 4. IR
                            table.Cell().Element(RowStyle).Text("I.R. (Impôt sur le Revenu)");
                            table.Cell().Element(RowStyle).AlignRight().Text($"{sni:N2}");
                            table.Cell().Element(RowStyle).AlignRight().Text($"{irRate*100:N0} %");
                            table.Cell().Element(RowStyle).AlignRight().Text("");
                            table.Cell().Element(RowStyle).AlignRight().Text($"{irAmount:N2}");
                            
                            // Net
                             table.Cell().ColumnSpan(5).PaddingTop(15).BorderTop(1).BorderColor(Colors.Black).PaddingBottom(5).Row(row => 
                             {
                                 row.RelativeItem().Text("NET À PAYER").Bold().FontSize(14);
                                 row.RelativeItem().AlignRight().Text($"{netAPayer:N2} MAD").Bold().FontSize(14).FontColor(Colors.Blue.Darken2);
                             });
                        });

                        col.Item().Height(40);
                        col.Item().Background(Colors.Grey.Lighten5).Padding(10)
                           .Text("Ce bulletin de paie est conforme à la législation marocaine en vigueur. À conserver sans limitation de durée.")
                           .FontSize(9).Italic().AlignCenter();
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span($"TechnoSolutions Maroc - ICE 001234567890123 - {DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
            });
        });

        // Generate PDF
        var stream = new MemoryStream(document.GeneratePdf());
        return File(stream, "application/pdf", fileName);
    }
}




