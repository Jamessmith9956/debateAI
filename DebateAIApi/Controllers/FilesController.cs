using Microsoft.AspNetCore.Mvc;

namespace DebateAIApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : ControllerBase
    {

        private readonly IFileService _fileservice; // interface the file service for spoofing

        public FilesController(IFileService fileservice)
        {
            _fileservice = fileservice;
        }


        [HttpGet]
        public async Task<IActionResult> ListAllBlobs()
        {
            var result = await _fileservice.ListAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var result = await _fileservice.UploadAsync(file);
            return Ok(result);
        }

        
        [HttpGet]
        [Route("filename")]
        public async Task<IActionResult> Download(string filename)
        {
            // note for download we could have just used the blob uri
            var result = await _fileservice.DownloadAsync(filename);
            return File(result.Content, result.ContentType, result.Name);
        }

        [HttpDelete]
        [Route("filename")]
        public async Task<IActionResult> Delete(string filename)
        {
            var result = await _fileservice.DeleteAsync(filename);
            return Ok(result);
        }

    }
}
