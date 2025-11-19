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

            // Attendre que la base de données soit prête
            await dbContext.Database.MigrateAsync(cancellationToken);

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
            Console.WriteLine($"Erreur lors de l'initialisation des rôles : {ex.Message}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

