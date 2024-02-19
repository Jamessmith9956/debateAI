using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DebateAIApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using System.Text;

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
            //Arrange
            //Act
            var result = await _controller.ListAllBlobs();
            //Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            // then check contents of result
            // break open the result and check the contents
            var okResult = result as OkObjectResult;
            var blobList = okResult.Value as List<BlobDto>;

            Assert.Equal(3, blobList.Count());
        }

        [Fact]
        public async Task UploadTest()
        {
            //Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            //Act
            var result = await _controller.Upload(fileMock.Object);
            //Assert result is ok 200
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DownloadTest() // Please Check
        {
            //Arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("This is a test"));
            _serviceMock.Setup(x => x.DownloadAsync("test.txt"))
                .ReturnsAsync(new BlobDto { Name = "test.txt", Content = stream, ContentType = "text/plain" });
            //Act
            var result = await _controller.Download("test.txt");
            //Assert
            Assert.NotNull(result);
            Assert.IsType<FileStreamResult>(result);
            var fileResult = result as FileStreamResult;
            Assert.Equal("test.txt", fileResult.FileDownloadName);
            Assert.Equal("text/plain", fileResult.ContentType);
            Assert.Equal(stream, fileResult.FileStream);
        }

        [Fact]
        public async Task DeleteTest()
        {
            //Arrange

            //Act
            var result = await _controller.Delete("test.txt");

            //Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            
        }
    }
}