using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;

namespace Apha.BST.Application.DTOs
{
    public class NewsDto
    {
        public string? Title { get; set; }

        public string? NewsContent { get; set; }

        public DateTime DatePublished { get; set; }

        public string? Author { get; set; }

        
     }
}
