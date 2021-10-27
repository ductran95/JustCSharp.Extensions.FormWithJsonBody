using Microsoft.AspNetCore.Http;

namespace JustCSharp.FormWithJsonBody.Examples.Models
{
    public class TestRequestWithFile: TestRequest
    {
        public IFormFile Image { get; set; }
    }
    
    public class TestRequestMultipleFile: TestRequest
    {
        public IFormFile Image { get; set; }
        public IFormFile Document { get; set; }
    }
}