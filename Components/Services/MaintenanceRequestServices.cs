using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GCBPMS.Components.Services
{
    public class MaintenanceRequestServices
    {
        private PmsContext _dbContext;
        private GlobalFunction GF;

        public MaintenanceRequestServices(PmsContext dbContext, GlobalFunction gf)
        {
            _dbContext = dbContext;
            GF = gf;
        }

        public async Task<List<Request>> getRequests()
        {
            return await _dbContext.Requests
                        .Include(r => r.Plate)
                            .ThenInclude(r => r.PlateBrandNavigation)
                        .Where(r => r.RequestStatus == "Requested")
                        .OrderBy(r => r.RequestDatetime)
                        .AsNoTracking()
                        .ToListAsync();
        }

        public async Task<Request> getRequestById(int id)
        {
            return await _dbContext.Requests
                        .Include(r => r.Plate)
                            .ThenInclude(r => r.PlateBrandNavigation)
                        .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task insertIntoSupplierRepair(SupplierDetail supplierDetail, Request request){

            var repair = new Repair
            {
                SupplierDetails = supplierDetail, // Let EF handle the relationship
                RepairType = "Supplier",
                StartDatetime = DateTime.Now,
                RepairStatus = "Repairing",
                RequestId = request.Id,
                AcceptedBy = await GF.getUsernameString(),
            };

            request.RequestStatus = "Accepted";

			var newUserAction = new UserAction()
			{
				Username = await GF.getUsernameString(),
				ActionDatetime = DateTime.Now,
				PlateId = request.PlateId,
				Action = 4 //Accept Request
			};
			await _dbContext.UserActions.AddAsync(newUserAction);


			await _dbContext.Repairs.AddAsync(repair);
            await _dbContext.SaveChangesAsync(); // Single database save
        }

        public async Task insertIntoTechnicianRepair(Repair repair, Request request){
			repair.RepairType = "In-house";
            repair.RepairStatus = "Repairing";
            repair.StartDatetime = DateTime.Now;
            repair.RequestId = request.Id;
            repair.AcceptedBy = await GF.getUsernameString();

			request.RequestStatus = "Accepted";

			var newUserAction = new UserAction()
			{
				Username = await GF.getUsernameString(),
				ActionDatetime = DateTime.Now,
				PlateId = request.PlateId,
				Action = 4 //Accept Request
			};
			await _dbContext.UserActions.AddAsync(newUserAction);

			await _dbContext.Repairs.AddAsync(repair); // Use AddAsync for better performance
            await _dbContext.SaveChangesAsync();
        }

		public async Task<List<Repair>> getRepairList()
		{
			return await _dbContext.Repairs
						.Include(r => r.Request)
							.ThenInclude(r => r.Plate)
                            .ThenInclude(r => r.PlateBrandNavigation)
						.Include(r => r.SupplierDetails)
						.OrderBy(r => r.RepairStatus == "Repairing" ? 1
								   : r.RepairStatus == "Cost-Record" ? 2
								   : r.RepairStatus == "Completed" ? 3
								   : 4) // Default case, if any other statuses exist
						.ThenByDescending(r => r.StartDatetime)
						.AsNoTracking()
						.ToListAsync();
		}

		public async Task<Repair> getRepairById(int id){
            return await _dbContext.Repairs
                            .Include(r => r.SupplierDetails)
                            .Include(r => r.RepairCosts)
                            .Include(r => r.Request)
                            .Include(r => r.Request.Plate)
                        .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task updateTechnicianRepair(Repair selectedRepair)
        {
            selectedRepair.Request.Plate.PlateStatus = "Inventory";
            selectedRepair.Request.RequestStatus = "Completed";
            selectedRepair.FinishDatetime = DateTime.Now;
            selectedRepair.RepairStatus = "Completed";
            selectedRepair.CompletedBy = await GF.getUsernameString();

			var newUserAction = new UserAction()
			{
				Username = await GF.getUsernameString(),
				ActionDatetime = DateTime.Now,
				PlateId = selectedRepair.Request.PlateId,
				Action = 5 //Complete Request
			};
			await _dbContext.UserActions.AddAsync(newUserAction);

			_dbContext.Update(selectedRepair);
            await _dbContext.SaveChangesAsync();
        }

        public async Task updateSupplierRepair(Repair selectedRepair)
        {
            selectedRepair.Request.Plate.PlateStatus = "Inventory";
            selectedRepair.Request.RequestStatus = "Completed";
            selectedRepair.FinishDatetime = DateTime.Now;
            selectedRepair.RepairStatus = "Cost-Record";
			selectedRepair.CompletedBy = await GF.getUsernameString();
			_dbContext.Update(selectedRepair);

			var newUserAction = new UserAction()
			{
				Username = await GF.getUsernameString(),
				ActionDatetime = DateTime.Now,
				PlateId = selectedRepair.Request.PlateId,
				Action = 5 //Complete Request
			};
			await _dbContext.UserActions.AddAsync(newUserAction);

			await _dbContext.SaveChangesAsync();
        }

        public async Task recordCost(Repair repair, RepairCost repairCost){
            repairCost.Id = 0;
            repairCost.RepairId = repair.Id;

			var newUserAction = new UserAction()
			{
				Username = await GF.getUsernameString(),
				ActionDatetime = DateTime.Now,
				PlateId = repair.Request.PlateId,
				Action = 6 //Cost Record
			};
			await _dbContext.UserActions.AddAsync(newUserAction);

			await _dbContext.RepairCosts.AddAsync(repairCost);
            await _dbContext.SaveChangesAsync();
        }

        public async Task completeRecordCost(Repair repair){
            repair.RepairStatus = "Completed";
            _dbContext.Update(repair);
            await _dbContext.SaveChangesAsync();
        }

    }
}
