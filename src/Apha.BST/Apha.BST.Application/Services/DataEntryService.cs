using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Repositories;
using AutoMapper;

namespace Apha.BST.Application.Services
{
    public class DataEntryService:IDataEntryService
    {
        private readonly IDataEntryRepository _dataEntryRepository;
        public DataEntryService(IDataEntryRepository dataEntryRepository, IMapper mapper)
        {
            _dataEntryRepository = dataEntryRepository ?? throw new ArgumentNullException(nameof(dataEntryRepository));
            //_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<bool> CanEditPage(string action)
        {
            var sites = await _dataEntryRepository.CanEditPage(action);
            return true;

        }
    }
}
