using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;

namespace GCBPMS.Components.Services
{
    public class ProdCreateRequestPageServices
    {
        private PmsContext _dbContext;
        private GlobalFunction GF;

        public ProdCreateRequestPageServices(PmsContext dbContext, GlobalFunction gf)
        {
            _dbContext = dbContext;
            GF = gf;
        }
    
        public async Task createInventoryRepairRequest(string repairReason, string repairRemark, Plate selectedPlate)
        {
            var currentTime = DateTime.Now;

            selectedPlate.PlateStatus = "Repairing"; 
            _dbContext.Plates.Update(selectedPlate);

			var newUserAction = new UserAction()
			{
				Username = await GF.getUsernameString(),
				ActionDatetime = DateTime.Now,
				PlateId = selectedPlate.Id,
				Action = 3 //Request
			};
			await _dbContext.UserActions.AddAsync(newUserAction);

			var request = new Request
            {
                PlateId = selectedPlate.Id,
                RequestDatetime = currentTime,
                RepairReason = repairReason,
                RepairRemark = repairRemark,
				Requestor = await GF.getUsernameString()
			};

            // Batch the operations together
            await _dbContext.Requests.AddAsync(request);
            await _dbContext.SaveChangesAsync();
        }

        public async Task createMachineRequest(Pot selectedPot, string repairReason, string repairRemark, Plate selectedPlate)
        {
            var currentTime = DateTime.Now;

            var changeReasonString = $"Repair - {repairReason}";
            var newPlateHistoryUsage = new PlateHistoryUsage
            {
                PlateId = selectedPlate.Id,
                PotId = selectedPot.Id,
                InstallDateTime = selectedPot.InstallDatetime.Value,
                RemoveDateTime = currentTime,
                ChangeReason = changeReasonString,
            };

            // Prepare all entities for a single SaveChanges call
            selectedPlate.PlateStatus = "Repairing";
            _dbContext.Plates.Update(selectedPlate);

            // Add and save plate history usage first to get its ID
            await _dbContext.PlateHistoryUsages.AddAsync(newPlateHistoryUsage);

			var newUserAction = new UserAction()
			{
				Username = await GF.getUsernameString(),
				ActionDatetime = DateTime.Now,
				PlateId = selectedPlate.Id,
				Action = 3 //Request
			};
			await _dbContext.UserActions.AddAsync(newUserAction);

			var request = new Request
            {
                PlateId = selectedPlate.Id,
                RequestDatetime = currentTime,
                RepairReason = repairReason,
                RepairRemark = repairRemark,
                PlateHistoryUsage = newPlateHistoryUsage, // Add the generated ID
                Requestor = await GF.getUsernameString()
            };

            await _dbContext.Requests.AddAsync(request);

            // Update pot
            selectedPot.InstallDatetime = null;
            selectedPot.PlateId = null;
     
            _dbContext.Pots.Update(selectedPot);

           

            // Add request and save remaining changes
            
            await _dbContext.SaveChangesAsync();
        }
    }
}
