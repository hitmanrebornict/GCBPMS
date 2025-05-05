using ClosedXML.Excel;
using GCBPMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Policy;

namespace GCBPMS.Components.Services
{
	public class ReportPageServices
	{
		private PmsContext _dbContext;
		private IWebHostEnvironment environment;

		public ReportPageServices(PmsContext dbContext, IWebHostEnvironment Environment)
		{
			_dbContext = dbContext;
			environment = Environment;
		}

		public async Task<List<Phase>> getPhaseListAsync()
		{
			return await _dbContext.Phases
							.Include(p => p.Presses)
								.ThenInclude(p => p.Pots)
							.Where(p => p.Active)
							.ToListAsync();
		}

		public async Task<string> getPotHistoryUsageReport(Phase selectedPhase, Press selectedPress, Pot selectedPot, DateTime startDatetime, DateTime endDatetime)
		{
			var query = _dbContext.PlateHistoryUsages
						.Include(p => p.Plate)
						.Include(p => p.Pot)
							.ThenInclude(p => p.Press)
							.ThenInclude(p => p.Phase)
						.Include(p => p.Requests)
						.OrderBy(p => p.Id)
						.AsQueryable();

			if (selectedPhase != null)
			{
				query = query.Where(p => p.Pot.Press.Phase.Id == selectedPhase.Id);
			}

			if (selectedPress != null)
			{
				query = query.Where(p => p.Pot.Press.Id == selectedPress.Id);
			}

			if (selectedPot != null)
			{
				query = query.Where(p => p.Pot.Id == selectedPot.Id);
			}

			if (startDatetime != DateTime.MinValue && endDatetime != DateTime.MinValue)
			{
				query = query.Where(p => p.InstallDateTime >= startDatetime && p.RemoveDateTime <= endDatetime);
			}

			var potHistoryUsageList = await query.ToListAsync();

			var excelFileName = $"PotHistoryUsageReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

			var directoryPath = Path.Combine(environment.WebRootPath, "Reports");

			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			// Combine the directory path with the file name to get the full file path
			var excelFilePath = Path.Combine(directoryPath, excelFileName);

			using (var workbook = new XLWorkbook())
			{
				var worksheet = workbook.Worksheets.Add("Pot History Usage Report");

				worksheet.Cell(1, 1).Value = "Phase";
				worksheet.Cell(1, 2).Value = "Press";
				worksheet.Cell(1, 3).Value = "Pot";
				worksheet.Cell(1, 4).Value = "Plate Name";
				worksheet.Cell(1, 5).Value = "Install Date";
				worksheet.Cell(1, 6).Value = "Remove Date";
				worksheet.Cell(1, 7).Value = "Remove Reason";
				worksheet.Cell(1, 8).Value = "Age";

				for (int i = 0; i < potHistoryUsageList.Count; i++)
				{
					var test = potHistoryUsageList[i];

					// Phase Name
					worksheet.Cell(i + 2, 1).Value = test.Pot?.Press?.Phase?.PhaseName ?? "N/A";

					// Press Name
					worksheet.Cell(i + 2, 2).Value = test.Pot?.Press?.PressName ?? "N/A";

					// Pot Name
					worksheet.Cell(i + 2, 3).Value = test.Pot?.PotName ?? "N/A";

					// Plate Name
					worksheet.Cell(i + 2, 4).Value = test.Plate?.PlateName ?? "N/A";

					// Install Date
					worksheet.Cell(i + 2, 5).Value = test.InstallDateTime.ToString("dd/MM/yyyy");

					// Remove Date
					worksheet.Cell(i + 2, 6).Value = test.RemoveDateTime.HasValue
						? test.RemoveDateTime.Value.ToString("dd/MM/yyyy")
						: "N/A";

					//// Remove Reason
					worksheet.Cell(i + 2, 7).Value = test.Requests != null && test.Requests.Any()
	? string.Join(", ", test.Requests.Select(p => p.RepairReason ?? "Unknown"))
	: "N/A";

					// Age (calculated as days between Install Date and Remove Date)
					worksheet.Cell(i + 2, 8).Value = (test.RemoveDateTime.Value - test.InstallDateTime).Days.ToString()
						?? "N/A";
				}

				workbook.SaveAs(excelFilePath);

				var downloadPDFPath = $"/Reports/{excelFileName}";

				return downloadPDFPath;
			}
		}

		public async Task<string> getPlateReportAsync(Phase selectedPhase, Brand selectedBrand, string selectedPlateStatus)
		{

			var query = _dbContext.Plates
						.Include(p => p.Pots)
						.ThenInclude(p => p.Press)
						.ThenInclude(p => p.Phase)
						.Include(p => p.PlateBrandNavigation)
						.Include(p => p.PlateHistoryUsages)
						.OrderBy(p => p.PlateName)
						.AsQueryable();

			if (selectedPhase != null)
			{
				query = query.Where(p => p.PhaseType == selectedPhase.Id);
			}

			//if (selectedPress != null)
			//{
			//	query = query.Where(p => p.Pots.Any(p => p.PressId == selectedPress.Id));
			//}

			//if (selectedPot != null)
			//{
			//	query = query.Where(p => p.Pots.Any(p => p.Id == selectedPot.Id));
			//}

			// Filter by Brand
			if (selectedBrand != null)
			{
				query = query.Where(p => p.PlateBrandNavigation.Id == selectedBrand.Id);
			}

			// Filter by Plate Status
			if (!string.IsNullOrEmpty(selectedPlateStatus))
			{
				query = query.Where(p => p.PlateStatus == selectedPlateStatus);
			}


			var plateDetailList = await query.ToListAsync();

			var excelFileName = $"PlateReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

			var directoryPath = Path.Combine(environment.WebRootPath, "Reports");

			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			// Combine the directory path with the file name to get the full file path
			var excelFilePath = Path.Combine(directoryPath, excelFileName);

			using (var workbook = new XLWorkbook())
			{
				var worksheet = workbook.Worksheets.Add("Pot History Usage Report");

				worksheet.Cell(1, 1).Value = "Plate";
				worksheet.Cell(1, 2).Value = "Brand";
				worksheet.Cell(1, 3).Value = "Status";
				worksheet.Cell(1, 4).Value = "Install Datetime";
				worksheet.Cell(1, 5).Value = "Phase Type";
				worksheet.Cell(1, 6).Value = "Phase";
				worksheet.Cell(1, 7).Value = "Plate";
				worksheet.Cell(1, 8).Value = "Pot";

				for (int i = 0; i < plateDetailList.Count; i++)
				{
					var data = plateDetailList[i];

					// Phase Name
					worksheet.Cell(i + 2, 1).Value = data.PlateName ?? "N/A";

					// Press Name
					worksheet.Cell(i + 2, 2).Value = data.PlateBrandNavigation.BrandName ?? "N/A";

					// Pot Name
					worksheet.Cell(i + 2, 3).Value = data.PlateStatus ?? "N/A";

					// Plate Name
					worksheet.Cell(i + 2, 4).Value = data.PlateInstallDatetime.ToString("dd/MM/yyyy") ?? "N/A";

					// Install Date
					worksheet.Cell(i + 2, 5).Value = data.PhaseTypeNavigation.PhaseName ?? "N/A";

					if (data.PlateStatus == "Used")
					{
						// Find the associated pot, press, and phase
						var currentPot = data.Pots.FirstOrDefault();
						worksheet.Cell(i + 2, 6).Value = currentPot?.Press?.Phase?.PhaseName ?? "N/A";
						worksheet.Cell(i + 2, 7).Value = currentPot?.Press?.PressName ?? "N/A";
						worksheet.Cell(i + 2, 8).Value = currentPot?.PotName ?? "N/A";
					}
					else
					{
						worksheet.Cell(i + 2, 6).Value = "N/A";
						worksheet.Cell(i + 2, 7).Value = "N/A";
						worksheet.Cell(i + 2, 8).Value = "N/A";
					}
				}

				workbook.SaveAs(excelFilePath);

				var downloadPDFPath = $"/Reports/{excelFileName}";

				return downloadPDFPath;
			}
		}

		public async Task<string> getRepairReportAsync()
		{

			var query = _dbContext.Repairs
						.Include(r => r.RepairCosts)
						.Include(r => r.Request)
							.ThenInclude(r => r.Plate)
						.AsNoTracking()
						.AsQueryable();

			var repairList = await query.ToListAsync();

			// Generate Excel file
			var excelFileName = $"RepairReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
			var directoryPath = Path.Combine(environment.WebRootPath, "Report");

			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			var excelFilePath = Path.Combine(directoryPath, excelFileName);

			using (var workbook = new XLWorkbook())
			{
				var worksheet = workbook.Worksheets.Add("Repair Report");

				// Header Row
				worksheet.Cell(1, 1).Value = "Plate Name";
				worksheet.Cell(1, 2).Value = "Request Date";
				worksheet.Cell(1, 3).Value = "Finish Date";
				worksheet.Cell(1, 4).Value = "Repair Duration (Days)";
				worksheet.Cell(1, 5).Value = "Repair Reason";
				worksheet.Cell(1, 6).Value = "Repair Type";
				worksheet.Cell(1, 7).Value = "Repair By (Technician Name)";
				worksheet.Cell(1, 8).Value = "Repair Name";
				worksheet.Cell(1, 9).Value = "Repair Cost";
				worksheet.Cell(1, 10).Value = "Cost Remarks";
				worksheet.Cell(1, 11).Value = "Requestor";

				int currentRow = 2;

				foreach (var repair in repairList)
				{
					// If there are multiple costs, the first row for the repair gets the main details
					bool isFirstRow = true;

					foreach (var cost in repair.RepairCosts)
					{
						
							worksheet.Cell(currentRow, 1).Value = repair.Request.Plate?.PlateName ?? "N/A";
							worksheet.Cell(currentRow, 2).Value = repair.Request.RequestDatetime.ToString("dd/MM/yyyy");
							worksheet.Cell(currentRow, 3).Value = repair.FinishDatetime.HasValue
								? repair.FinishDatetime.Value.ToString("dd/MM/yyyy")
								: "N/A";
							worksheet.Cell(currentRow, 4).Value = repair.FinishDatetime.HasValue
								? (repair.FinishDatetime.Value - repair.StartDatetime).Days.ToString()
								: "N/A";
							worksheet.Cell(currentRow, 5).Value = repair.Request.RepairReason ?? "N/A";
							worksheet.Cell(currentRow, 6).Value = repair.RepairType ?? "N/A";
							worksheet.Cell(currentRow, 7).Value = repair.TechnicianName ?? "N/A";

						// Fill repair cost details
						worksheet.Cell(currentRow, 8).Value = cost.CostName ?? "N/A";
						worksheet.Cell(currentRow, 9).Value = cost.Cost;
							worksheet.Cell(currentRow, 10).Value = cost.CostRemark ?? "N/A";
						worksheet.Cell(currentRow, 11).Value = repair.Request.Requestor ?? "N/A";
						isFirstRow = false;
						

						currentRow++;
					}

					// If there are no costs, fill all repair details in a single row
					if (!repair.RepairCosts.Any())
					{
						worksheet.Cell(currentRow, 1).Value = repair.Request.Plate?.PlateName ?? "N/A";
						worksheet.Cell(currentRow, 2).Value = repair.Request.RequestDatetime.ToString("dd/MM/yyyy");
						worksheet.Cell(currentRow, 3).Value = repair.FinishDatetime.HasValue
							? repair.FinishDatetime.Value.ToString("dd/MM/yyyy")
							: "N/A";
						worksheet.Cell(currentRow, 4).Value = repair.FinishDatetime.HasValue
							? (repair.FinishDatetime.Value - repair.StartDatetime).Days.ToString()
							: "N/A";
						worksheet.Cell(currentRow, 5).Value = repair.Request.RepairReason ?? "N/A";
						worksheet.Cell(currentRow, 6).Value = repair.RepairType ?? "N/A";
						worksheet.Cell(currentRow, 7).Value = repair.TechnicianName ?? "N/A";

						worksheet.Cell(currentRow, 8).Value = "N/A";
						worksheet.Cell(currentRow, 9).Value = "N/A";
						worksheet.Cell(currentRow, 10).Value = "N/A";
						worksheet.Cell(currentRow, 11).Value = repair.Request.Requestor ?? "N/A";

						currentRow++;
					}
				}

				workbook.SaveAs(excelFilePath);
			}

			var downloadPDFPath = $"/Report/{excelFileName}";
			return downloadPDFPath;
		
        }
    }
}
