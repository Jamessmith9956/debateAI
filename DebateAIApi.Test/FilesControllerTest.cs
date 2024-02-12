using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DebateAIApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;

namespace DebateAIApi.Test
{
    public class FilesControllerTest
    {
        IConfiguration _configuration;
        SecretsProvider _secretProvider;
        private Mock<IFileService> _serviceMock;
        FilesController _controller;
        //IFileService _service;

        public FilesControllerTest()
        {
            // consider using a webfactory https://timdeschryver.dev/blog/how-to-test-your-csharp-web-api#writing-your-own-webapplicationfactory
            // consider setting up a configuration provider
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json")
                .AddEnvironmentVariables();
            //replace with a configuration provider
            builder.GetFileProvider();
            _configuration = builder.Build();
            
            //_secretProvider = new SecretsProvider(_configuration.GetSection("KeyVaultURL").Value!);
            _serviceMock = new Mock<IFileService>(); 
            var blobList = new List<BlobDto>
            {
                new BlobDto { Name = "test1", Uri = "https://test1.com", ContentType = "text/plain" },
                new BlobDto { Name = "test2", Uri = "https://test2.com", ContentType = "text/plain" },
                new BlobDto { Name = "test3", Uri = "https://test3.com", ContentType = "text/plain" }
            };
            _serviceMock.Setup(x => x.ListAsync()).ReturnsAsync(blobList);
            _controller = new FilesController(_serviceMock.Object);
        }

        [Fact]
        public async Task ListAllBlobsTestAsync()
        {
            var result = await _controller.ListAllBlobs();

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            // then check contents of result
            // break open the result and check the contents
            var okResult = result as OkObjectResult;
            var blobList = okResult.Value as List<BlobDto>;

            Assert.Equal(3, blobList.Count());
        }

        [Fact]
        public void UploadTest()
        {
            Assert.True(false);
        }

        [Fact]
        public void DownloadTest()
        {
            Assert.True(false);
        }

        [Fact]
        public void DeleteTest()
        {
            Assert.True(false);
        }
    }
}