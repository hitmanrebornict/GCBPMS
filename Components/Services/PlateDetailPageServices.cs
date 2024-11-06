using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GCBPMS.Components.Services
{
    public class PlateDetailPageServices
    {
        private PmsContext _dbContext;

        public PlateDetailPageServices(PmsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Plate>> getPlateList()
        {
            return await _dbContext.Plates
                .Include(p => p.Pots)
                    .ThenInclude(p => p.Press)
                    .ThenInclude(p => p.Phase)
                .Include(p => p.PlateBrandNavigation)
                .Where(p => p.Active == true).ToListAsync();
        }

        public async Task<Plate> getPlateDetail(int id)
        {
            return await _dbContext.Plates
                .Include(p => p.Pots)
                    .ThenInclude(p => p.Press)
                    .ThenInclude(p => p.Phase)
                .Include(p => p.PlateBrandNavigation)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<PlateHistoryUsage>> getPlateHistoryUsage(int id)
        {
            return await _dbContext.PlateHistoryUsages
                .Include(p => p.Pot)
                    .ThenInclude(p => p.Press)
                .Where(p => p.PlateId == id).ToListAsync();
        }

        public async Task<int> getTimeUsedForPlate(Plate selectedPlate)
        {
            var potInstallDateTime = selectedPlate.Pots.FirstOrDefault()?.InstallDatetime;
            var currentDate = DateTime.Now;
            var timeUsed = currentDate - potInstallDateTime;
            return timeUsed.Value.Days;
        }
    }
}   
