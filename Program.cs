using GCBPMS.Components;
using GCBPMS.Components.Services;
using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;
using GCBPMS.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using GCBPMS.Components.Pages;

namespace GCBPMS
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddDbContext<PmsContext>(options =>
				   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

            builder.Services.AddDbContext<GCBPMSIdentityDbContext>(options =>
                   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false; // Disable email confirmation
                options.User.RequireUniqueEmail = false;        // No unique email required
            })
           .AddRoles<IdentityRole>()
           .AddEntityFrameworkStores<GCBPMSIdentityDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login"; // Redirect to this path when unauthorized
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Optional: where to redirect if access is denied
            });

            // Add services to the container.
            builder.Services.AddRazorComponents()
				.AddInteractiveServerComponents();

			builder.Services.AddScoped<GlobalFunction>();
            builder.Services.AddRadzenComponents();
			builder.Services.AddScoped<MasterDataPageServices>();
			builder.Services.AddScoped<PressDetailPageServices>();
			builder.Services.AddScoped<PlateAssignService>();
            builder.Services.AddScoped<PlateDetailPageServices>();
            builder.Services.AddScoped<ProdCreateRequestPageServices>();
            builder.Services.AddScoped<MaintenanceRequestServices>();
			builder.Services.AddScoped<NotificationService>();
            builder.Services.AddHostedService<RoleSeederHostedService>();
			builder.Services.AddScoped<PressDetailPage>();
			builder.Services.AddScoped<ReportPageServices>();
			builder.Services.AddScoped<MaintenanceDashboardServices>();

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();

			app.UseStaticFiles();
			app.UseAntiforgery();

			app.MapRazorComponents<App>()
				.AddInteractiveServerRenderMode();

			app.MapRazorPages();

			app.Run();
		}
	}
}
