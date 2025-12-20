using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlateformeRHCollaborative.Web.Data;

namespace PlateformeRHCollaborative.Web.Services;

public class RoleInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public RoleInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Hack: Ajout manuel des colonnes car nous sommes sur SQL Server et les migrations sont bloquées
            try
            {
                await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE Employees ADD SalaryBase decimal(18,2) NOT NULL DEFAULT 3000", cancellationToken);
            }
            catch { /* Ignorer si déjà existant */ }
            try
            {
                await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE Employees ADD PerformancePoints int NOT NULL DEFAULT 0", cancellationToken);
            }
            catch { /* Ignorer si déjà existant */ }

            // Tables pour le module de performance
            try
            {
                await dbContext.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EvaluationCriteria' AND xtype='U')
                    CREATE TABLE EvaluationCriteria (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        Name NVARCHAR(MAX) NOT NULL,
                        Weight FLOAT NOT NULL,
                        Description NVARCHAR(MAX)
                    )", cancellationToken);

                await dbContext.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PerformanceEvaluations' AND xtype='U')
                    CREATE TABLE PerformanceEvaluations (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        EmployeeId INT NOT NULL,
                        ManagerId NVARCHAR(MAX) NOT NULL,
                        EvaluationDate DATETIME2 NOT NULL,
                        Period NVARCHAR(MAX) NOT NULL,
                        FinalScore FLOAT NOT NULL,
                        Status INT NOT NULL,
                        CalculatedBonus DECIMAL(18,2) NOT NULL,
                        BonusStatus INT NOT NULL,
                        RHComments NVARCHAR(MAX),
                        CONSTRAINT FK_PerformanceEvaluations_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE CASCADE
                    )", cancellationToken);

                await dbContext.Database.ExecuteSqlRawAsync(@"
                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='EvaluationDetails' AND xtype='U')
                    CREATE TABLE EvaluationDetails (
                        Id INT PRIMARY KEY IDENTITY(1,1),
                        PerformanceEvaluationId INT NOT NULL,
                        EvaluationCriteriaId INT NOT NULL,
                        Score INT NOT NULL,
                        CONSTRAINT FK_EvaluationDetails_PerformanceEvaluations_PerformanceEvaluationId FOREIGN KEY (PerformanceEvaluationId) REFERENCES PerformanceEvaluations(Id) ON DELETE CASCADE,
                        CONSTRAINT FK_EvaluationDetails_EvaluationCriteria_EvaluationCriteriaId FOREIGN KEY (EvaluationCriteriaId) REFERENCES EvaluationCriteria(Id) ON DELETE CASCADE
                    )", cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating tables: {ex.Message}");
            }

            // Attendre que la base de données soit prête
            try
            {
                await dbContext.Database.MigrateAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Migration error (ignoring): {ex.Message}");
            }

            // Initialiser les critères par défaut
            var performanceService = scope.ServiceProvider.GetRequiredService<IPerformanceService>();
            await performanceService.InitializeDefaultCriteriaAsync();

            string[] roles = { "Employe", "Manager", "RH" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
        catch (Exception ex)
        {
            // Logger l'erreur mais ne pas bloquer le démarrage
            Console.WriteLine($"Erreur lors de l'initialisation des rôles : {ex}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

