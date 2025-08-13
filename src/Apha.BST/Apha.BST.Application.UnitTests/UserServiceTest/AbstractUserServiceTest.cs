using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using AutoMapper;
using NSubstitute;

namespace Apha.BST.Application.UnitTests.UserServiceTest
{
    public abstract class AbstractUserServiceTest
    {
        protected IUserService? _userService;
        protected IUserRepository? _userRepository;
        protected IMapper? _mapper;

        public void MockGetUsersAsync(string userId, List<UserView> userViews, List<UserViewDto> userViewDtos)
        {
            var mockRepo = Substitute.For<IUserRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockRepo.GetUsersAsync(userId).Returns(userViews);
            mockMapper.Map<List<UserViewDto>>(userViews).Returns(userViewDtos);

            _userService = new UserService(mockRepo, mockMapper);
            _userRepository = mockRepo;
            _mapper = mockMapper;
        }

        public void MockGetUserByIdAsync(string userId, List<UserView> userViews, UserDto? userDto)
        {
            var mockRepo = Substitute.For<IUserRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockRepo.GetUsersAsync(userId).Returns(userViews);

            if (userViews.Any())
            {
                mockMapper.Map<UserDto>(userViews[0]).Returns(userDto);
            }

            _userService = new UserService(mockRepo, mockMapper);
            _userRepository = mockRepo;
            _mapper = mockMapper;
        }

        public void MockAddUserAsync(UserDto userDto, User user, List<VlaLocView> vlaLocViews, List<VlaLocDto> vlaLocDtos, string paramName)
        {
            var mockRepo = Substitute.For<IUserRepository>();
            var mockMapper = Substitute.For<IMapper>();

            if (userDto != null && (string.IsNullOrEmpty(userDto.UserId) || string.IsNullOrEmpty(userDto.UserName)))
            {
                mockMapper.When(x => x.Map<User>(Arg.Is<UserDto>(dto =>
                    string.IsNullOrEmpty(dto.UserId) || string.IsNullOrEmpty(dto.UserName))))
                    .Do(x => { throw new ArgumentException("Invalid user data", paramName); });
            }
            else if (userDto != null)
            {
                mockMapper.Map<User>(userDto).Returns(user);
            }

            mockRepo.AddUserAsync(user).Returns(Task.CompletedTask);
            mockRepo.GetLocationsAsync().Returns(vlaLocViews);
            mockMapper.Map<List<VlaLocDto>>(vlaLocViews).Returns(vlaLocDtos);

            _userService = new UserService(mockRepo, mockMapper);
            _userRepository = mockRepo;
            _mapper = mockMapper;
        }

        public void MockUpdateUserAsync(UserDto userDto, User user, List<UserView> userViews, List<UserViewDto> userViewDtos)
        {
            var mockRepo = Substitute.For<IUserRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockMapper.Map<User>(userDto).Returns(user);
            mockRepo.UpdateUserAsync(user).Returns(Task.CompletedTask);
            mockRepo.GetUsersAsync(userDto.UserId).Returns(userViews);
            mockMapper.Map<List<UserViewDto>>(userViews).Returns(userViewDtos);

            _userService = new UserService(mockRepo, mockMapper);
            _userRepository = mockRepo;
            _mapper = mockMapper;
        }

        public void MockDeleteUserAsync(string userId, List<UserView> userViews)
        {
            var mockRepo = Substitute.For<IUserRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockRepo.GetUsersAsync(userId).Returns(userViews);
            mockRepo.DeleteUserAsync(userId).Returns(Task.CompletedTask);

            _userService = new UserService(mockRepo, mockMapper);
            _userRepository = mockRepo;
            _mapper = mockMapper;
        }
    }
}
