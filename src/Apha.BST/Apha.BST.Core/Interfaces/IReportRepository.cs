using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;

namespace Apha.BST.Core.Interfaces
{
    public interface IReportRepository
    {
        Task<List<AphaReport>> GetAphaReportsAsync();
        Task<List<PeopleReport>> GetPeopleReportsAsync();
        Task<List<SiteReport>> GetSiteReportsAsync();
        Task<List<TrainerReport>> GetTrainerReportsAsync();
        Task<List<TrainingReport>> GetTrainingReportsAsync();
    }
}
