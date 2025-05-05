using Microsoft.AspNetCore.Identity;

namespace GCBPMS.Components.Services
{
    public class RoleSeederHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public RoleSeederHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a scope to get scoped services like RoleManager and UserManager
            using (var scope = _serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                // Seed roles and users
                await SeedRolesAsync(roleManager, userManager);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // This method can be left empty if there's nothing to clean up.
            return Task.CompletedTask;
        }

        // Method to seed roles (and optionally users)
        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            string[] roleNames = { "Maintenance Admin", "Maintenance User", "Production Admin", "Production User","Admin"};

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
