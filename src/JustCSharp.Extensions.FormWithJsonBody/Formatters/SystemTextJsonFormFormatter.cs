using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
#if (!NETSTANDARD2_0)
    using Microsoft.Extensions.Options;
#endif
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;

namespace JustCSharp.FormWithJsonBody.Formatters
{
    public class SystemTextJsonFormFormatter : JsonFormFormatter
    {
        private readonly ILogger<SystemTextJsonFormFormatter> _logger;
        public JsonSerializerOptions SerializerOptions { get; }

        public SystemTextJsonFormFormatter(
#if (!NETSTANDARD2_0)
            IOptions<JsonOptions> jsonOptions,
#endif
            ILogger<SystemTextJsonFormFormatter> logger)
        {
#if (!NETSTANDARD2_0)
            SerializerOptions = jsonOptions.Value.JsonSerializerOptions;
#else 
            SerializerOptions = new JsonSerializerOptions();
#endif

            _logger = logger;
        }

        public override Task<InputFormatterResult> ReadFormAsync(FormFormatterContext context, string rawJson)
        {
            var httpContext = context.HttpContext;

            object model;
            try
            {
                model = JsonSerializer.Deserialize(rawJson, context.ModelType, SerializerOptions);
            }
            catch (JsonException jsonException)
            {
                var path = jsonException.Path;

                var formatterException = new InputFormatterException(jsonException.Message, jsonException);

                context.ModelState.TryAddModelError(path, formatterException, context.Metadata);

                // Log.JsonInputException(_logger, jsonException);

                return InputFormatterResult.FailureAsync();
            }
            catch (Exception exception) when (exception is FormatException || exception is OverflowException)
            {
                // The code in System.Text.Json never throws these exceptions. However a custom converter could produce these errors for instance when
                // parsing a value. These error messages are considered safe to report to users using ModelState.

                context.ModelState.TryAddModelError(string.Empty, exception, context.Metadata);
                // Log.JsonInputException(_logger, exception);

                return InputFormatterResult.FailureAsync();
            }

            if (model == null && !context.TreatEmptyInputAsDefaultValue)
            {
                // Some nonempty inputs might deserialize as null, for example whitespace,
                // or the JSON-encoded value "null". The upstream BodyModelBinder needs to
                // be notified that we don't regard this as a real input so it can register
                // a model binding error.
                return InputFormatterResult.NoValueAsync();
            }
            else
            {
                // Log.JsonInputSuccess(_logger, context.ModelType);
                return InputFormatterResult.SuccessAsync(model);
            }
        }
    }
}