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
       // Task<IEnumerable<SiteDTO>> GetAllSitesAsync();
        Task<IEnumerable<SiteDTO>> GetAllSitesAsync(string plantNo);
        Task<List<SiteTraineeDTO>> GetSiteTraineesAsync(string plantNo);
        Task<bool> DeleteTraineeAsync(int personId);
        //Task CreateSiteAsync(SiteDTO siteDto);
        //Task<SiteDTO> CreateSiteAsync(SiteDTO siteDto);
        Task<string> CreateSiteAsync(SiteDTO siteDto);

    }
}
