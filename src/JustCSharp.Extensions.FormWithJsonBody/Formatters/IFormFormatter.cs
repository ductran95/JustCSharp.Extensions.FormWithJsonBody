using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace JustCSharp.FormWithJsonBody.Formatters
{
    public interface IFormFormatter
    {
        Task<InputFormatterResult> ReadAsync(FormFormatterContext context);
    }
}