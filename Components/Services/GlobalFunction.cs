using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;

namespace GCBPMS.Components.Services
{
	public class GlobalFunction
	{
		private PmsContext _dbContext;
		public List<Phase> phaseList = new List<Phase>();
		public GlobalFunction(PmsContext dbContext)
		{
			_dbContext = dbContext;
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
			return await _dbContext.Pots.Where(p => p.PressId == pressId).ToListAsync();
		}

		public async Task<List<Plate>> getPlatesInInventory(){
			return await _dbContext.Plates.Where(p => p.Active == true && p.PlateStatus == "Inventory").ToListAsync();
		}

		public async Task<Plate> getPlateInPot(int potId){
            return await _dbContext.Plates
            .Include(p => p.Pots)
            .Where(p => p.Active == true && p.Pots.Any(p => p.Id == potId)).FirstOrDefaultAsync();
        }
	}
}
