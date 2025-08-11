using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Core.Entities;
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
        public async Task<IEnumerable<PersonsDto>> GetAllPersonAsync()
        {           
                var persons = await _personRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<PersonsDto>>(persons);            
        }
        public async Task<IEnumerable<PersonLookupDto>> GetPersonsForDropdownAsync()
        {
            var entities = await _personRepository.GetAllPersonsForDropdownAsync();
            return _mapper.Map<IEnumerable<PersonLookupDto>>(entities);
        }
        public async Task<string?> GetSiteByIdAsync(int personId)
        {
            var site = await _personRepository.GetSiteByIdAsync(personId);
            return site;

        }
        //PersonSiteDTO
        public async Task<IEnumerable<PersonSiteLookupDto>> GetAllSitesAsync(string plantNo)
        {
            var sites = await _personRepository.GetAllSitesAsync(plantNo);
            return _mapper.Map<IEnumerable<PersonSiteLookupDto>>(sites);
        }
        public async Task<IEnumerable<PersonDetailsDto>> GetAllPersonByNameAsync(int Person)
        {
            try
            {
                var bookings = await _personRepository.GetAllPersonByNameAsync(Person);
                var result = _mapper.Map<IEnumerable<PersonDetailsDto>>(bookings);
                foreach (var person in result)
                {
                    Console.WriteLine($"DTO:{person.Person}");
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> AddPersonAsync(AddPersonDto personDto,string userName)
        {
            var person = _mapper.Map<AddPerson>(personDto);
            var createdPerson = await _personRepository.AddPersonAsync(person,userName);
            //if (createdSite == "EXISTS")
            //{
            //    return "Person already exists. Please choose another Site / Plant No.";
            //}

            return $"{personDto.Name} saved as a person";

        }
        public async Task<string?> GetPersonNameByIdAsync(int personId)
        {
            //var dto = new EditPersonDto();
            //var editPerson = _mapper.Map<EditPerson>(dto);
            var person = await _personRepository.GetPersonNameByIdAsync(personId);
            return person;

        }
        public async Task<string> UpdatePersonAsync(EditPersonDto dto)
        {

            var editPerson = _mapper.Map<EditPerson>(dto);



            string traineeName = dto.Person;
            //string trainerName = trainer?.Person ?? dto.TrainerId.ToString();
            var result = await _personRepository.UpdatePersonAsync(editPerson);
            if (result.StartsWith("FAIL"))
            {
                return $"Save failed.";
            }

            return $"{traineeName} updated with new values";

        }
        public async Task<string> DeletePersonAsync(int personId)
        {
            // Fetch person name first
            var personName = await _personRepository.GetPersonNameByIdAsync(personId);
            // Fetch site information
            var site = await _personRepository.GetSiteByIdAsync(personId);

            var deleted = await _personRepository.DeletePersonAsync(personId);

            if (deleted)
            {
                return $"{personName} from {site} has been deleted from the database.";
            }
            else
            {
                return $"{personName} has training records. Delete them first if you wish to remove the person.";
            }
        }

    }
}
