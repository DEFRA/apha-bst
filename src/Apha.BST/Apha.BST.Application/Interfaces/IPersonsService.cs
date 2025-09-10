using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;

namespace Apha.BST.Application.Interfaces
{
    public interface IPersonsService
    {
        Task<IEnumerable<PersonDetailDto>> GetAllPersonByNameAsync(int personId);
        Task<IEnumerable<PersonLookupDto>> GetPersonsForDropdownAsync();
        Task<string> DeletePersonAsync(int personId);
        //for binding the dropdown in add persons page
        Task<IEnumerable<PersonSiteLookupDto>> GetAllSitesAsync(string plantNo);
        Task<string> AddPersonAsync(AddPersonDto personsDto, string userName);
        Task<string> UpdatePersonAsync(EditPersonDto dto);
        Task<string?> GetPersonNameByIdAsync(int personId);
        Task<string?> GetSiteByIdAsync(int personId);
        Task<string?> GetSiteNameById(int personId);
    }
}
