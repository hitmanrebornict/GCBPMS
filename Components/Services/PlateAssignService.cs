using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GCBPMS.Components.Services
{
    public class PlateAssignService
    {
        private PmsContext _dbContext;

        public PlateAssignService(PmsContext dbContext)
        {
            _dbContext = dbContext;
        }   

        public async Task<List<Press>> getActivePressesJoinPhase(){
            return await _dbContext.Presses.Include(p => p.Phase).Where(p => p.Active == true).ToListAsync();
        }

        public async Task<Plate> getPlateInPot(int potId){
            return await _dbContext.Plates
            .Include(p => p.Pots)
            .Where(p => p.Active == true && p.Pots.Any(p => p.Id == potId)).FirstOrDefaultAsync();
        }

        public async Task assignPlateToPot(int potId, int plateId){
            var pot = await _dbContext.Pots.FindAsync(potId);
            var plate = await _dbContext.Plates.FindAsync(plateId);
            pot.PlateId = plateId;
            pot.InstallDatetime = DateTime.Now;
            plate.PlateStatus = "Used";
            await _dbContext.SaveChangesAsync();
        }

        public async Task switchPlateBetweenPots(int potId, int switchPotId)
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
                    RemoveDateTime = DateTime.Now,
                    InstallDateTime  = pot.InstallDatetime.Value,
                },
                new PlateHistoryUsage
                {
                    PlateId = switchPot.PlateId.Value,
                    PotId = switchPotId,
                    RemoveDateTime = DateTime.Now,
                    InstallDateTime = switchPot.InstallDatetime.Value,
                }
            };

            _dbContext.PlateHistoryUsages.AddRange(plateUsageHistories);

            var tempPlateId = pot.PlateId;
            pot.PlateId = switchPot.PlateId;
            pot.InstallDatetime = DateTime.Now;

            switchPot.PlateId = tempPlateId;
            switchPot.InstallDatetime = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }
    }
}
