using JustCSharp.FormWithJsonBody.Attributes;
using JustCSharp.FormWithJsonBody.Examples.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JustCSharp.FormWithJsonBody.Examples.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FormController: ControllerBase
    {
        [HttpPost]
        public IActionResult RequestWithoutFile([FromForm] TestRequest request) => StatusCode(StatusCodes.Status200OK);
        
        [HttpPost]
        public IActionResult RequestWithFile([FromForm] TestRequestWithFile request) => StatusCode(StatusCodes.Status200OK);
        
        [HttpPost]
        public IActionResult RequestWithMultipleFile([FromForm] TestRequestMultipleFile request) => StatusCode(StatusCodes.Status200OK);
        
        [HttpPost]
        public IActionResult RequestWithListFile([FromForm] TestRequestListFile request) => StatusCode(StatusCodes.Status200OK);
        
        [HttpPost]
        public IActionResult RequestWithFileCollection([FromForm] TestRequestFileCollection request) => StatusCode(StatusCodes.Status200OK);
        
        [HttpPost]
        public IActionResult RequestWithChildren([FromForm] TestComplicatedRequest request) => StatusCode(StatusCodes.Status200OK);
    }
}