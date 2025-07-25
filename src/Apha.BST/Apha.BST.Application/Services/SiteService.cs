using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Apha.BST.Application.Services
{
    public class SiteService: ISiteService
    {

        private readonly ISiteRepository _siteRepository;
        private readonly IMapper _mapper;            

        public SiteService(ISiteRepository siteRepository, IMapper mapper)
        {
            _siteRepository = siteRepository ?? throw new ArgumentNullException(nameof(siteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<SiteDTO>> GetAllSitesAsync(string plantNo)
        {
            try
            {
                var sites = await _siteRepository.GetAllSitesAsync(plantNo);
                return _mapper.Map<IEnumerable<SiteDTO>>(sites);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //public async Task<IEnumerable<SiteDTO>> GetAllSitesAsync(string plantNo)
        //{
        //    var sites = await _siteRepository.GetAllSitesAsync(plantNo);

        //    var grouped = sites
        //        .GroupBy(s => new { s.PlantNo, s.Name })
        //        .Select(g => new SiteDTO
        //        {
        //            PlantNo = g.Key.PlantNo,
        //            Name = g.Key.Name,
        //            AddressLine1 = g.First().AddressLine1,
        //            AddressLine2 = g.First().AddressLine2,
        //            AddressTown = g.First().AddressTown,
        //            AddressCounty = g.First().AddressCounty,
        //            AddressPostCode = g.First().AddressPostCode,
        //            Telephone = g.First().Telephone,
        //            Fax = g.First().Fax,
        //            Ahvla = string.Join(", ", g.Select(x => x.Ahvla).Distinct())
        //        });

        //    return grouped;
        //}

        public async Task<List<SiteTraineeDTO>> GetSiteTraineesAsync(string plantNo)
        {
            var trainees = await _siteRepository.GetSiteTraineesAsync(plantNo);
            return trainees.Select(t => new SiteTraineeDTO
            {
                PersonId = t.PersonId,
                Person = t.Person,
                Cattle = t.Cattle,
                Sheep = t.Sheep,
                Goats = t.Goats,                
            }).ToList();
        }

        public async Task<string> CreateSiteAsync(SiteDTO siteDto)
        {
            var site = _mapper.Map<Site>(siteDto);
            var createdSite = await _siteRepository.AddSiteAsync(site);          
            if (createdSite.ReturnCode == 1)
            {
                return "Site already exists. Please choose another Site / Plant No.";
            }

            return "Site added successfully.";
        }      

        public async Task<bool> DeleteTraineeAsync(int personId)
        {
            return await _siteRepository.DeleteTraineeAsync(personId);
        }

    }
}
