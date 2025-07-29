using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Core.Entities;


namespace Apha.BST.Application.Interfaces
{
    public interface ISiteService
    {
        Task<IEnumerable<SiteDTO>> GetAllSitesAsync(string plantNo);
        Task<List<SiteTraineeDTO>> GetSiteTraineesAsync(string plantNo);
        Task<string> DeleteTraineeAsync(int personId);       
        Task<string> AddSiteAsync(SiteDTO siteDto);

    }
}
