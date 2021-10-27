using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

namespace JustCSharp.FormWithJsonBody.Formatters
{
    public abstract class JsonFormFormatter: IFormFormatter, IInputFormatterExceptionPolicy
    {
        public InputFormatterExceptionPolicy ExceptionPolicy => InputFormatterExceptionPolicy.MalformedInputExceptions;
        
        public abstract Task<InputFormatterResult> ReadFormAsync(FormFormatterContext context, string rawJson);
        
        public virtual Task<InputFormatterResult> ReadAsync(FormFormatterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Form.TryGetValue(context.ModelName, out var rawJson))
            {
                return ReadFormAsync(context, rawJson);
            }
            else
            {
                var exception = new Exception($"No form data match {context.ModelName}");
                context.ModelState.AddModelError(context.ModelName, exception, context.Metadata);

                return InputFormatterResult.FailureAsync();
            }
        }
    }
}