using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Interfaces;
using AutoMapper;
using ClosedXML.Excel;

using Microsoft.Extensions.Configuration;

namespace Apha.BST.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ReportService(IReportRepository reportRepository,
                            IMapper mapper,
                            IConfiguration configuration)
        {
            _reportRepository = reportRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<List<AphaReportDto>> GetAphaReportsAsync()
        {
            var aphaReports = await _reportRepository.GetAphaReportsAsync();
            return _mapper.Map<List<AphaReportDto>>(aphaReports);
        }

        public async Task<List<PeopleReportDto>> GetPeopleReportsAsync()
        {
            var peopleReports = await _reportRepository.GetPeopleReportsAsync();
            return _mapper.Map<List<PeopleReportDto>>(peopleReports);
        }

        public async Task<List<SiteReportDto>> GetSiteReportsAsync()
        {
            var siteReports = await _reportRepository.GetSiteReportsAsync();
            return _mapper.Map<List<SiteReportDto>>(siteReports);
        }

        public async Task<List<TrainerReportDto>> GetTrainerReportsAsync()
        {
            var trainerReports = await _reportRepository.GetTrainerReportsAsync();
            return _mapper.Map<List<TrainerReportDto>>(trainerReports);
        }

        public async Task<List<TrainingReportDto>> GetTrainingReportsAsync()
        {
            var trainingReports = await _reportRepository.GetTrainingReportsAsync();
            return _mapper.Map<List<TrainingReportDto>>(trainingReports);
        }

        public async Task<(byte[] FileContents, string FileName)> GenerateExcelReportAsync()
        {
            string dateNow = $"BST_All_{DateTime.Today:yyyy-MM-dd}.xlsx";
            string? templatePath = _configuration["ReportSettings:TemplateFilePath"];

            if (string.IsNullOrWhiteSpace(templatePath))
                throw new InvalidOperationException("Report template file path is not configured.");

            // Get all report data
            var siteReports = await GetSiteReportsAsync();
            var trainerReports = await GetTrainerReportsAsync();
            var peopleReports = await GetPeopleReportsAsync();
            var trainingReports = await GetTrainingReportsAsync();
            var aphaReports = await GetAphaReportsAsync();

            using var memoryStream = new MemoryStream();
            using (var templateStream = File.OpenRead(templatePath))
            using (var workbook = new XLWorkbook(templateStream))
            {
                FillSitesSheet(workbook, siteReports, peopleReports);
                FillPeopleSheet(workbook, peopleReports);
                FillTrainersSheet(workbook, trainerReports , trainingReports);
                FillTrainingSheet(workbook, trainingReports);
                FillLocationsSheet(workbook, aphaReports);

                workbook.SaveAs(memoryStream);
            }

            return (memoryStream.ToArray(), dateNow);
        }

        #region Private helper methods for Excel generation

        private static void FillSitesSheet(XLWorkbook workbook, List<SiteReportDto> sites, List<PeopleReportDto> people)
        {
            var ws = workbook.Worksheets.FirstOrDefault(w => w.Name == "Sites") ?? workbook.Worksheets.Add("Sites");
            ws.Clear();
            ws.Cell(1, 1).Value = "Plant_No";
            ws.Cell(1, 2).Value = "Name";
            ws.Cell(1, 3).Value = "Address 1";
            ws.Cell(1, 4).Value = "Address 2";
            ws.Cell(1, 5).Value = "Town";
            ws.Cell(1, 6).Value = "County";
            ws.Cell(1, 7).Value = "Postcode";
            ws.Cell(1, 8).Value = "Telephone";
            ws.Cell(1, 9).Value = "Fax";
            ws.Cell(1, 10).Value = "Count";
            ws.Cell(1, 11).Value = "Run total";
            ws.Cell(1, 12).Value = "Start";
            ws.Range(1, 1, 1, 12).Style.Font.Bold = true;

            int row = 2;
            foreach (var site in sites)
            {
                ws.Cell(row, 1).Value = site.PlantNo;
                var nameCell = ws.Cell(row, 2);
                nameCell.Value = site.Name;
                // Find the first person whose LocationId matches the site.Name
                int peopleRow = 2 + people.FindIndex(p => string.Equals(p.LocationId, site.Name, StringComparison.OrdinalIgnoreCase));
                if (peopleRow >= 2)
                    nameCell.SetHyperlink(new XLHyperlink($"'People'!B{peopleRow}"));
                else
                    nameCell.SetHyperlink(new XLHyperlink("'People'!B1")); // fallback
                ws.Cell(row, 3).Value = site.Add1;
                ws.Cell(row, 4).Value = site.Add2;
                ws.Cell(row, 5).Value = site.Town;
                ws.Cell(row, 6).Value = site.County;
                ws.Cell(row, 7).Value = site.Postcode;
                ws.Cell(row, 8).Value = site.Phone;
                ws.Cell(row, 9).Value = site.Fax;
                ws.Cell(row, 10).Value = site.People;
                ws.Cell(row, 11).Value = site.RunTot;
                ws.Cell(row, 12).Value = site.Excel;
                row++;
            }
        }

        private static void FillPeopleSheet(XLWorkbook workbook, List<PeopleReportDto> people)
        {
            var ws = workbook.Worksheets.FirstOrDefault(w => w.Name == "People") ?? workbook.Worksheets.Add("People");
            ws.Clear();
            ws.Cell(1, 1).Value = "ID";
            ws.Cell(1, 2).Value = "Person";
            ws.Cell(1, 3).Value = "Location ID";
            ws.Cell(1, 4).Value = "APHA Location";
            ws.Cell(1, 5).Value = "Trainer";
            ws.Cell(1, 6).Value = "Trainee";
            ws.Cell(1, 7).Value = "Trained";
            // Make header row bold
            ws.Range(1, 1, 1, 7).Style.Font.Bold = true;

            int row = 2;
            foreach (var person in people)
            {
                ws.Cell(row, 1).Value = person.PersonId;
                ws.Cell(row, 2).Value = person.Person;
                ws.Cell(row, 3).Value = person.LocationId;
                ws.Cell(row, 4).Value = person.AphaLocation;
                ws.Cell(row, 5).Value = person.Trainer;
                ws.Cell(row, 6).Value = person.Trainee;
                ws.Cell(row, 7).Value = person.Trained;
                row++;
            }
        }

        private static void FillTrainersSheet(XLWorkbook workbook, List<TrainerReportDto> trainers, List<TrainingReportDto> trainings)
        {
            var ws = workbook.Worksheets.FirstOrDefault(w => w.Name == "Trainers") ?? workbook.Worksheets.Add("Trainers");
            ws.Clear();
            ws.Cell(1, 1).Value = "Trainer ID";
            ws.Cell(1, 2).Value = "Trainer";
            ws.Cell(1, 3).Value = "Trained";
            ws.Cell(1, 4).Value = "Run total";
            ws.Cell(1, 5).Value = "Start";
            ws.Range(1, 1, 1, 5).Style.Font.Bold = true;

            int row = 2;
            foreach (var trainer in trainers)
            {
                ws.Cell(row, 1).Value = trainer.ID;
                var idCell = ws.Cell(row, 2);
                idCell.Value = trainer.Trainer;
                // Find the first training where Trainer matches
                int trainingRow = 2 + trainings.FindIndex(t => string.Equals(t.Trainer, trainer.Trainer, StringComparison.OrdinalIgnoreCase));
                if (trainingRow >= 2)
                    idCell.SetHyperlink(new XLHyperlink($"'Training'!A{trainingRow}"));
                else
                    idCell.SetHyperlink(new XLHyperlink("'Training'!A1")); // fallback
                ws.Cell(row, 3).Value = trainer.Trained;
                ws.Cell(row, 4).Value = trainer.RunTot;
                ws.Cell(row, 5).Value = trainer.Excel;
                row++;
            }
        }

        private static void FillTrainingSheet(XLWorkbook workbook, List<TrainingReportDto> trainings)
        {
            var ws = workbook.Worksheets.FirstOrDefault(w => w.Name == "Training") ?? workbook.Worksheets.Add("Training");
            ws.Clear();
            ws.Cell(1, 1).Value = "Trainer";
            ws.Cell(1, 2).Value = "Trainee";
            ws.Cell(1, 3).Value = "Date trained";
            ws.Cell(1, 4).Value = "Species";
            ws.Cell(1, 5).Value = "VLA";
            ws.Range(1, 1, 1, 5).Style.Font.Bold = true;

            int row = 2;
            foreach (var training in trainings)
            {
                ws.Cell(row, 1).Value = training.Trainer;
                ws.Cell(row, 2).Value = training.Trainee;
                ws.Cell(row, 3).Value = training.TrainedOn;
                ws.Cell(row, 4).Value = training.TrainingAnimal;
                ws.Cell(row, 5).Value = training.VLA;
                row++;
            }
        }

        private static void FillLocationsSheet(XLWorkbook workbook, List<AphaReportDto> locations)
        {
            var ws = workbook.Worksheets.FirstOrDefault(w => w.Name == "Locations") ?? workbook.Worksheets.Add("APHA");
            ws.Clear();
            ws.Cell(1, 1).Value = "Location  ID";
            ws.Cell(1, 2).Value = "Location";
            ws.Cell(1, 3).Value = "APHA";
            ws.Range(1, 1, 1, 3).Style.Font.Bold = true;

            int row = 2;
            foreach (var location in locations)
            {
                ws.Cell(row, 1).Value = location.ID;
                ws.Cell(row, 2).Value = location.Location;
                ws.Cell(row, 3).Value = location.APHA;
                row++;
            }
        }

        #endregion
    }
}
