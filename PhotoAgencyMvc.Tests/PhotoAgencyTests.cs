using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Moq;
using PhotoAgencyMvc.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Routing;

namespace PhotoAgencyMvc.Tests
{
    public class TestPhotoAgencyContext : PhotoAgencyContext
    {
        public TestPhotoAgencyContext(DbContextOptions<PhotoAgencyContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }
    }
    public class AuthTests
    {
        private readonly PhotoAgencyContext _context;

        public AuthTests()
        {
            // Настройка in-memory базы данных
            var options = new DbContextOptionsBuilder<PhotoAgencyContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TestPhotoAgencyContext(options);
        }

        [Fact]
        public async Task RegisterModel_OnPostAsync_ValidInput_RedirectsToLogin()
        {
            // Arrange
            var registerModel = new RegisterModel(_context)
            {
                Input = new RegisterModel.InputModel
                {
                    Username = "testuser",
                    Password = "testpass",
                    Email = "test@example.com",
                    FullName = "Test User",
                    Phone = "1234567890",
                    Address = "123 Test St"
                }
            };

            // Act
            var result = await registerModel.OnPostAsync();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Login", redirectResult.PageName);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
            Assert.NotNull(user);
            Assert.Equal(3, user.RoleId); // Client role
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId == user.Id);
            Assert.NotNull(client);
            Assert.Equal("Test User", client.FullName);
        }

        [Fact]
        public async Task RegisterModel_OnPostAsync_InvalidInput_ReturnsPage()
        {
            // Arrange
            var registerModel = new RegisterModel(_context)
            {
                Input = new RegisterModel.InputModel
                {
                    Username = "", 
                    Password = "testpass",
                    Email = "test@example.com",
                    FullName = "Test User",
                    Phone = "1234567890",
                    Address = "123 Test St"
                }
            };
            registerModel.ModelState.AddModelError("Input.Username", "Username is required");

            // Act
            var result = await registerModel.OnPostAsync();

            // Assert
            var pageResult = Assert.IsType<PageResult>(result);
            Assert.False(registerModel.ModelState.IsValid);
            Assert.Empty(_context.Users); // Пользователь не добавлен
        }

        [Fact]
        
        public async Task LoginModel_OnPostAsync_ValidCredentials_RedirectsToDashboard()
        {
            // Arrange
            var user = new User { Id = 1, Username = "testuser", Password = "testpass", Email = "test@example.com", RoleId = 3, Role = new Role { Id = 3, Name = "Client" } };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var httpContext = new Mock<HttpContext>();
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(u => u.Content(It.IsAny<string>())).Returns("~/");
            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            urlHelperFactory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>())).Returns(urlHelper.Object);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(sp => sp.GetService(typeof(IUrlHelperFactory)))
                .Returns(urlHelperFactory.Object);
            var authService = new Mock<IAuthenticationService>();
            authService
                .Setup(s => s.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);
            serviceProvider
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(authService.Object);

            httpContext.Setup(c => c.RequestServices).Returns(serviceProvider.Object);

            var actionContext = new ActionContext(httpContext.Object, new RouteData(), new PageActionDescriptor());
            var pageContext = new PageContext(actionContext) { HttpContext = httpContext.Object };

            var loginModel = new LoginModel(_context)
            {
                Input = new LoginModel.InputModel
                {
                    Username = "testuser",
                    Password = "testpass"
                },
                PageContext = pageContext
            };

            // Act
            var result = await loginModel.OnPostAsync();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Dashboard", redirectResult.PageName);
            Assert.Equal("Client", redirectResult.RouteValues["area"]);
            authService.Verify(s => s.SignInAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme, It.IsAny<ClaimsPrincipal>(), null), Times.Once());
        }

        [Fact]
        public async Task LoginModel_OnPostAsync_InvalidCredentials_ReturnsPageWithError()
        {
            var user = new User { Id = 1, Username = "testuser", Password = "testpass", Email = "test@example.com", RoleId = 3, Role = new Role { Id = 3, Name = "Client" } };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var httpContext = new Mock<HttpContext>();
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(u => u.Content(It.IsAny<string>())).Returns("~/");
            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            urlHelperFactory.Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>())).Returns(urlHelper.Object);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider
                .Setup(sp => sp.GetService(typeof(IUrlHelperFactory)))
                .Returns(urlHelperFactory.Object);
            var authService = new Mock<IAuthenticationService>();
            authService
                .Setup(s => s.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);
            serviceProvider
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(authService.Object);

            httpContext.Setup(c => c.RequestServices).Returns(serviceProvider.Object);

            var actionContext = new ActionContext(httpContext.Object, new RouteData(), new PageActionDescriptor());
            var pageContext = new PageContext(actionContext) { HttpContext = httpContext.Object };

            var loginModel = new LoginModel(_context)
            {
                Input = new LoginModel.InputModel
                {
                    Username = "testuser",
                    Password = "wrongpass"
                },
                PageContext = pageContext
            };

            // Act
            var result = await loginModel.OnPostAsync();

            // Assert
            var pageResult = Assert.IsType<PageResult>(result);
            Assert.Equal("Неверный логин или пароль.", loginModel.Message);

        }
    }
}