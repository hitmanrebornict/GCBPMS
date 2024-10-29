using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;

namespace GCBPMS.Components.Services
{
	public class GlobalFunction
	{
		private PmsContext _dbContext;
		public GlobalFunction(PmsContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<Phase>> getActivePhase()
		{
			return await _dbContext.Phases.Where(p => p.Active == true).ToListAsync();
		}

	}
}
