using GCBPMS.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace GCBPMS.Components.Services
{
	public class GlobalFunction
	{
		private PmsContext _dbContext;
		public List<Phase> phaseList = new List<Phase>();
		private NotificationService _notificationService;
		private readonly AuthenticationStateProvider _authenticationStateProvider;
		private readonly UserManager<IdentityUser> _userManager;
		public enum MasterDataType
		{
            Phase,
			Press,
			Pot,
			Plate,
			Brand,
			User
		}
		public GlobalFunction(PmsContext dbContext, 
			NotificationService notificationService,
			AuthenticationStateProvider authenticationStateProvider,
			UserManager<IdentityUser> userManager)
		{
			_dbContext = dbContext;
			_notificationService = notificationService;
			_authenticationStateProvider = authenticationStateProvider;
			_userManager = userManager;
		}

		public async Task<string> getUsernameString()
		{
			var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
			var user = authState.User;
			return (_userManager.GetUserName(user));
		}

		public async Task<List<Phase>> getActivePhase()
		{
			return await _dbContext.Phases.Where(p => p.Active == true).ToListAsync();
		}

		public async Task<List<Press>> getActivePress()
		{
			return await _dbContext.Presses.Where(p => p.Active == true).ToListAsync();
		}

		public async Task<List<Pot>> getActivePot()
		{
			return await _dbContext.Pots.Where(p => p.Active == true).ToListAsync();
		}	

		public async Task<List<Brand>> getActiveBrand()
		{
			return await _dbContext.Brands.Where(p => p.Active).ToListAsync();
		}

		public async Task<List<Plate>> getActivePlate()
		{
			return await _dbContext.Plates
							.Where(p => p.Active == true).ToListAsync();
		}

		public async Task<List<Press>> getPressesByPhaseID(int phaseId)
		{
			return await _dbContext.Presses.Where(p => p.PhaseId == phaseId).ToListAsync();
		}

		public async Task<List<Pot>> getPotsByPressID(int pressId)
		{
			return await _dbContext.Pots
				.Include(p => p.Plate)
				.Where(p => p.PressId == pressId).ToListAsync();
		}

		public async Task<List<Plate>> getPlatesInInventory(){
			return await _dbContext.Plates.Where(p => p.Active == true && p.PlateStatus == "Inventory").ToListAsync();
		}

        public async Task<Plate> getPlateInPot(int potId)
        {
            return await _dbContext.Plates
            .Include(p => p.Pots)
            .Where(p => p.Active == true && p.Pots.Any(p => p.Id == potId)).FirstOrDefaultAsync();
        }

        public void ShowNotification(NotificationMessage message)
		{
			_notificationService.Notify(message);
		}

		public async Task<List<UserAction>> getUserActionList()
		{
			return await _dbContext.UserActions
							.Include(u => u.ActionNavigation)
							.Include(u => u.Plate)
							.OrderByDescending(u => u.ActionDatetime)
							.ToListAsync();
		}
	}
}
