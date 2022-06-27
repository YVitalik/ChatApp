using AutoFixture;
using ChatApp.BLL.DTOs.AdministrationDTOs;
using ChatApp.BLL.Services;
using ChatApp.DAL.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BLL.Tests.Moq
{
    public class UserServiceTests
    {
        private List<User> _users = new List<User>
        {
            new User { Id = "1" },
            new User { Id = "2" }
        };

        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        private Mock<UserManager<User>> _userManager;
        protected readonly Fixture _fixture = new Fixture();
        private readonly UserService _sut;

        public UserServiceTests()
        {
            _userManager = MockUserManager<User>(_users);
            _sut = new UserService(_userManager.Object);

            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                                                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Fact]
        public async Task Register_PassRegisterDtoToMethod_ShouldSuccesfullyCreateNewUser()
        {
            //Arrange
            var registerUserDto = new RegisterDTO
            {
                Username = "testUser",
                Email = "Gnnnn",
                Password = "hello"
            };

            const int START_AMOUNT_OF_USERS_IN_TEST_LIST = 2;
            
            //Act
            await _sut.Register(registerUserDto);

            //Assert
            _users.Count.Should().Be(START_AMOUNT_OF_USERS_IN_TEST_LIST + 1);
        }

        [Fact]
        public async Task Login_PassLoginDtoToMethod_ShouldSuccesfullyLoginUser()
        {
            //Arrange
            var loginUserDto = new LoginDTO
            {
                Username = "usersa",
                Password = "work"
            };

            _userManager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(_fixture.Create<User>());
            _userManager.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

            //Act
            var actual = await _sut.Login(loginUserDto);

            //Assert
            actual.Should().NotBeNull();
        }
    }
}

