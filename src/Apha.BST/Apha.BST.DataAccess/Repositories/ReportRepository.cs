using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.DataAccess.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly BstContext _context;

        public ReportRepository(BstContext context)
        {
            _context = context;
        }

        public async Task<List<AphaReport>> GetAphaReportsAsync()
        {
            return await _context.Set<AphaReport>()
                .FromSqlRaw("EXEC sp_Report_APHA")
                .ToListAsync();
        }

        public async Task<List<PeopleReport>> GetPeopleReportsAsync()
        {
            return await _context.Set<PeopleReport>()
                .FromSqlRaw("EXEC sp_Report_People")
                .ToListAsync();
        }

        public async Task<List<SiteReport>> GetSiteReportsAsync()
        {
            return await _context.Set<SiteReport>()
                .FromSqlRaw("EXEC sp_Report_Site")
                .ToListAsync();
        }

        public async Task<List<TrainerReport>> GetTrainerReportsAsync()
        {
            return await _context.Set<TrainerReport>()
                .FromSqlRaw("EXEC sp_Report_Trainer")
                .ToListAsync();
        }

        public async Task<List<TrainingReport>> GetTrainingReportsAsync()
        {
            return await _context.Set<TrainingReport>()
                .FromSqlRaw("EXEC sp_Report_Training")
                .ToListAsync();
        }
    }
}
