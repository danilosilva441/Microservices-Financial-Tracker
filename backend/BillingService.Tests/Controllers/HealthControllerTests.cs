using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BillingService.Tests.Controllers
{
    public class HealthControllerTests
    {
        private readonly Type _controllerType;
        private readonly BillingService.Controllers.HealthController _controller;

        public HealthControllerTests()
        {
            _controllerType = typeof(BillingService.Controllers.HealthController);
            _controller = new BillingService.Controllers.HealthController();
        }

        [Fact]
        public void HealthController_ShouldExist()
        {
            // Assert
            Assert.NotNull(_controllerType);
            Assert.Equal("HealthController", _controllerType.Name);
            Assert.Equal("BillingService.Controllers", _controllerType.Namespace);
        }

        [Fact]
        public void CheckHealth_MethodShouldExist()
        {
            // Act
            var method = _controllerType.GetMethod("CheckHealth");

            // Assert
            Assert.NotNull(method);
            Assert.Equal("CheckHealth", method!.Name);
            Assert.True(method.IsPublic);
        }

        [Fact]
        public void CheckHealth_ShouldReturnOkObjectResult()
        {
            // Act
            var result = _controller.CheckHealth();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void CheckHealth_ShouldReturnStatusCode200()
        {
            // Act
            var result = _controller.CheckHealth() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result!.StatusCode);
        }

        [Fact]
        public void CheckHealth_ShouldReturnCorrectMessage()
        {
            // Act
            var result = _controller.CheckHealth() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("BillingService is Healthy", result!.Value);
        }

        [Fact]
        public void CheckHealth_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var method = _controllerType.GetMethod("CheckHealth");

            // Act
            var attributes = method!.GetCustomAttributes(typeof(HttpGetAttribute), false);

            // Assert
            Assert.NotEmpty(attributes);
            var httpGetAttr = attributes[0] as HttpGetAttribute;
            Assert.Equal("/health", httpGetAttr!.Template);
        }

        [Fact]
        public void HealthController_ShouldHaveApiControllerAttribute()
        {
            // Act
            var attributes = _controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);

            // Assert
            Assert.NotEmpty(attributes);
        }

        [Fact]
        public void HealthController_ShouldExtendControllerBase()
        {
            // Assert
            Assert.True(typeof(ControllerBase).IsAssignableFrom(_controllerType));
        }

        [Fact]
        public void CheckHealth_ShouldReturnConsistentMessage()
        {
            // Act
            var result1 = _controller.CheckHealth() as OkObjectResult;
            var result2 = _controller.CheckHealth() as OkObjectResult;

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal(result1!.Value, result2!.Value);
        }
    }
}