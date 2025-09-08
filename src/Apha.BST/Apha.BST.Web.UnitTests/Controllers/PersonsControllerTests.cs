using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Web.Controllers;
using Apha.BST.Web.Models;
using Apha.BST.Web.PresentationService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class PersonsControllerTests
    {
        private readonly IPersonsService _personService;
        private readonly IMapper _mapper;
        private readonly IUserDataService _userDataService;
        private readonly ILogService _logService;
        private readonly PersonsController _controller;
        private static SqlException CreateSqlException()
        {
            // This creates an uninitialized SqlException instance for testing purposes.
            return (SqlException)RuntimeHelpers.GetUninitializedObject(typeof(SqlException));
        }
        public PersonsControllerTests()
        {
            _personService = Substitute.For<IPersonsService>();
            _mapper = Substitute.For<IMapper>();
            _userDataService = Substitute.For<IUserDataService>();
            _logService = Substitute.For<ILogService>();
            _controller = new PersonsController(_personService, _mapper, _userDataService, _logService);
            // Setup TempData for the controller
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Substitute.For<ITempDataProvider>());
            _controller.TempData = tempData;

            // Setup ControllerContext with ActionDescriptor
            var httpContext = new DefaultHttpContext();
            var actionContext = new ActionContext(
                httpContext,
                new Microsoft.AspNetCore.Routing.RouteData(),
                new ControllerActionDescriptor());
            _controller.ControllerContext = new ControllerContext(actionContext);
            _controller.ControllerContext = new ControllerContext
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    ActionName = "DeletePerson"
                }
            };
            // Setup default permissions
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userDataService.GetUsername().Returns("testUser");
        }

        [Fact]
        public async Task ViewPerson_InvalidModelState_RedirectsToViewPerson()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "Sample error");

            // Act
            var result = await _controller.ViewPerson();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewPerson", redirectResult.ActionName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public async Task ViewPerson_DifferentSelectedPersonValues_CallsGetAllPersonByNameAsyncWithCorrectParameter(int selectedPerson)
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(Task.FromResult(true));
            _personService.GetPersonsForDropdownAsync().Returns(new List<PersonLookupDto>());
            _personService.GetAllPersonByNameAsync(selectedPerson).Returns(new List<PersonDetailDto>());
            _mapper.Map<IEnumerable<PersonViewModel>>(Arg.Any<IEnumerable<PersonDetailDto>>()).Returns(new List<PersonViewModel>());

            // Act
            await _controller.ViewPerson(selectedPerson);

            // Assert
            await _personService.Received(1).GetAllPersonByNameAsync(selectedPerson);
        }

        [Fact]
        public async Task ViewPerson_CallsRequiredMethods()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(Task.FromResult(true));
            _personService.GetPersonsForDropdownAsync().Returns(new List<PersonLookupDto>());
            _personService.GetAllPersonByNameAsync(Arg.Any<int>()).Returns(new List<PersonDetailDto>());
            _mapper.Map<IEnumerable<PersonViewModel>>(Arg.Any<IEnumerable<PersonDetailDto>>()).Returns(new List<PersonViewModel>());

            // Act
            await _controller.ViewPerson();

            // Assert
            await _userDataService.Received(1).CanEditPage(Arg.Any<string>());
            await _personService.Received(1).GetPersonsForDropdownAsync();
            await _personService.Received(1).GetAllPersonByNameAsync(Arg.Any<int>());
            _mapper.Received(1).Map<IEnumerable<PersonViewModel>>(Arg.Any<IEnumerable<PersonDetailDto>>());
        }

        [Fact]
        public async Task ViewPerson_MapperUsedCorrectly()
        {
            // Arrange
            var allPersonDetailDto = new List<PersonDetailDto> { new PersonDetailDto { PersonID = 1, Name = "John Doe" } };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(Task.FromResult(true));
            _personService.GetPersonsForDropdownAsync().Returns(new List<PersonLookupDto>());
            _personService.GetAllPersonByNameAsync(Arg.Any<int>()).Returns(allPersonDetailDto);

            // Act
            await _controller.ViewPerson();

            // Assert
            _mapper.Received(1).Map<IEnumerable<PersonViewModel>>(Arg.Is<IEnumerable<PersonDetailDto>>(dto => dto == allPersonDetailDto));
        }

        [Fact]
        public async Task ViewPerson_ReturnsCorrectViewModel()
        {
            // Arrange
            int selectedPerson = 1;
            bool canEdit = true;
            var dropdownDto = new List<PersonLookupDto> { new PersonLookupDto { PersonID = 1, Person = "John Doe" } };
            var allPersonDetailDto = new List<PersonDetailDto> { new PersonDetailDto { PersonID = 1, Name = "John Doe" } };
            var personViewModel = new List<PersonViewModel> { new PersonViewModel { PersonId = 1, Name = "John Doe" } };

            _userDataService.CanEditPage(Arg.Any<string>()).Returns(Task.FromResult(canEdit));
            _personService.GetPersonsForDropdownAsync().Returns(dropdownDto);
            _personService.GetAllPersonByNameAsync(selectedPerson).Returns(allPersonDetailDto);
            _mapper.Map<IEnumerable<PersonViewModel>>(Arg.Any<IEnumerable<PersonDetailDto>>()).Returns(personViewModel);

            // Act
            var result = await _controller.ViewPerson(selectedPerson);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PersonListViewModel>(viewResult.Model);
            Assert.Equal(selectedPerson, model.SelectedPerson);
            Assert.Equal(canEdit, model.CanEdit);
            Assert.IsType<List<SelectListItem>>(model.AllPerson);
            Assert.IsType<List<PersonViewModel>>(model.FilteredPerson);
        }
        [Fact]
        public async Task DeletePerson_InvalidModelState_RedirectsToViewPerson()
        {
            // Arrange
            int personId = 1;
            string personName = "John Doe";
            int selectedPerson = 1;

            _controller.ControllerContext.ActionDescriptor.ActionName = "DeletePerson";
            _controller.ModelState.AddModelError("PersonId", "PersonId is required");
            _userDataService.CanEditPage("DeletePerson").Returns(true);

            // Act
            var result = await _controller.DeletePerson(personId, personName, selectedPerson);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewPerson", redirectResult.ActionName);

            // Verify that delete service was not called due to invalid model state
            await _personService.DidNotReceive().DeletePersonAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task DeletePerson_CannotEdit_ReturnsViewWithCanEditFalse()
        {
            // Arrange
            int personId = 1;
            string personName = "John Doe";
            int selectedPerson = 1;

            _controller.ControllerContext.ActionDescriptor.ActionName = "DeletePerson";
            _userDataService.CanEditPage("DeletePerson").Returns(false);

            var mockPersonDto = new List<PersonDetailDto>
    {
        new PersonDetailDto { PersonID = 1, Person = "John Doe" },
        new PersonDetailDto { PersonID = 2, Person = "Jane Smith" }
    };

            _personService.GetAllPersonByNameAsync(selectedPerson).Returns(mockPersonDto);
            _mapper.Map<IEnumerable<PersonViewModel>>(mockPersonDto).Returns(new List<PersonViewModel>
    {
        new PersonViewModel { PersonId = 1, Person = "John Doe" },
        new PersonViewModel { PersonId = 2, Person = "Jane Smith" }
    });

            // Act
            var result = await _controller.DeletePerson(personId, personName, selectedPerson);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ViewPerson", viewResult.ViewName);

            var model = Assert.IsType<PersonListViewModel>(viewResult.Model);
            Assert.False(model.CanEdit);
            Assert.Equal(selectedPerson, model.SelectedPerson);
            Assert.Empty(model.AllPerson); // No dropdown items when canEdit is false

            // Verify that delete service was not called
            await _personService.DidNotReceive().DeletePersonAsync(Arg.Any<int>());
        }

        [Fact]
        public async Task DeletePerson_SuccessfulDelete_PersonExistsInDropdown_ReturnsViewWithUpdatedModel()
        {
            // Arrange
            int personId = 1;
            string personName = "John Doe";
            int selectedPerson = 1;

            _controller.ControllerContext.ActionDescriptor.ActionName = "DeletePerson";
            _userDataService.CanEditPage("DeletePerson").Returns(true);
            _personService.DeletePersonAsync(personId).Returns("Person deleted successfully");

            var dropdownDto = new List<PersonLookupDto>
    {
        new PersonLookupDto { PersonID = 1, Person = "John Doe" },
        new PersonLookupDto { PersonID = 2, Person = "Jane Smith" }
    };

            var allPersonDto = new List<PersonDetailDto>
    {
        new PersonDetailDto { PersonID = 1, Person = "John Doe" },
        new PersonDetailDto { PersonID = 2, Person = "Jane Smith" }
    };

            _personService.GetPersonsForDropdownAsync().Returns(dropdownDto);
            _personService.GetAllPersonByNameAsync(selectedPerson).Returns(allPersonDto);
            _mapper.Map<IEnumerable<PersonViewModel>>(allPersonDto).Returns(new List<PersonViewModel>
    {
        new PersonViewModel { PersonId = 1, Person = "John Doe" },
        new PersonViewModel { PersonId = 2, Person = "Jane Smith" }
    });

            // Act
            var result = await _controller.DeletePerson(personId, personName, selectedPerson);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("ViewPerson", viewResult.ViewName);

            var model = Assert.IsType<PersonListViewModel>(viewResult.Model);
            Assert.True(model.CanEdit);
            Assert.Equal(selectedPerson, model.SelectedPerson);

            var allPersonList = model.AllPerson.ToList();
            Assert.Equal(2, allPersonList.Count);
            Assert.Equal("1", allPersonList[0].Value);
            Assert.Equal("John Doe", allPersonList[0].Text);

            // Check TempData directly instead of using NSubstitute Received
            Assert.Equal("Person deleted successfully", _controller.TempData["PersonMessage"]);
            await _personService.Received(1).DeletePersonAsync(personId);
        }

        [Fact]
        public async Task DeletePerson_SuccessfulDelete_PersonNotInDropdown_AddsPersonToDropdown()
        {
            // Arrange
            int personId = 3;
            string personName = "Bob Wilson";
            int selectedPerson = 1;

            _controller.ControllerContext.ActionDescriptor.ActionName = "DeletePerson";
            _userDataService.CanEditPage("DeletePerson").Returns(true);
            _personService.DeletePersonAsync(personId).Returns("Person deleted successfully");

            var dropdownDto = new List<PersonLookupDto>
    {
        new PersonLookupDto { PersonID = 1, Person = "John Doe" },
        new PersonLookupDto { PersonID = 2, Person = "Jane Smith" }
    };

            var allPersonDto = new List<PersonDetailDto>
    {
        new PersonDetailDto { PersonID = 1, Person = "John Doe" },
        new PersonDetailDto { PersonID = 2, Person = "Jane Smith" }
    };

            _personService.GetPersonsForDropdownAsync().Returns(dropdownDto);
            _personService.GetAllPersonByNameAsync(selectedPerson).Returns(allPersonDto);
            _mapper.Map<IEnumerable<PersonViewModel>>(allPersonDto).Returns(new List<PersonViewModel>
    {
        new PersonViewModel { PersonId = 1, Person = "John Doe" },
        new PersonViewModel { PersonId = 2, Person = "Jane Smith" }
    });

            // Act
            var result = await _controller.DeletePerson(personId, personName, selectedPerson);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PersonListViewModel>(viewResult.Model);

            var allPersonList = model.AllPerson.ToList();
            Assert.Equal(3, allPersonList.Count); // Original 2 + 1 added
            Assert.Contains(allPersonList, item => item.Value == "3" && item.Text == "Bob Wilson");

            await _personService.Received(1).DeletePersonAsync(personId);
        }

        [Fact]
        public async Task DeletePerson_SelectedPersonIsZero_RedirectsToViewPersonWithSelectedPerson()
        {
            // Arrange
            int personId = 1;
            string personName = "John Doe";
            int selectedPerson = 0;

            _controller.ControllerContext.ActionDescriptor.ActionName = "DeletePerson";
            _userDataService.CanEditPage("DeletePerson").Returns(true);
            _personService.DeletePersonAsync(personId).Returns("Person deleted successfully");
            _personService.GetPersonsForDropdownAsync().Returns(new List<PersonLookupDto>());

            // Act
            var result = await _controller.DeletePerson(personId, personName, selectedPerson);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewPerson", redirectResult.ActionName);
            Assert.Equal(0, redirectResult.RouteValues?["selectedPerson"]);
        }

        [Fact]
        public async Task DeletePerson_SqlExceptionThrown_LogsExceptionAndSetsErrorMessage()
        {
            // Arrange
            int personId = 1;
            string personName = "John Doe";
            int selectedPerson = 1;
            var sqlException = CreateSqlException();

            _controller.ControllerContext.ActionDescriptor.ActionName = "DeletePerson";
            _userDataService.CanEditPage("DeletePerson").Returns(true);
            _personService.DeletePersonAsync(personId).Throws(sqlException);

            var allPersonDto = new List<PersonDetailDto>();
            _personService.GetAllPersonByNameAsync(selectedPerson).Returns(allPersonDto);
            _mapper.Map<IEnumerable<PersonViewModel>>(allPersonDto).Returns(new List<PersonViewModel>());

            // Act
            var result = await _controller.DeletePerson(personId, personName, selectedPerson);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PersonListViewModel>(viewResult.Model);

            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), "DeletePerson");
            Assert.Contains("Delete failed:", _controller.TempData["PersonMessage"]?.ToString());
        }

        [Fact]
        public async Task DeletePerson_GeneralExceptionThrown_LogsExceptionAndSetsErrorMessage()
        {
            // Arrange
            int personId = 1;
            string personName = "John Doe";
            int selectedPerson = 1;

            _controller.ControllerContext.ActionDescriptor.ActionName = "DeletePerson";
            _userDataService.CanEditPage("DeletePerson").Returns(true);
            _personService.DeletePersonAsync(personId).Throws(new Exception("General error"));

            var allPersonDto = new List<PersonDetailDto>();
            _personService.GetAllPersonByNameAsync(selectedPerson).Returns(allPersonDto);
            _mapper.Map<IEnumerable<PersonViewModel>>(allPersonDto).Returns(new List<PersonViewModel>());

            // Act
            var result = await _controller.DeletePerson(personId, personName, selectedPerson);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PersonListViewModel>(viewResult.Model);

            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), "DeletePerson");
            Assert.Contains("Delete failed: General error", _controller.TempData["PersonMessage"]?.ToString());
        }

      
        [Fact]
        public async Task AddPerson_ValidModelAndUserHasEditPermissions_ReturnsRedirectToActionResult()
        {
            // Arrange
            var viewModel = new AddPersonViewModel { Name = "John Doe", PlantNo = "Plant001" };
            var dto = new AddPersonDto { Name = "John Doe", PlantNo = "Plant001" };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userDataService.GetUsername().Returns("testuser");
            _mapper.Map<AddPersonDto>(viewModel).Returns(dto);
            _personService.AddPersonAsync(Arg.Any<AddPersonDto>(), Arg.Any<string>()).Returns("Person added successfully");

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddPerson", redirectResult.ActionName);
            Assert.Equal("Person added successfully", _controller.TempData["PersonMessage"]);
            await _personService.Received(1).AddPersonAsync(Arg.Any<AddPersonDto>(), Arg.Any<string>());
        }

        [Fact]
        public async Task AddPerson_InvalidModelState_ReturnsViewResult()
        {
            // Arrange
            var viewModel = new AddPersonViewModel();
            _controller.ModelState.AddModelError("Person", "Person is required");
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(viewModel, viewResult.Model);
        }

        [Fact]
        public async Task AddPerson_SqlException_LogsExceptionAndReturnsRedirectToActionResult()
        {
            // Arrange

            var viewModel = new AddPersonViewModel {Name = "John Doe", PlantNo = "Plant001" };
            var dto = new AddPersonDto {Name = "John Doe", PlantNo = "Plant001" };
            var sqlException = CreateSqlException();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userDataService.GetUsername().Returns("testuser");
            _mapper.Map<AddPersonDto>(viewModel).Returns(dto);
            _personService.AddPersonAsync(Arg.Any<AddPersonDto>(), Arg.Any<string>()).Throws(sqlException);

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddPerson", redirectResult.ActionName);
            Assert.Equal("Save failed: Exception of type 'Microsoft.Data.SqlClient.SqlException' was thrown.", _controller.TempData["PersonMessage"]);
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task AddPerson_GeneralException_LogsExceptionAndReturnsRedirectToActionResult()
        {
            // Arrange

            var viewModel = new AddPersonViewModel { Name = "John Doe", PlantNo = "Plant001" };
            var dto = new AddPersonDto { Name = "John Doe", PlantNo = "Plant001" };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userDataService.GetUsername().Returns("testuser");
            _mapper.Map<AddPersonDto>(viewModel).Returns(dto);
            _personService.AddPersonAsync(Arg.Any<AddPersonDto>(), Arg.Any<string>()).Throws(new Exception());

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddPerson", redirectResult.ActionName);
            Assert.Equal("Save failed: Exception of type 'System.Exception' was thrown.", _controller.TempData["PersonMessage"]);
            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task AddPerson_UserDoesNotHaveEditPermissions_ReturnsRedirectToActionResult()
        {
            // Arrange
            var viewModel = new AddPersonViewModel { Name = "John Doe", PlantNo = "Plant001" };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddPerson", redirectResult.ActionName);
            Assert.Null(_controller.TempData["PersonMessage"]);
            await _personService.DidNotReceive().AddPersonAsync(Arg.Any<AddPersonDto>(), Arg.Any<string>());
        }
        [Fact]
        public async Task AddPerson_ValidInput_CanEditTrue_ReturnsRedirectToActionResult()
        {
            // Arrange
            var viewModel = new AddPersonViewModel { Name="Abc", PlantNo="Site1" };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userDataService.GetUsername().Returns("testUser");
            _mapper.Map<AddPersonDto>(Arg.Any<AddPersonViewModel>()).Returns(new AddPersonDto());
            _personService.AddPersonAsync(Arg.Any<AddPersonDto>(), Arg.Any<string>()).Returns("Person added successfully");

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddPerson", redirectResult.ActionName);
            Assert.Equal("Person added successfully", _controller.TempData["PersonMessage"]);
        }

        [Fact]
        public async Task AddPerson_ValidInput_CanEditFalse_ReturnsRedirectToActionResult()
        {
            // Arrange
            var viewModel = new AddPersonViewModel { Name="Abc", PlantNo="Site1" };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddPerson", redirectResult.ActionName);
            Assert.Null(_controller.TempData["PersonMessage"]);
        }



        [Fact]
        public async Task AddPerson_SqlException_LogsAndReturnsRedirectToActionResult()
        {
            // Arrange

            var viewModel = new AddPersonViewModel {Name="Abc",PlantNo= "Site1"  };
            var sqlException = CreateSqlException();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userDataService.GetUsername().Returns("testUser");
            _mapper.Map<AddPersonDto>(Arg.Any<AddPersonViewModel>()).Returns(new AddPersonDto());
            _personService.AddPersonAsync(Arg.Any<AddPersonDto>(), Arg.Any<string>()).Throws(sqlException);

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddPerson", redirectResult.ActionName);
            Assert.Equal("Save failed: Exception of type 'Microsoft.Data.SqlClient.SqlException' was thrown.", _controller.TempData["PersonMessage"]);
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task AddPerson_GeneralException_LogsAndReturnsRedirectToActionResult()
        {
            // Arrange



            var viewModel = new AddPersonViewModel {Name = "John Doe", PlantNo = "Plant001" };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userDataService.GetUsername().Returns("testUser");
            _mapper.Map<AddPersonDto>(Arg.Any<AddPersonViewModel>()).Returns(new AddPersonDto());
            _personService.AddPersonAsync(Arg.Any<AddPersonDto>(), Arg.Any<string>()).Throws(new Exception());

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddPerson", redirectResult.ActionName);
            Assert.Equal("Save failed: Exception of type 'System.Exception' was thrown.", _controller.TempData["PersonMessage"]);
            _logService.Received(1).LogGeneralException(Arg.Any<Exception>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }
        [Fact]
        public async Task EditPerson_ValidModelState_ReturnsViewWithCorrectModel()
        {
            // Arrange
            int id = 1;
            bool canEdit = true;
            string personName = "John Doe";
            string siteName = "Site A";
            var sites = new List<PersonSiteLookupDto> { new PersonSiteLookupDto { PlantNo = "P001", Name = "Site A" } };

            _userDataService.CanEditPage(Arg.Any<string>()).Returns(Task.FromResult(canEdit));
            _personService.GetPersonNameByIdAsync(id).Returns(Task.FromResult<string?>(personName));

            _personService.GetSiteByIdAsync(id).Returns(Task.FromResult<string?>(siteName));
            _personService.GetAllSitesAsync("All").Returns(sites);



            // Act
            var result = await _controller.EditPerson(id) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<EditPersonViewModel>(result.Model);
            Assert.Equal(id, model.PersonId);
            Assert.Equal(personName, model.Person);
            Assert.Equal(siteName, model.Name);
            Assert.Equal(canEdit, model.CanEdit);
            Assert.Single(model.SiteOptions);
        }

        [Fact]
        public async Task EditPerson_InvalidModelState_RedirectsToViewPerson()
        {
            // Arrange
            int id = 1;
            _controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await _controller.EditPerson(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ViewPerson", result.ActionName);
        }

        [Fact]
        public async Task EditPerson_PersonNotFound_RedirectsToViewPerson()
        {
            // Arrange
            int id = 1;
            _personService.GetPersonNameByIdAsync(id).Returns(Task.FromResult<string?>(null));

            // Act
            var result = await _controller.EditPerson(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ViewPerson", result.ActionName);
        }



        [Fact]
        public async Task EditPerson_HandlesCanEditFlagCorrectly()
        {
            // Arrange
            int id = 1;
            bool canEdit = false;
            string personName = "John Doe";
            string siteName = "Site A";
            var sites = new List<PersonSiteLookupDto> { new PersonSiteLookupDto { PlantNo = "P001", Name = "Site A" } };

            _userDataService.CanEditPage(Arg.Any<string>()).Returns(Task.FromResult(canEdit));
            _personService.GetPersonNameByIdAsync(id).Returns(Task.FromResult<string?>(personName));

            _personService.GetSiteByIdAsync(id).Returns(Task.FromResult<string?>(siteName));
            _personService.GetAllSitesAsync("All").Returns(sites);


            // Act
            var result = await _controller.EditPerson(id) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<EditPersonViewModel>(result.Model);
            Assert.False(model.CanEdit);
        }
        [Fact]
        public async Task EditPerson_InvalidModelState_ReturnsViewWithModel()
        {
            // Arrange
            var viewModel = new EditPersonViewModel{ PersonId = 1 };
            _controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await _controller.EditPerson(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Same(viewModel, viewResult.Model);
        }

        [Fact]
        public async Task EditPerson_CanEditTrue_UpdateSuccessful_RedirectsToEditPerson()
        {
            // Arrange
            var viewModel = new EditPersonViewModel{ PersonId = 1 };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<EditPersonDto>(viewModel).Returns(new EditPersonDto());
            _personService.UpdatePersonAsync(Arg.Any<EditPersonDto>()).Returns("Update successful");

            // Act
            var result = await _controller.EditPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("EditPerson", redirectResult.ActionName);
            Assert.Equal("Update successful", _controller.TempData["PersonMessage"]);
        }

        [Fact]
        public async Task EditPerson_CanEditFalse_RedirectsToEditPerson()
        {
            // Arrange

            var viewModel = new EditPersonViewModel { PersonId = 1 };

            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.EditPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("EditPerson", redirectResult.ActionName);
            Assert.Null(_controller.TempData["PersonMessage"]);
        }

        [Fact]
        public async Task EditPerson_SqlException_LogsAndRedirectsToEditPerson()
        {

            var viewModel = new EditPersonViewModel { PersonId = 1 };
            var sqlException = CreateSqlException();
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<EditPersonDto>(viewModel).Returns(new EditPersonDto());
            _personService.UpdatePersonAsync(Arg.Any<EditPersonDto>()).Throws(sqlException);

            // Act
            var result = await _controller.EditPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("EditPerson", redirectResult.ActionName);
            Assert.Equal("Update failed: Exception of type 'Microsoft.Data.SqlClient.SqlException' was thrown.", _controller.TempData["PersonMessage"]);
            _logService.Received(1).LogSqlException(Arg.Any<SqlException>(), _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task EditPerson_GeneralException_LogsAndRedirectsToEditPerson()
        {
            // Arrange
            var viewModel = new EditPersonViewModel { PersonId = 1, Person = "Test", Name = "Test Site" };
            var exception = new Exception("Test error");
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _mapper.Map<EditPersonDto>(viewModel).Returns(new EditPersonDto());
            _personService.UpdatePersonAsync(Arg.Any<EditPersonDto>()).Throws(exception);

            // Act
            var result = await _controller.EditPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("EditPerson", redirectResult.ActionName);
            Assert.Equal("Update failed: Test error", _controller.TempData["PersonMessage"]);
            _logService.Received(1).LogGeneralException(exception, _controller.ControllerContext.ActionDescriptor.ActionName);
        }

        [Fact]
        public async Task AddPerson_Get_ReturnsViewWithModel()
        {
            // Arrange
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _personService.GetAllSitesAsync(Arg.Any<string>()).Returns(new List<PersonSiteLookupDto>());

            // Act
            var result = await _controller.AddPerson();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddPersonViewModel>(viewResult.Model);
            Assert.True(model.CanEdit);
            Assert.NotNull(model.Sites);
        }

        [Fact]
        public async Task AddPerson_Post_ValidModel_UserCanEdit_RedirectsToAddPerson()
        {
            // Arrange
            var viewModel = new AddPersonViewModel { Name = "John Doe", PlantNo = "P1" };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userDataService.GetUsername().Returns("testuser");
            _mapper.Map<AddPersonDto>(Arg.Any<AddPersonViewModel>()).Returns(new AddPersonDto());
            _personService.AddPersonAsync(Arg.Any<AddPersonDto>(), Arg.Any<string>()).Returns("Person added successfully");

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddPerson", redirectResult.ActionName);
           _controller.TempData["PersonMessage"] = "Person added successfully";
        }

        [Fact]
        public async Task AddPerson_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var viewModel = new AddPersonViewModel();
            _controller.ModelState.AddModelError("Person", "Required");
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _personService.GetAllSitesAsync(Arg.Any<string>()).Returns(new List<PersonSiteLookupDto>());

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<AddPersonViewModel>(viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(model.CanEdit);
            Assert.NotNull(model.Sites);
        }
        [Fact]
        public async Task AddPerson_InvalidModelState_GetUsernameReturnsNull_PopulatesSitesAndReturnsView()
        {
            // Arrange
            _controller.ControllerContext.ActionDescriptor.ActionName = "AddPerson";
            _controller.ModelState.AddModelError("Name", "Name is required");

            var viewModel = new AddPersonViewModel { Name = "" };

            bool canEdit = true;
            _userDataService.CanEditPage("AddPerson").Returns(canEdit);
            _userDataService.GetUsername().Returns("");

            var mockSites = new List<PersonSiteLookupDto>
    {
        new PersonSiteLookupDto { PlantNo = "001", Name = "Site 1" },
        new PersonSiteLookupDto { PlantNo = "002", Name = "Site 2" }
    };

            _personService.GetAllSitesAsync(Arg.Any<string>()).Returns(mockSites);

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedViewModel = Assert.IsType<AddPersonViewModel>(viewResult.Model);

          
            Assert.NotNull(returnedViewModel.Sites);
            var sitesList = returnedViewModel.Sites.ToList();
            Assert.Equal(2, sitesList.Count);
            Assert.Equal("001", sitesList[0].Value);
            Assert.Equal("Site 1", sitesList[0].Text);
            Assert.Equal("002", sitesList[1].Value);
            Assert.Equal("Site 2", sitesList[1].Text);

            Assert.True(returnedViewModel.CanEdit);

            // Verify GetUsername was called (line 5)
            _userDataService.Received(1).GetUsername();

            // Verify AddPersonAsync was not called due to invalid ModelState
            await _personService.DidNotReceive().AddPersonAsync(Arg.Any<AddPersonDto>(), Arg.Any<string>());
        }

        [Fact]
        public async Task AddPerson_InvalidModelState_GetUsernameReturnsValue_PopulatesSitesAndReturnsView()
        {
            // Arrange
            _controller.ControllerContext.ActionDescriptor.ActionName = "AddPerson";
            _controller.ModelState.AddModelError("Name", "Name is required");

            var viewModel = new AddPersonViewModel { Name = "" };

            bool canEdit = false;
            _userDataService.CanEditPage("AddPerson").Returns(canEdit);
            _userDataService.GetUsername().Returns("testuser@domain.com"); 

            var mockSites = new List<PersonSiteLookupDto>
    {
        new PersonSiteLookupDto { PlantNo = "003", Name = "Site 3" }
    };

            _personService.GetAllSitesAsync(Arg.Any<string>()).Returns(mockSites);

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedViewModel = Assert.IsType<AddPersonViewModel>(viewResult.Model);

            // Sites dropdown population with single item
            Assert.NotNull(returnedViewModel.Sites);
            var sitesList = returnedViewModel.Sites.ToList();
            Assert.Single(sitesList);
            Assert.Equal("003", sitesList[0].Value);
            Assert.Equal("Site 3", sitesList[0].Text);

            Assert.False(returnedViewModel.CanEdit);

            // Verify GetUsername was called (line 5)
            _userDataService.Received(1).GetUsername();
        }
        [Fact]
        public async Task EditPerson_InvalidModelState_PopulatesSiteOptions()
        {
            // Arrange
            _controller.ControllerContext.ActionDescriptor.ActionName = "EditPerson";
            _controller.ModelState.AddModelError("Name", "Name is required");

            var viewModel = new EditPersonViewModel { PersonId = 1 };

            _userDataService.CanEditPage("EditPerson").Returns(true);

            var mockSites = new List<PersonSiteLookupDto>
    {
        new PersonSiteLookupDto { PlantNo = "001", Name = "Site 1" },
        new PersonSiteLookupDto { PlantNo = "002", Name = "Site 2" }
    };

            _personService.GetAllSitesAsync(Arg.Any<string>()).Returns(mockSites);

            // Act
            var result = await _controller.EditPerson(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedViewModel = Assert.IsType<EditPersonViewModel>(viewResult.Model);

            // This covers lines 9-10: Select + ToList operations
            Assert.NotNull(returnedViewModel.SiteOptions);
            var siteOptions = returnedViewModel.SiteOptions.ToList();
            Assert.Equal(2, siteOptions.Count);
            Assert.Equal("001", siteOptions[0].Value);
            Assert.Equal("Site 1", siteOptions[0].Text);
        }

        [Fact]
        public async Task EditPerson_InvalidModelState_EmptySitesList_CreatesEmptySelectList()
        {
            // Arrange
            _controller.ControllerContext.ActionDescriptor.ActionName = "EditPerson";
            _controller.ModelState.AddModelError("Name", "Name is required");

            var viewModel = new EditPersonViewModel { PersonId = 1 };

            _userDataService.CanEditPage("EditPerson").Returns(false);
            _personService.GetAllSitesAsync(Arg.Any<string>()).Returns(new List<PersonSiteLookupDto>());

            // Act
            var result = await _controller.EditPerson(viewModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var returnedViewModel = Assert.IsType<EditPersonViewModel>(viewResult.Model);

            // This covers lines 9-10 with empty collection
            Assert.NotNull(returnedViewModel.SiteOptions);
            Assert.Empty(returnedViewModel.SiteOptions);
        }
        [Fact]
        public async Task AddPerson_Post_GeneralException_LogsAndSetsErrorMessage()
        {
            // Arrange
            var viewModel = new AddPersonViewModel {Name = "John Doe", PlantNo = "P1" };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(true);
            _userDataService.GetUsername().Returns("testuser");
            _mapper.Map<AddPersonDto>(Arg.Any<AddPersonViewModel>()).Returns(new AddPersonDto());
            _personService.AddPersonAsync(Arg.Any<AddPersonDto>(), Arg.Any<string>())
            .Throws(new Exception("Test exception"));

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            _logService.Received().LogGeneralException(Arg.Any<Exception>(), Arg.Any<string>());
            Assert.StartsWith("Save failed:", _controller.TempData["PersonMessage"] as string);
        }

        [Fact]
        public async Task AddPerson_Post_UserCannotEdit_DoesNotCallAddPersonAsync()
        {
            // Arrange
            var viewModel = new AddPersonViewModel {Name = "John Doe", PlantNo = "P1" };
            _userDataService.CanEditPage(Arg.Any<string>()).Returns(false);

            // Act
            var result = await _controller.AddPerson(viewModel);

            // Assert
            await _personService.DidNotReceive().AddPersonAsync(Arg.Any<AddPersonDto>(), Arg.Any<string>());
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AddPerson", redirectResult.ActionName);
        }
    }
    }