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
        //private readonly BSTContext _context;
        private readonly IMapper _mapper;
        //private readonly ILogger<SiteService> _logger;

        //public SiteService(BSTContext context, IMapper mapper, ILogger<SiteService> logger)
        //{
        //    //_context = context;
        //    _mapper = mapper;
        //    _logger = logger;
        //}

        //public async Task<IEnumerable<SiteDTO>> GetAllSitesAsync()
        //{
        //    try
        //    {
        //        var sites = await _context.Sites.ToListAsync();
        //        return _mapper.Map<IEnumerable<SiteDTO>>(sites);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while getting all sites");
        //        throw;
        //    }
        //}

        public SiteService(ISiteRepository siteRepository, IMapper mapper)
        {
            _siteRepository = siteRepository ?? throw new ArgumentNullException(nameof(siteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        //public async Task<IEnumerable<SiteDTO>> GetAllSitesAsync()
        //{
        //    try
        //    {
        //        var sites = await _siteRepository.GetAllAsync();
        //        return _mapper.Map<IEnumerable<SiteDTO>>(sites);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

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

        public async Task<List<SiteTraineeDTO>> GetSiteTraineesAsync(string plantNo)
        {
            var trainees = await _siteRepository.GetSiteTraineesAsync(plantNo);
            return trainees.Select(t => new SiteTraineeDTO
            {
                PersonId = t.PersonId,
                Person = t.Person,
                Cattle = t.Cattle,
                Sheep = t.Sheep,
                Goats = t.Goats
            }).ToList();
        }

        public async Task<string> CreateSiteAsync(SiteDTO siteDto)
        {
            var site = _mapper.Map<Site>(siteDto);
            var createdSite = await _siteRepository.AddSiteAsync(site);
            //return _mapper.Map<SiteDTO>(createdSite);
            if (createdSite.ReturnCode == 1)
            {
                return "Site already exists. Please choose another Site / Plant No.";
            }

            return "Site added successfully.";
        }

        //public async Task<bool> CreateSiteAsync(SiteDTO siteDto)
        //{
        //    try
        //    {
        //        var existingSite = await _context.Sites.FindAsync(siteDto.PlantNo);
        //        if (existingSite != null)
        //        {
        //            return false; // Site already exists
        //        }

        //        var site = _mapper.Map<Site>(siteDto);
        //        _context.Sites.Add(site);
        //        await _context.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while creating a new site");
        //        throw;
        //    }
        //}
        //public Task<int> AddSiteAsync(Site site)
        //{
        //    // Delegate to DbContext
        //    return _context.AddSiteAsync(site);
        //}

        public async Task<bool> DeleteTraineeAsync(int personId)
        {
            return await _siteRepository.DeleteTraineeAsync(personId);
        }

    }
}
