using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;

namespace Apha.BST.Application.Interfaces
{
    public interface IReportService
    {
        Task<List<SiteReportDto>> GetSiteReportsAsync();
        Task<List<TrainerReportDto>> GetTrainerReportsAsync();
        Task<List<PeopleReportDto>> GetPeopleReportsAsync();
        Task<List<TrainingReportDto>> GetTrainingReportsAsync();
        Task<List<AphaReportDto>> GetAphaReportsAsync();
        Task<(byte[] FileContents, string FileName)> GenerateExcelReportAsync();
    }
}
