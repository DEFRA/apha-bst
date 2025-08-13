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
        Task<IEnumerable<Site>> GetAllSitesAsync(string plantNo);
        Task<List<SiteTrainee>> GetSiteTraineesAsync(string plantNo);
        Task<bool> DeleteTraineeAsync(int personId);        
        Task<string> AddSiteAsync(Site site, string userName);

        // For deleting trainee by person ID
        // For getting person name by ID
        Task<string?> GetPersonNameByIdAsync(int personId);
        Task<string> UpdateSiteAsync(Site site);

    }
}
