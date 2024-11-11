using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GCBPMS.Components.Services
{
    public class MaintenanceRequestServices
    {
        private PmsContext _dbContext;

        public MaintenanceRequestServices(PmsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Request>> getRequests()
        {
            return await _dbContext.Requests
                        .Include(r => r.Plate)
                        .Where(r => r.RequestStatus == "Requested")
                        .OrderBy(r => r.RequestDatetime)
                        .ToListAsync();
        }

        public async Task<Request> getRequestById(int id)
        {
            return await _dbContext.Requests
                        .Include(r => r.Plate)
                        .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task insertIntoSupplierRepair(SupplierDetail supplierDetail, Request request){
            var repair = new Repair {
                SupplierDetails = supplierDetail, // Let EF handle the relationship
                RepairType = "Supplier",
                StartDatetime = DateTime.Now,
                RepairStatus = "Repairing",
                RequestId = request.Id
            };

            request.RequestStatus = "Accepted";


            await _dbContext.Repairs.AddAsync(repair);
            await _dbContext.SaveChangesAsync(); // Single database save
        }

        public async Task insertIntoTechnicianRepair(Repair repair, Request request){
            repair.RepairType = "In-house";
            repair.RepairStatus = "Repairing";
            repair.StartDatetime = DateTime.Now;
            repair.RequestId = request.Id;

            request.RequestStatus = "Accepted";
            
            await _dbContext.Repairs.AddAsync(repair); // Use AddAsync for better performance
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Repair>> getRepairList(){
            return await _dbContext.Repairs
                        .Include(r => r.Request)
                            .ThenInclude(r => r.Plate)
                        .Include(r => r.SupplierDetails)
                        .Where(r => r.RepairStatus == "Repairing" || r.RepairStatus == "Cost-Record")
                        .OrderByDescending(r => r.StartDatetime)
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
            _dbContext.Update(selectedRepair);
            await _dbContext.SaveChangesAsync();
        }

        public async Task updateSupplierRepair(Repair selectedRepair)
        {
            selectedRepair.Request.Plate.PlateStatus = "Inventory";
            selectedRepair.Request.RequestStatus = "Completed";
            selectedRepair.FinishDatetime = DateTime.Now;
            selectedRepair.RepairStatus = "Cost-Record";
            _dbContext.Update(selectedRepair);
            await _dbContext.SaveChangesAsync();
        }

        public async Task recordCost(Repair repair, RepairCost repairCost){
            repairCost.Id = 0;
            repairCost.RepairId = repair.Id;
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
