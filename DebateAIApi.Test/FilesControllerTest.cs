using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using DebateAIApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace DebateAIApi.Test
{
    public class FilesControllerTest
    {
        IConfiguration _configuration;
        SecretsProvider _secretProvider;
        FilesController _controller;
        FileService _service;

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
            
            _secretProvider = new SecretsProvider(_configuration.GetSection("KeyVaultURL").Value!);
            _service = new FileService(_configuration, _secretProvider);
            _controller = new FilesController(_service);
        }

        [Fact]
        public void ListAllBlobsTest()
        {
            var result = _controller.ListAllBlobs();

            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result.Result);
            // then check contents of result
            Assert.IsType<List<BlobDto>>(result);
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