using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;

namespace Apha.BST.Core.Interfaces
{
    public interface ISiteRepository
    {
        //Task<IEnumerable<Site>> GetAllAsync();
        Task<IEnumerable<Site>> GetAllSitesAsync(string plantNo);
        Task<List<SiteTrainee>> GetSiteTraineesAsync(string plantNo);
        Task<bool> DeleteTraineeAsync(int personId);
        //Task<Site> AddSiteAsync(Site site);
        Task<AddSiteResult> AddSiteAsync(Site site);

    }
}
