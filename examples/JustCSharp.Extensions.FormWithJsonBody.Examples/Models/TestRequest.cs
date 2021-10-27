using System.ComponentModel.DataAnnotations;

namespace JustCSharp.FormWithJsonBody.Examples.Models
{
    public class TestRequest
    {
        [Required]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}