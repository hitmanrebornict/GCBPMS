using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GCBPMS.Components.Services
{
    public class MasterDataPageServices
    {
        private PmsContext _dbContext;
        public MasterDataPageServices(PmsContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Phase>> getPhasesAsync()
        {
            return await _dbContext.Phases.ToListAsync();
        }

        public async Task<List<Brand>> getBrandsAsync()
        {
            return await _dbContext.Brands.ToListAsync();
        }

        public async Task<List<Plate>> getPlatesAsync()
        {
            return await _dbContext.Plates.ToListAsync();
        }

        public async Task<List<Pot>> getPotsAsync()
        {
            return await _dbContext.Pots.ToListAsync();
        }

        public async Task<List<Press>> getPressesAsync()
        {
            return await _dbContext.Presses.ToListAsync();
        }
        public async Task CreatePhaseAsync(string phaseName, int potNumber)
        {
            var phase = new Phase
            {
                PhaseName = phaseName,
                PotNumber = potNumber,
                Active = true
            };
            await _dbContext.Phases.AddAsync(phase);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreateBrandAsync(string brandName)
        {
            var brand = new Brand
            {
                BrandName = brandName,
                Active = true
            };
            await _dbContext.Brands.AddAsync(brand);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreatePlateAsync(string plateName, int brandId, int phaseTypeId, DateTime installDate)
        {
            var plate = new Plate
            {
                PlateName = plateName,
                PlateBrand = brandId,
                PhaseType = phaseTypeId,
                PlateInstallDatetime = installDate,
                Active = true,
                PlateStatus = "New"
            };
            await _dbContext.Plates.AddAsync(plate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreatePotAsync(string potName, int pressId, int plateId)
        {
            var pot = new Pot
            {
                PotName = potName,
                PressId = pressId,
                PlateId = plateId,
                Active = true
            };
            await _dbContext.Pots.AddAsync(pot);
            await _dbContext.SaveChangesAsync();
        }

        public async Task CreatePressAsync(string pressName, int phaseId)
        {
            
            var press = new Press
            {
                PressName = pressName,
                PhaseId = phaseId,
                Active = true
            };
            await _dbContext.Presses.AddAsync(press);
            await _dbContext.SaveChangesAsync();
        }

    }
}
