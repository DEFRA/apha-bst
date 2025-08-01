using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Apha.BST.Application.Interfaces
{
    public interface IStaticDropdownService
    {
        List<SelectListItem> GetTrainingTypes();
        List<SelectListItem> GetTrainingAnimal();
    }
}
