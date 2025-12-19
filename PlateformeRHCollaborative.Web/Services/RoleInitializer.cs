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
            try {
                await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE Employees ADD SalaryBase decimal(18,2) NOT NULL DEFAULT 3000", cancellationToken);
            } catch { /* Ignorer si déjà existant */ }
            try {
                await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE Employees ADD PerformancePoints int NOT NULL DEFAULT 0", cancellationToken);
            } catch { /* Ignorer si déjà existant */ }

            // Attendre que la base de données soit prête
            try {
                await dbContext.Database.MigrateAsync(cancellationToken);
            } catch (Exception ex) { 
                Console.WriteLine($"Migration error (ignoring): {ex.Message}");
            }

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

