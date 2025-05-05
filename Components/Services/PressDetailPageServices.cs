using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Dynamic.Core;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace GCBPMS.Components.Services
{
    public class PressDetailPageServices
    {
        private PmsContext _dbContext;
        private IWebHostEnvironment _environment;

        public PressDetailPageServices(PmsContext dbContext, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _environment = environment;
        }
        public async Task<List<Pot>> getPotsListAsync()
        {
            return await _dbContext.Pots
                     .Include(p => p.Plate)
                         .ThenInclude(p => p.PlateBrandNavigation)
                     .Include(p => p.Press)
                         .ThenInclude(p => p.Phase)
                     .AsNoTracking()
                     .Where(p => p.Active)
                     .ToListAsync();
        }

        public (int, int) getPotsUsageDataForPieChart(List<Pot> potList)
        {
            var occupiedNumber = potList.Count(p => p.Plate != null);
            var emptyNumber = potList.Count() - occupiedNumber;
            return (occupiedNumber, emptyNumber);
        }

       

        public async Task<List<Pot>> getPotsByPressID(int pressID)
        {
            return await _dbContext.Pots
            .Include(p => p.Plate)  
            .Where(p => p.PressId == pressID)
            
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<List<Press>> getPressesByPhaseID(int phaseID)
        {
            return await _dbContext.Presses
                .Where(p => p.PhaseId == phaseID)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task CreatePotsAsync(Phase selectedPhase, int pressID){
            for(int i = 1; i <= selectedPhase.PotNumber; i++){
                var potLeft = new Pot{
                    PotName = $"{i}L",
                    PressId = pressID,
                    Active = true
                };
                await _dbContext.Pots.AddAsync(potLeft);

                var potRight = new Pot{
                    PotName = $"{i}R", 
                    PressId = pressID,
                    Active = true
                };
                await _dbContext.Pots.AddAsync(potRight);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<PlateHistoryUsage>> getPotUsageHistoryByPotID(int potID){
            return await _dbContext.PlateHistoryUsages
                .Include(p => p.Plate)
                    .ThenInclude(p => p.PlateBrandNavigation)
                .Where(p => p.PotId == potID)
                .OrderBy(p => p.RemoveDateTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public string exportToExcel(List<Pot> potsList)
        {
            var excelFileName = $"PotOccupancyReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            var directoryPath = Path.Combine(_environment.WebRootPath, "Reports");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Combine the directory path with the file name to get the full file path
            var excelFilePath = Path.Combine(directoryPath, excelFileName);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Pot Occupancy Report");

                worksheet.Cell(1, 1).Value = "Phase";
                worksheet.Cell(1, 2).Value = "Press";
                worksheet.Cell(1, 3).Value = "Pot";
                worksheet.Cell(1, 4).Value = "Plate";

                int rowCount = 2;
                foreach(var selectedPot in potsList)
                {
                    worksheet.Cell(rowCount, 1).Value = selectedPot.Press?.Phase?.PhaseName ?? "Empty";
                    worksheet.Cell(rowCount, 2).Value = selectedPot.Press.PressName;
                    worksheet.Cell(rowCount, 3).Value = selectedPot.PotName;
                    worksheet.Cell(rowCount, 4).Value = selectedPot.Plate?.PlateName ?? "Empty";
                    rowCount++;
                }

                workbook.SaveAs(excelFilePath);

                var downloadPDFPath = $"/Reports/{excelFileName}";

                return downloadPDFPath;
            }
        }
    }
}
