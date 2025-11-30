// using BillingService.Data;
// using BillingService.Models;
// using FluentAssertions;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Data.Sqlite;
// using Microsoft.EntityFrameworkCore;
// using Moq;
// using System.Security.Claims;
// using Xunit;
// using Xunit.Abstractions;
// using Microsoft.Extensions.DependencyInjection;

// namespace BillingService.Tests.Security
// {
//     public class TenantIsolationTests : IDisposable
//     {
//         private readonly string _dbFilePath;
//         private readonly ITestOutputHelper _output;

//         public TenantIsolationTests(ITestOutputHelper output)
//         {
//             _output = output;
//             // Cria um nome de ficheiro temporário único para isolar o teste
//             _dbFilePath = Path.Combine(Path.GetTempPath(), $"BillingDb_{Guid.NewGuid()}.db");

//             // Inicializa o banco (Schema) como Admin (sem tenant)
//             using (var context = CreateContext(null))
//             {
//                 context.Database.EnsureCreated();
//             }
//         }

//         public void Dispose()
//         {
//             // Apaga o ficheiro do banco após o teste
//             if (File.Exists(_dbFilePath))
//             {
//                 try { File.Delete(_dbFilePath); } catch { }
//             }
//         }

//         private BillingDbContext CreateContext(Guid? tenantId)
//         {
//             var connectionString = $"Data Source={_dbFilePath}";

//             // Criar um novo ServiceProvider para cada contexto para evitar cache de modelo/serviço
//             var serviceProvider = new ServiceCollection()
//                 .AddEntityFrameworkSqlite()
//                 .AddSingleton<IHttpContextAccessor>(sp =>
//                 {
//                     var mock = new Mock<IHttpContextAccessor>();
//                     var context = new DefaultHttpContext();
//                     if (tenantId.HasValue)
//                     {
//                         context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
//                         {
//                             new Claim("tenantId", tenantId.Value.ToString())
//                         }, "TestAuth"));
//                     }
//                     mock.Setup(x => x.HttpContext).Returns(context);
//                     return mock.Object;
//                 })
//                 .BuildServiceProvider();

//             var options = new DbContextOptionsBuilder<BillingDbContext>()
//                 .UseSqlite(connectionString)
//                 .UseInternalServiceProvider(serviceProvider) // Força uso de novo SP
//                 .EnableServiceProviderCaching(false)
//                 .Options;

//             // Precisamos passar um IHttpContextAccessor para o construtor também, 
//             // mesmo que o UseInternalServiceProvider já tenha um configurado internamente para o EF.
//             // O DbContext usa o que é passado no construtor para definir a propriedade CurrentTenantId.
//             var accessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

//             return new BillingDbContext(options, accessor);
//         }

//         [Fact]
//         public async Task GlobalQueryFilter_DeveFiltrarApenasDadosDoTenantCorreto()
//         {
//             // Arrange
//             var tenantA = Guid.NewGuid();
//             var tenantB = Guid.NewGuid();

//             // 1. Inserir dados como Admin (sem filtro)
//             using (var dbAdmin = CreateContext(null))
//             {
//                 dbAdmin.Unidades.Add(new Unidade { Id = Guid.NewGuid(), Nome = "Unidade A", TenantId = tenantA, MetaMensal = 1000, UserId = Guid.NewGuid() });
//                 dbAdmin.Unidades.Add(new Unidade { Id = Guid.NewGuid(), Nome = "Unidade B", TenantId = tenantB, MetaMensal = 2000, UserId = Guid.NewGuid() });
//                 await dbAdmin.SaveChangesAsync();
//                 _output.WriteLine("Dados inseridos no SQLite (ficheiro).");
//             }

//             // Act & Assert - Tenant A
//             using (var dbUserA = CreateContext(tenantA))
//             {
//                 _output.WriteLine($"Lendo como Tenant A: {tenantA}");

//                 // O filtro global deve ser aplicado aqui
//                 var unidadesVisiveis = await dbUserA.Unidades.ToListAsync();

//                 foreach (var u in unidadesVisiveis) _output.WriteLine($" - Viu: {u.Nome}");

//                 unidadesVisiveis.Should().HaveCount(1, "o filtro de segurança falhou e mostrou dados de outro tenant!");
//                 unidadesVisiveis.First().Nome.Should().Be("Unidade A");
//             }

//             // Act & Assert - Tenant B
//             using (var dbUserB = CreateContext(tenantB))
//             {
//                 var unidadesVisiveis = await dbUserB.Unidades.ToListAsync();
//                 unidadesVisiveis.Should().HaveCount(1);
//                 unidadesVisiveis.First().Nome.Should().Be("Unidade B");
//             }
//         }
//     }
// }