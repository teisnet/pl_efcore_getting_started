using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using SamuraiApp.Data;
using SamuraiApp.Domain;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using System.Linq;

namespace Tests
{
  [TestClass]
  public class InMemoryAttemptControllerIntegrationTests
   {
    private readonly WebApplicationFactory<SamuraiAPI.Startup> _factory;

    public InMemoryAttemptControllerIntegrationTests()
    {
      _factory = new WebApplicationFactory<SamuraiAPI.Startup>();
    }

    

    [TestMethod]
    public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
    {
      // Arrange
     
     // var client = _factory.CreateClient();
      var client = _factory.WithWebHostBuilder(builder =>
      {
        builder.ConfigureTestServices(services =>
        {
          var serviceDescriptors = services.Where(descriptor => (descriptor.ServiceType == typeof(DbContextOptions))|(descriptor.ServiceType == typeof(SamuraiContext))).ToList();

          foreach (var item in serviceDescriptors)
          {
            services.Remove(item);
          }
       
          services.AddDbContext<SamuraiContext>(opt =>
         opt.UseInMemoryDatabase("ControllerGet") );
        });
      })
       .CreateClient();

      // Act
      var response = await client.GetAsync("/api/Samurais");

      // Assert
      response.EnsureSuccessStatusCode(); // Status Code 200-299
      var responseString = await response.Content.ReadAsStringAsync();
      var responseObject = JsonConvert.DeserializeObject<List<Samurai>>(responseString);

      Assert.AreNotEqual(0, responseObject.Count);
    }
    
  [TestMethod]
  public async Task Post_CanInsertIntoDatabase()
  {
    // Arrange
    var client = _factory.CreateClient();
      var json = JsonConvert.SerializeObject(new Samurai { Name = "xyz" });//, Formatting.Indented);
      var httpContent = new StringContent(json,Encoding.Default,  "application/json");
      //// Act
      var response = await client.PostAsync("/api/SamuraisSoc", httpContent);
      // Assert
      response.EnsureSuccessStatusCode(); // Status Code 200-299
      var responseString = await response.Content.ReadAsStringAsync();
      var responseObject = JsonConvert.DeserializeObject<PostResponse>(responseString);
      
      Assert.IsTrue(responseObject.id>0);
  }
    private struct PostResponse
    {
      public int id;
      public Samurai samurai;
    }
}
  }
