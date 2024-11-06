using GCBPMS.Components;
using GCBPMS.Components.Services;
using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;
using Radzen;
using GCBPMS.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace GCBPMS
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddDbContext<PmsContext>(options =>
				   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDbContext<GCBPMSIdentityDbContext>(options =>
                   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<GCBPMSIdentityDbContext>();

			// Add services to the container.
			builder.Services.AddRazorComponents()
				.AddInteractiveServerComponents();

			builder.Services.AddTransient<PmsContext>();
			builder.Services.AddScoped<GlobalFunction>();
            builder.Services.AddRadzenComponents();
			builder.Services.AddScoped<MasterDataPageServices>();
			builder.Services.AddScoped<PressDetailPageServices>();
			builder.Services.AddScoped<PlateAssignService>();
            builder.Services.AddScoped<PlateDetailPageServices>();
            builder.Services.AddScoped<ProdCreateRequestPageServices>();
            builder.Services.AddScoped<MaintenanceRequestServices>();
			builder.Services.AddScoped<NotificationService>();

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

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
