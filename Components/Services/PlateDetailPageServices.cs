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
                    .OrderBy(p => p.PlateName)
                        .ThenBy(p => p.PlateStatus)
                .Include(p => p.PlateBrandNavigation)
                .Include(p => p.PhaseTypeNavigation)
                .AsNoTracking()
                .Where(p => p.Active == true).ToListAsync();
        }

        public async Task<(int, int, int)> getPlatePieChartData(List<Plate> plateList)
        {
            var usedPlateNumber = plateList.Count(p => p.PlateStatus == "Used");
            var inventoryPlateNumber = plateList.Count(p => p.PlateStatus == "Inventory");
            var reparingPlateNumber = plateList.Count(p => p.PlateStatus == "Repairing");

            return (usedPlateNumber, inventoryPlateNumber, reparingPlateNumber);
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

        public async Task<List<Repair>> getSelectedRepairListByPlateId(int plateId)
        {
            var selectedRepairList = await _dbContext.Repairs
                                        .Include(r => r.Request)
                                        .Include(r => r.SupplierDetails)
                                        .Include(r => r.RepairCosts)
                                        .Where(r => r.Request.PlateId == plateId)
                                        .ToListAsync();

            return selectedRepairList;
        }


    }
}   
