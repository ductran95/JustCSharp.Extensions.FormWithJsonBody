using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace JustCSharp.FormWithJsonBody.Examples.Models
{
    public class TestRequestListFile: TestRequest
    {
        public List<IFormFile> Images { get; set; }
    }
    
    public class TestRequestFileCollection: TestRequest
    {
        public IFormFileCollection Images { get; set; }
    }
}