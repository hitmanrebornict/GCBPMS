using GCBPMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Diagnostics;

namespace GCBPMS.Components.Services
{
    public class MasterDataPageServices
    {
        private PmsContext _dbContext;
        private UserManager<IdentityUser> _userManager;
        private GlobalFunction GFS;
        public MasterDataPageServices(PmsContext dbContext, UserManager<IdentityUser> userManager, GlobalFunction gf)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            GFS = gf;
        }

        public async Task<List<Phase>> getPhasesAsync()
        {
            return await _dbContext.Phases.ToListAsync();
        }

        public async Task<List<Brand>> getBrandsAsync()
        {
            return await _dbContext.Brands.ToListAsync();
        }

        public async Task<List<Plate>> getPlatesAsync()
        {
            return await _dbContext.Plates
                            .Include(p => p.PlateBrandNavigation)
                            .Include(p => p.PhaseTypeNavigation)
                            .OrderBy(p => p.PlateName)
                            .AsNoTracking()
                            .ToListAsync();
        }

        public async Task<List<Pot>> getPotsAsync()
        {
            return await _dbContext.Pots
                            .Include(p => p.Plate)
                            .Include(p => p.Press)
                             .ToListAsync();
        }

        public async Task<List<Press>> getPressesAsync()
        {
            return await _dbContext.Presses.ToListAsync();
        }

        public async Task<List<AspNetUser>> getUserListAsync()
        {
            return await _dbContext.AspNetUsers
                 .Include(r => r.Roles)
                .ToListAsync();
        }


        public async Task CreatePhaseAsync(string phaseName, int potNumber)
        {
            var phase = new Phase
            {
                PhaseName = phaseName,
                PotNumber = potNumber,
                Active = true
            };

			var newUserAction = new UserAction()
			{
				Username = await GFS.getUsernameString(),
				ActionDatetime = DateTime.Now,
				Action = 8 //Create - Phase
			};

			await _dbContext.UserActions.AddAsync(newUserAction);

			await _dbContext.Phases.AddAsync(phase);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateBrandAsync(string brandName)
        {
            var brand = new Brand
            {
                BrandName = brandName,
                Active = true
            };

			var newUserAction = new UserAction()
			{
				Username = await GFS.getUsernameString(),
				ActionDatetime = DateTime.Now,
				Action = 12 //Create - Brand
			};

			await _dbContext.UserActions.AddAsync(newUserAction);

			await _dbContext.Brands.AddAsync(brand);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreatePlateAsync(string plateName, int brandId, int phaseTypeId, DateTime installDate)
        {
            var plate = new Plate
            {
                PlateName = plateName,
                PlateBrand = brandId,
                PhaseType = phaseTypeId,
                PlateInstallDatetime = installDate,
                Active = true,
                PlateStatus = "Inventory"
            };

			var newUserAction = new UserAction()
			{
				Username = await GFS.getUsernameString(),
				ActionDatetime = DateTime.Now,
				Action = 11 //Create - Plate
			};

			await _dbContext.UserActions.AddAsync(newUserAction);

			await _dbContext.Plates.AddAsync(plate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreatePotAsync(string potName, int pressId)
        {
            var pot = new Pot
            {
                PotName = potName,
                PressId = pressId,
                Active = true
            };

			var newUserAction = new UserAction()
			{
				Username = await GFS.getUsernameString(),
				ActionDatetime = DateTime.Now,
				Action = 10 //Create - Pot
			};

			await _dbContext.UserActions.AddAsync(newUserAction);

			await _dbContext.Pots.AddAsync(pot);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreatePressAsync(string pressName, int phaseId)
        {
            
            var press = new Press
            {
                PressName = pressName,
                PhaseId = phaseId,
                Active = true
            };

			var newUserAction = new UserAction()
			{
				Username = await GFS.getUsernameString(),
				ActionDatetime = DateTime.Now,
				Action = 9 //Create - Press
			};

			await _dbContext.UserActions.AddAsync(newUserAction);

			await _dbContext.Presses.AddAsync(press);
            await _dbContext.SaveChangesAsync();
        }

        public async Task updatePress(Press selectedPress)
        {
            _dbContext.Presses.Update(selectedPress);

			var newUserAction = new UserAction()
			{
				Username = await GFS.getUsernameString(),
				ActionDatetime = DateTime.Now,
				Action = 15 //Edit - Press
			};

			await _dbContext.UserActions.AddAsync(newUserAction);

			await _dbContext.SaveChangesAsync();
        }

		public async Task updatePhase(Phase selectedPhase)
		{
			_dbContext.Phases.Update(selectedPhase);
			var newUserAction = new UserAction()
			{
				Username = await GFS.getUsernameString(),
				ActionDatetime = DateTime.Now,
				Action = 14 //Edit - Phase
			};

			await _dbContext.UserActions.AddAsync(newUserAction);
			await _dbContext.SaveChangesAsync();
		}

		public async Task updateBrand(Brand selectedBrand)
		{
			_dbContext.Brands.Update(selectedBrand);
			var newUserAction = new UserAction()
			{
				Username = await GFS.getUsernameString(),
				ActionDatetime = DateTime.Now,
				Action = 18 //Edit - Brand
			};

			await _dbContext.UserActions.AddAsync(newUserAction);
			await _dbContext.SaveChangesAsync();
		}

		public async Task updatePlate(Plate selectedPlate)
		{
			_dbContext.Plates.Update(selectedPlate);
			var newUserAction = new UserAction()
			{
				Username = await GFS.getUsernameString(),
				ActionDatetime = DateTime.Now,
				Action = 17 //Edit - Plate
			};

			await _dbContext.UserActions.AddAsync(newUserAction);
			await _dbContext.SaveChangesAsync();
		}

        public async Task updatePot(Pot selectedPot)
        {
            _dbContext.Pots.Update(selectedPot);
			var newUserAction = new UserAction()
			{
				Username = await GFS.getUsernameString(),
				ActionDatetime = DateTime.Now,
				Action = 16 //Edit - Pot
			};

			await _dbContext.UserActions.AddAsync(newUserAction);
			await _dbContext.SaveChangesAsync();
        }

        public async Task insertUserDataIntoDatabase(string addedName, string selectedRole, string password)
        {
            

                //var user = CreateUser();
                var user = new IdentityUser { UserName = addedName };
                var result = await _userManager.CreateAsync(user, password);

			if (result.Succeeded)
			{
				Debug.WriteLine("User created: " + user.UserName);
				await _userManager.AddToRoleAsync(user, selectedRole);
			}
			else
			{
				Debug.WriteLine("Error creating user: " + result.Errors);
			}
				foreach (var error in result.Errors)
                {
                    throw new Exception(error.Description);
                }

			

			var aspnetuser = await _dbContext.AspNetUsers.FirstOrDefaultAsync(u => u.UserName == addedName);

                if (aspnetuser != null)
                {
                    aspnetuser.Active = true;

                    _dbContext.AspNetUsers.Update(aspnetuser);
                    await _dbContext.SaveChangesAsync();
                }

			var newUserAction = new UserAction()
			{
				Username = await GFS.getUsernameString(),
				ActionDatetime = DateTime.Now,
				Action = 13 //Create - User
			};

			await _dbContext.UserActions.AddAsync(newUserAction);

		}

            
        

		public async Task updatePassword(AspNetUser selectedUser , string password)
		{
            var user = await _userManager.FindByNameAsync(selectedUser.UserName) ;

			if (user == null)
			{
				throw new Exception("User not found.");
			}

			var removePasswordResult = await _userManager.RemovePasswordAsync(user);

			if (!removePasswordResult.Succeeded) return;

			await _userManager.AddPasswordAsync(user, password);
		}

		public async Task updateUser(AspNetUser selectedUser, string selectedRole)
		{
			var user = await _userManager.FindByNameAsync(selectedUser.UserName);

			if (user == null)
			{
				throw new Exception("User not found.");
			}

			// Find the corresponding AspNetUser to update the 'Active' status
			var aspNetUser = await _dbContext.AspNetUsers.FirstOrDefaultAsync(u => u.UserName == selectedUser.UserName);
			if (aspNetUser == null)
			{
				throw new Exception("AspNetUser not found.");
			}

			aspNetUser.Active = selectedUser.Active;

			// Save changes to AspNetUser
			_dbContext.AspNetUsers.Update(aspNetUser);
			await _dbContext.SaveChangesAsync();

			// Update the role
			var currentRoles = await _userManager.GetRolesAsync(user);

			// Remove all existing roles
			await _userManager.RemoveFromRolesAsync(user, currentRoles);

			// Add the new role
			await _userManager.AddToRoleAsync(user, selectedRole);

			var newUserAction = new UserAction()
			{
				Username = await GFS.getUsernameString(),
				ActionDatetime = DateTime.Now,
				Action = 19 //Edit - User
			};

			await _dbContext.UserActions.AddAsync(newUserAction);
		}
	}
}
