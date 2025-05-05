using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;

namespace GCBPMS.Components.Services
{
	public class MaintenanceDashboardServices
	{
		private PmsContext _dbContext;
		private GlobalFunction GF;

		public MaintenanceDashboardServices(PmsContext dbContext, GlobalFunction gf)
		{
			_dbContext = dbContext;
			GF = gf;
		}

		public async Task<List<PlateHistoryUsage>> getDataForChart()
		{
			return await _dbContext.PlateHistoryUsages
						.Include(phu => phu.Pot)
							.ThenInclude(phu => phu.Press)
							.ThenInclude(phu => phu.Phase)
						.ToListAsync();
		}

		public async Task<List<Press>> getPressList()
		{
			return await _dbContext.Presses
					.Where(p => p.Active)
					.OrderBy(r => r.PressName == "Press 1" ? 1
								   : r.PressName == "Press 2" ? 2
								   : r.PressName == "Press 3" ? 3
								   : r.PressName == "Press 4" ? 4
								   : r.PressName == "Press 5" ? 5
								   : r.PressName == "Press 6" ? 6
								   : r.PressName == "Press 7" ? 7
								   : r.PressName == "Press 8" ? 8
								   : r.PressName == "Press 9" ? 9
								   : r.PressName == "Press 10" ? 10
								   : r.PressName == "Press 11" ? 11
								   : r.PressName == "Press 12" ? 12
								   : r.PressName == "Press 13" ? 13
								   : r.PressName == "Press 14" ? 14
								   : r.PressName == "Press 15" ? 15
								   : r.PressName == "Press 16" ? 16
								   : r.PressName == "Press 17" ? 17
								   : r.PressName == "Press 18" ? 18
								   : r.PressName == "Press 19" ? 19
								   : r.PressName == "Press 20" ? 20
								   : r.PressName == "Press 21" ? 21
								   : r.PressName == "Press 22" ? 22
								   : r.PressName == "Press 23" ? 23
								   : r.PressName == "Press 24" ? 24
								   : r.PressName == "Press 25" ? 25
								   : r.PressName == "Press 26" ? 26
								   : 100)
					.ToListAsync();
		}

		
	}
}
