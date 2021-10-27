using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JustCSharp.FormWithJsonBody.Formatters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace JustCSharp.FormWithJsonBody.ModelBindings
{
    public class FormWithJsonBodyModelBinder: IModelBinder
    {
        /// <summary>
        /// Returns UTF8 Encoding without BOM and throws on invalid bytes.
        /// </summary>
        protected static readonly Encoding UTF8EncodingWithoutBOM
            = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

        /// <summary>
        /// Returns UTF16 Encoding which uses littleEndian byte order with BOM and throws on invalid bytes.
        /// </summary>
        protected static readonly Encoding UTF16EncodingLittleEndian
            = new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true);
        
        protected readonly MvcOptions _mvcOptions;
        protected readonly FormWithJsonBodyModelBinderOptions _options;
        protected readonly ILoggerFactory _loggerFactory;
        protected readonly IFormFormatter _formFormatter;
        
        private readonly ILogger<FormWithJsonBodyModelBinder> _logger;
        
        public FormWithJsonBodyModelBinder(
            FormWithJsonBodyModelBinderOptions options, 
            MvcOptions mvcOptions,
            IFormFormatter formFormatter,
            ILoggerFactory loggerFactory
        )
        {
            _options = options;
            _mvcOptions = mvcOptions;
            _formFormatter = formFormatter;
            _loggerFactory = loggerFactory;

            _logger = _loggerFactory.CreateLogger<FormWithJsonBodyModelBinder>();
        }
        
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            object dataModel = null;
            bool isModelSet = false;
            
            try
            {
                // Create an instance for ModelType
                dataModel = Activator.CreateInstance(bindingContext.ModelType);
            }
            catch (Exception)
            {
                bindingContext.Result = ModelBindingResult.Failed();

                return;
            }
            
            var modelProperties = bindingContext.ModelType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();

            var request = bindingContext.ActionContext.HttpContext.Request;
            IFormCollection form;
            try
            {
                form = await request.ReadFormAsync();
            }
            catch (InvalidDataException ex)
            {
                // ReadFormAsync can throw InvalidDataException if the form content is malformed.
                // Wrap it in a ValueProviderException that the CompositeValueProvider special cases.
#if (NET5_0)
                throw new ValueProviderException("Failed to read Form data", ex);
#else
                throw new Exception("Failed to read Form data", ex);
#endif

            }
            catch (IOException ex)
            {
                // ReadFormAsync can throw IOException if the client disconnects.
                // Wrap it in a ValueProviderException that the CompositeValueProvider special cases.
#if (NET5_0)
                throw new ValueProviderException("Failed to read Form data", ex);
#else
                throw new Exception("Failed to read Form data", ex);
#endif
            }
            
            var allowEmptyInputInModelBinding = _mvcOptions?.AllowEmptyInputInBodyModelBinding == true;
            
            var formatterContext = new FormFormatterContext(
                bindingContext.HttpContext,
                _options.BodyName,
                bindingContext.ModelState,
                bindingContext.ModelMetadata,
                form,
                allowEmptyInputInModelBinding);
            
            try
            {
                var result = await _formFormatter.ReadAsync(formatterContext);

                if (result.HasError)
                {
                    // Formatter encountered an error. Do not use the model it returned.
                    // _logger.DoneAttemptingToBindModel(bindingContext);
                    return;
                }

                if (result.IsModelSet)
                {
                    isModelSet = true;
                    dataModel = result.Model;
                }
                else
                {
                    // If the input formatter gives a "no value" result, that's always a model state error,
                    // because BodyModelBinder implicitly regards input as being required for model binding.
                    // If instead the input formatter wants to treat the input as optional, it must do so by
                    // returning InputFormatterResult.Success(defaultForModelType), because input formatters
                    // are responsible for choosing a default value for the model type.
                    var message = bindingContext
                        .ModelMetadata
                        .ModelBindingMessageProvider
                        .MissingRequestBodyRequiredValueAccessor();
                    bindingContext.ModelState.AddModelError(_options.BodyName, message);
                }
            }
            catch (Exception exception) when (exception is InputFormatterException || ShouldHandleException(_formFormatter))
            {
                bindingContext.ModelState.AddModelError(_options.BodyName, exception, bindingContext.ModelMetadata);
            }
            
            // Set File Property from Form
            foreach (var modelProperty in modelProperties)
            {
                if (typeof(IFormFile).IsAssignableFrom(modelProperty.PropertyType))
                {
                    try
                    {
                        modelProperty.SetValue(dataModel, form.Files.GetFile(modelProperty.Name));
                        isModelSet = true;
                    }
                    catch (Exception) { }
                }
                else if (typeof(IFormFileCollection).IsAssignableFrom(modelProperty.PropertyType))
                {
                    try
                    {
                        var files = form.Files.GetFiles(modelProperty.Name);
                        
                        var formFileCollection = new FormFileCollection();
                        formFileCollection.AddRange(files);
                        
                        modelProperty.SetValue(dataModel, formFileCollection);
                        
                        isModelSet = true;
                    }
                    catch (Exception) { }
                }
                else if (typeof(IEnumerable<IFormFile>).IsAssignableFrom(modelProperty.PropertyType))
                {
                    try
                    {
                        modelProperty.SetValue(dataModel, form.Files.GetFiles(modelProperty.Name).ToList());
                        isModelSet = true;
                    }
                    catch (Exception) { }
                }
                else if (typeof(IFormFile[]).IsAssignableFrom(modelProperty.PropertyType))
                {
                    try
                    {
                        modelProperty.SetValue(dataModel, form.Files.GetFiles(modelProperty.Name).ToArray());
                        isModelSet = true;
                    }
                    catch (Exception) { }
                }
            }

            if (isModelSet)
            {
                bindingContext.Result = ModelBindingResult.Success(dataModel);
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(null);
            }
        }
        
        private bool ShouldHandleException(IFormFormatter formFormatter)
        {
            // Any explicit policy on the formatters overrides the default.
            var policy = (formFormatter as IInputFormatterExceptionPolicy)?.ExceptionPolicy ??
                         InputFormatterExceptionPolicy.MalformedInputExceptions;

            return policy == InputFormatterExceptionPolicy.AllExceptions;
        }
    }
}