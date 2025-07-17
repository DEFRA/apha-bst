using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Interfaces;
using AutoMapper;

namespace Apha.BST.Application.Services
{
    public class PersonsService : IPersonsService
    {
        private readonly IPersonsRepository _personRepository;
        private readonly IMapper _mapper;
        public PersonsService(IPersonsRepository personRepository, IMapper mapper)
        {
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<IEnumerable<PersonsDTO>> GetAllPersonAsync()
        {
            try
            {
                var bookings = await _personRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<PersonsDTO>>(bookings);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
