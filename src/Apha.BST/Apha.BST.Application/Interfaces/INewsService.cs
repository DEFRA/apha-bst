using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;

namespace Apha.BST.Application.Interfaces
{
    public interface INewsService
    {
        Task<IEnumerable<NewsDto>> GetLatestNewsAsync();
    }
}
