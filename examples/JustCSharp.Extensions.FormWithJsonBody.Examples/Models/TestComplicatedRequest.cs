using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace JustCSharp.FormWithJsonBody.Examples.Models
{
    public class TestComplicatedRequest: TestRequestWithFile
    {
        public List<TestChildRequest> Children { get; set; }
    }

    public class TestChildRequest
    {
        public string Code { get; set; }
        public string Text { get; set; }
        public IFormFile Sample { get; set; }
    }
}