// using BillingService.Controllers;
// using BillingService.DTOs;
// using BillingService.Models;
// using BillingService.Services.Interfaces;
// using FluentAssertions;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using SharedKernel;
// using System.Security.Claims;
// using Xunit;

// namespace BillingService.Tests.Controllers
// {
//     public class ExpensesControllerTests
//     {
//         private readonly Mock<IExpenseService> _serviceMock;
//         private readonly ExpensesController _controller;

//         public ExpensesControllerTests()
//         {
//             _serviceMock = new Mock<IExpenseService>();
//             _controller = new ExpensesController(_serviceMock.Object);
            
//             var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
//             {
//                 new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
//                 new Claim("tenantId", Guid.NewGuid().ToString())
//             }, "TestAuth"));
            
//             _controller.ControllerContext = new ControllerContext
//             {
//                 HttpContext = new DefaultHttpContext { User = user }
//             };
//         }

//         [Fact]
//         public async Task CreateCategory_Sucesso_DeveRetornarCreated()
//         {
//             var dto = new ExpenseCategoryCreateDto { Name = "Cat1" };
//             _serviceMock.Setup(s => s.CreateCategoryAsync(dto, It.IsAny<Guid>()))
//                 .ReturnsAsync((new ExpenseCategory { Id = Guid.NewGuid() }, null));

//             var result = await _controller.CreateCategory(dto);

//             result.Should().BeOfType<CreatedAtActionResult>();
//         }

//         [Fact]
//         public async Task CreateExpense_UnidadeInvalida_DeveRetornarNotFound()
//         {
//             var dto = new ExpenseCreateDto();
//             _serviceMock.Setup(s => s.CreateExpenseAsync(dto, It.IsAny<Guid>()))
//                 .ReturnsAsync((null, ErrorMessages.UnidadeNotFound));

//             var result = await _controller.CreateExpense(dto);

//             var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
//             notFound.Value.Should().Be(ErrorMessages.UnidadeNotFound);
//         }
//     }
// }