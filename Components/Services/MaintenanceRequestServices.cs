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

    }
}
