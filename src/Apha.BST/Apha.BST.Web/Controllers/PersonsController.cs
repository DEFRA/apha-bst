using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Apha.BST.Web.Controllers
{
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personService;
        private readonly IMapper _mapper;

        public PersonsController(IPersonsService personService, IMapper mapper)
        {
            _personService = personService;
            _mapper = mapper;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var person = await _personService.GetAllPersonAsync();
            _mapper.Map<IEnumerable<PersonsDto>>(person);
            return View(person);

        }
    }
}
