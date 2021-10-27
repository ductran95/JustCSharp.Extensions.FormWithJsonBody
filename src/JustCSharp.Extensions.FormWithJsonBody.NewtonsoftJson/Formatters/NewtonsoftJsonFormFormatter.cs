using System;
using System.Threading.Tasks;
using JustCSharp.FormWithJsonBody.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace JustCSharp.FormWithJsonBody.NewtonsoftJson.Formatters
{
    public class NewtonsoftJsonFormFormatter: JsonFormFormatter
    {
        private readonly ILogger<NewtonsoftJsonFormFormatter> _logger;
        public JsonSerializerSettings JsonSerializerSettings { get; }
        
        public NewtonsoftJsonFormFormatter(
#if (!NETSTANDARD2_0)
            IOptions<MvcNewtonsoftJsonOptions> jsonOptions,
#endif
            ILogger<NewtonsoftJsonFormFormatter> logger)
        {
#if (!NETSTANDARD2_0)
            JsonSerializerSettings = jsonOptions.Value.SerializerSettings;
#else 
            JsonSerializerSettings = new JsonSerializerSettings();
#endif
            _logger = logger;
        }

        public override Task<InputFormatterResult> ReadFormAsync(FormFormatterContext context, string rawJson)
        {
            var httpContext = context.HttpContext;

            object model;
            try
            {
                model = JsonConvert.DeserializeObject(rawJson, context.ModelType, JsonSerializerSettings);
            }
            catch (JsonSerializationException jsonException)
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