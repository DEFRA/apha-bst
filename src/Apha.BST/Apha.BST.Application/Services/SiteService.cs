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
        public const string Exists = "EXISTS";

        public SiteService(ISiteRepository siteRepository, IMapper mapper)
        {
            _siteRepository = siteRepository ?? throw new ArgumentNullException(nameof(siteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<SiteDto>> GetAllSitesAsync(string plantNo)
        {          
                var sites = await _siteRepository.GetAllSitesAsync(plantNo);
                return _mapper.Map<IEnumerable<SiteDto>>(sites);            
        }

        public async Task<List<SiteTraineeDto>> GetSiteTraineesAsync(string plantNo)
        {
            var trainees = await _siteRepository.GetSiteTraineesAsync(plantNo);
            return trainees.Select(t => new SiteTraineeDto
            {
                PersonId = t.PersonId,
                Person = t.Person,
                Cattle = t.Cattle,
                SheepAndGoat = t.SheepAndGoat
            }).ToList();
        }

        //For Addsite
        public async Task<string> AddSiteAsync(SiteDto siteDto, string userName)
        {
            var site = _mapper.Map<Site>(siteDto);
            var createdSite = await _siteRepository.AddSiteAsync(site,userName);
            if (createdSite == Exists)
            {
                return "Site already exists. Please choose another Site / Plant No.";
            }

            return $"'{siteDto.Name}' saved as site";
            
        }
        public async Task<string> DeleteTraineeAsync(int personId)
        {
            // Fetch person name first
            var personName = await _siteRepository.GetPersonNameByIdAsync(personId);

            var deleted = await _siteRepository.DeleteTraineeAsync(personId);

            if (deleted)
            {
                return $"Trainee '{personName}' deleted successfully.";
            }
            else
            {
                return $"Trainee '{personName}' has training records. Delete them first if you wish to remove the person.";
            }
        }
        public async Task<string> UpdateSiteAsync(SiteInputDto siteInputDto)
        {
            var siteInput = _mapper.Map<SiteInput>(siteInputDto);
            await _siteRepository.UpdateSiteAsync(siteInput);
            return $"{siteInput.Name}  updated successfully";

        }
    }
}
