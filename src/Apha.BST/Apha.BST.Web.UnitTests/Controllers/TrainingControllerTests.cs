using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Web.Controllers;
using Apha.BST.Web.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Apha.BST.Core.Interfaces;
using Apha.BST.Core.Entities;
using Microsoft.Extensions.Logging;

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class TrainingControllerTests
    {
        private readonly ITrainingService _mockTrainingService;
        private readonly IPersonsService _mockPersonService;
        private readonly IMapper _mockMapper;
        private readonly TrainingController _controller;       

        public TrainingControllerTests()
        {
            _mockTrainingService = Substitute.For<ITrainingService>();
            _mockPersonService = Substitute.For<IPersonsService>();
            _mockMapper = Substitute.For<IMapper>();                     
            // Setup TempData for the controller
            _controller.TempData = Substitute.For<ITempDataDictionary>();
        }

    }

}
