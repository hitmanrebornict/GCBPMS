using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace GCBPMS.Components.Services
{
    public class PressDetailPageServices
    {
        private PmsContext _dbContext;

        public PressDetailPageServices(PmsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Pot>> getPotsByPressID(int pressID)
        {
            return await _dbContext.Pots
            .Include(p => p.Plate)  
            .Where(p => p.PressId == pressID)
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<List<Press>> getPressesByPhaseID(int phaseID)
        {
            return await _dbContext.Presses
                .Where(p => p.PhaseId == phaseID)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task CreatePotsAsync(Phase selectedPhase, int pressID){
            for(int i = 1; i <= selectedPhase.PotNumber; i++){
                var potLeft = new Pot{
                    PotName = $"{i}L",
                    PressId = pressID,
                    Active = true
                };
                await _dbContext.Pots.AddAsync(potLeft);

                var potRight = new Pot{
                    PotName = $"{i}R", 
                    PressId = pressID,
                    Active = true
                };
                await _dbContext.Pots.AddAsync(potRight);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<PlateHistoryUsage>> getPotUsageHistoryByPotID(int potID){
            return await _dbContext.PlateHistoryUsages
                .Include(p => p.Plate)
                .Where(p => p.PotId == potID)
                .OrderBy(p => p.RemoveDateTime)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
