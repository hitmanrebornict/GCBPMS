using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GCBPMS.Components.Services
{
    public class PlateAssignService
    {
        private PmsContext _dbContext;
        private GlobalFunction GF;

        public PlateAssignService(PmsContext dbContext, GlobalFunction gf)
        {
            _dbContext = dbContext;
            GF = gf;
        }   

        public async Task<List<Press>> getActivePressesJoinPhase(){
            return await _dbContext.Presses.Include(p => p.Phase).Where(p => p.Active == true).ToListAsync();
        }

        public async Task<Plate> getPlateInPot(int potId){
            return await _dbContext.Plates
            .Include(p => p.Pots)
            .Where(p => p.Active == true && p.Pots.Any(p => p.Id == potId)).FirstOrDefaultAsync();
        }

        public async Task assignPlateToPot(int potId, int plateId, DateTime InstallDateTime){
            var pot = await _dbContext.Pots.FindAsync(potId);
            var plate = await _dbContext.Plates.FindAsync(plateId);
            pot.PlateId = plateId;
            pot.InstallDatetime = InstallDateTime;
            plate.PlateStatus = "Used";

            var newUserAction = new UserAction()
            {
                Username = await GF.getUsernameString(),
                ActionDatetime = DateTime.Now,
                PlateId = plateId,
                Action = 1 //Assign
            };
            await _dbContext.UserActions.AddAsync(newUserAction);
            await _dbContext.SaveChangesAsync();
        }

        public async Task removePlateFromPot(int potId, int plateId, DateTime RemoveDateTime)
        {
            var currentTime = RemoveDateTime;
            var phuInstallDatetime = new DateTime();
			var pot = await _dbContext.Pots.FindAsync(potId);
			var plate = await _dbContext.Plates.FindAsync(plateId);
            if(pot.InstallDatetime != null)
            {
				phuInstallDatetime = pot.InstallDatetime.Value;
			}
            else
            {
                phuInstallDatetime = currentTime;
            }
            
			var newPlateHistoryUsage = new PlateHistoryUsage
			{
				PlateId = plate.Id,
				PotId = potId,
				RemoveDateTime = currentTime,
				InstallDateTime = phuInstallDatetime,
				ChangeReason = "Remove"
			};

			await _dbContext.PlateHistoryUsages.AddAsync(newPlateHistoryUsage);

			var newUserAction = new UserAction()
			{
				Username = await GF.getUsernameString(),
				ActionDatetime = DateTime.Now,
				PlateId = plateId,
				Action = 7 //Remove
			};
			await _dbContext.UserActions.AddAsync(newUserAction);

			pot.PlateId = null;
            pot.InstallDatetime = null;
            plate.PlateStatus = "Inventory";
			await _dbContext.SaveChangesAsync();
		}

        public async Task switchPlateBetweenPots(int potId, int switchPotId, DateTime curRemoveDateTime)
        {
          
            var pots = await _dbContext.Pots
                .Where(p => p.Id == potId || p.Id == switchPotId)
                .ToListAsync();

            if (pots.Count != 2)
            {
                throw new ArgumentException("Invalid pot IDs provided.");
            }

            var pot = pots.First(p => p.Id == potId);
            var switchPot = pots.First(p => p.Id == switchPotId);

            var plateUsageHistories = new List<PlateHistoryUsage>
            {
                new PlateHistoryUsage
                {
                    PlateId = pot.PlateId.Value,
                    PotId = potId,
                    RemoveDateTime = curRemoveDateTime,
                    InstallDateTime  = pot.InstallDatetime.Value,
                    ChangeReason = "Swtich"
                },
                new PlateHistoryUsage
                {
                    PlateId = switchPot.PlateId.Value,
                    PotId = switchPotId,
                    RemoveDateTime = curRemoveDateTime,
                    InstallDateTime = switchPot.InstallDatetime.Value,
					ChangeReason = "Swtich"
				}
            };

            _dbContext.PlateHistoryUsages.AddRange(plateUsageHistories);

			var newUserAction = new UserAction()
			{
				Username = await GF.getUsernameString(),
				ActionDatetime = DateTime.Now,
				PlateId = pot.PlateId.Value,
				Action = 2 //Switch
			};

			await _dbContext.UserActions.AddAsync(newUserAction);

			var tempPlateId = pot.PlateId;
            pot.PlateId = switchPot.PlateId;
            pot.InstallDatetime = curRemoveDateTime;

            switchPot.PlateId = tempPlateId;
            switchPot.InstallDatetime = curRemoveDateTime;

            await _dbContext.SaveChangesAsync();
        }
    }
}
