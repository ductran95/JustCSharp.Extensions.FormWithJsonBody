using JustCSharp.FormWithJsonBody.Attributes;
using JustCSharp.FormWithJsonBody.Examples.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JustCSharp.FormWithJsonBody.Examples.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FormWithJsonController: ControllerBase
    {
        [HttpPost]
        public IActionResult RequestWithoutFile([FromFormWithJsonBody] TestRequest request) => StatusCode(StatusCodes.Status200OK);
        
        [HttpPost]
        public IActionResult RequestWithFile([FromFormWithJsonBody] TestRequestWithFile request) => StatusCode(StatusCodes.Status200OK);
        
        [HttpPost]
        public IActionResult RequestWithMultipleFile([FromFormWithJsonBody] TestRequestMultipleFile request) => StatusCode(StatusCodes.Status200OK);
        
        [HttpPost]
        public IActionResult RequestWithListFile([FromFormWithJsonBody] TestRequestListFile request) => StatusCode(StatusCodes.Status200OK);
        
        [HttpPost]
        public IActionResult RequestWithFileCollection([FromFormWithJsonBody] TestRequestFileCollection request) => StatusCode(StatusCodes.Status200OK);
        
        [HttpPost]
        public IActionResult RequestWithChildren([FromFormWithJsonBody] TestComplicatedRequest request) => StatusCode(StatusCodes.Status200OK);
    }
}