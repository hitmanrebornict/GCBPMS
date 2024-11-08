﻿using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;

namespace GCBPMS.Components.Services
{
    public class ProdCreateRequestPageServices
    {
        private PmsContext _dbContext;

        public ProdCreateRequestPageServices(PmsContext dbContext)
        {
            _dbContext = dbContext;
        }
    
        public async Task createInventoryRepairRequest(string repairReason, string repairRemark, Plate selectedPlate)
        {
            var currentTime = DateTime.Now;

            selectedPlate.PlateStatus = "Repairing"; 
            _dbContext.Plates.Update(selectedPlate);

            var request = new Request
            {
                PlateId = selectedPlate.Id,
                RequestDatetime = currentTime,
                RepairReason = repairReason,
                RepairRemark = repairRemark,
            };

            // Batch the operations together
            await _dbContext.Requests.AddAsync(request);
            await _dbContext.SaveChangesAsync();
        }

        public async Task createMachineRequest(int selectedPot, string repairReason, string repairRemark, Plate selectedPlate)
        {
            var currentTime = DateTime.Now;

            // Get pot in a single query with tracking
            var pot = await _dbContext.Pots
                .Where(p => p.Id == selectedPot)
                .FirstAsync();

            // Prepare all entities for a single SaveChanges call
            selectedPlate.PlateStatus = "Repairing";
            _dbContext.Plates.Update(selectedPlate);

            var newPlateHistoryUsage = new PlateHistoryUsage
            {
                PlateId = selectedPlate.Id,
                PotId = pot.Id,
                InstallDateTime = pot.InstallDatetime.Value,
                RemoveDateTime = currentTime
            };

            var request = new Request
            {
                PlateId = selectedPlate.Id,
                RequestDatetime = currentTime,
                RepairReason = repairReason,
                RepairRemark = repairRemark,
            };

            // Update pot
            pot.InstallDatetime = null;
            pot.PlateId = null;

            // Add new entities
            await _dbContext.PlateHistoryUsages.AddAsync(newPlateHistoryUsage);
            await _dbContext.Requests.AddAsync(request);

            // Save all changes in a single transaction
            await _dbContext.SaveChangesAsync();
        }
    }
}
