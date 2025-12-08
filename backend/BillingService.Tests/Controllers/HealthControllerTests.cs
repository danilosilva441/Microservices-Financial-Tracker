// using BillingService.Controllers;
// using FluentAssertions;
// using Microsoft.AspNetCore.Mvc;
// using Xunit;

// namespace BillingService.Tests.Controllers
// {
//     public class HealthControllerTests
//     {
//         [Fact]
//         public void Get_DeveRetornarOk()
//         {
//             // Arrange
//             var controller = new HealthController();

//             // Act
//             var result = controller.Get();

//             // Assert
//             var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//             okResult.Value.Should().Be("Healthy"); // Ou o que o seu controller retornar
//         }
//     }
// }