using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JustCSharp.Extensions.FormWithJsonBody.Formatters
{
    public class FormFormatterContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="FormFormatterContext"/>.
        /// </summary>
        /// <param name="httpContext">
        /// The <see cref="Microsoft.AspNetCore.Http.HttpContext"/> for the current operation.
        /// </param>
        /// <param name="modelName">The name of the model.</param>
        /// <param name="modelState">
        /// The <see cref="ModelStateDictionary"/> for recording errors.
        /// </param>
        /// <param name="metadata">
        /// The <see cref="ModelMetadata"/> of the model to deserialize.
        /// </param>
        /// <param name="form">
        /// Request form data
        /// </param>
        public FormFormatterContext(
            HttpContext httpContext,
            string modelName,
            ModelStateDictionary modelState,
            ModelMetadata metadata,
            IFormCollection form)
            : this(httpContext, modelName, modelState, metadata, form, treatEmptyInputAsDefaultValue: false)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="FormFormatterContext"/>.
        /// </summary>
        /// <param name="httpContext">
        /// The <see cref="Microsoft.AspNetCore.Http.HttpContext"/> for the current operation.
        /// </param>
        /// <param name="modelName">The name of the model.</param>
        /// <param name="modelState">
        /// The <see cref="ModelStateDictionary"/> for recording errors.
        /// </param>
        /// <param name="metadata">
        /// The <see cref="ModelMetadata"/> of the model to deserialize.
        /// </param>
        /// <param name="form">
        /// Request form data
        /// </param>
        /// <param name="treatEmptyInputAsDefaultValue">
        /// A value for the <see cref="TreatEmptyInputAsDefaultValue"/> property.
        /// </param>
        public FormFormatterContext(
            HttpContext httpContext,
            string modelName,
            ModelStateDictionary modelState,
            ModelMetadata metadata,
            IFormCollection form,
            bool treatEmptyInputAsDefaultValue)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (modelName == null)
            {
                throw new ArgumentNullException(nameof(modelName));
            }

            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }

            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (form == null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            HttpContext = httpContext;
            ModelName = modelName;
            ModelState = modelState;
            Metadata = metadata;
            Form = form;
            TreatEmptyInputAsDefaultValue = treatEmptyInputAsDefaultValue;
            ModelType = metadata.ModelType;
        }

        /// <summary>
        /// Gets a flag to indicate whether the input formatter should allow no value to be provided.
        /// If <see langword="false"/>, the input formatter should handle empty input by returning
        /// <see cref="InputFormatterResult.NoValueAsync()"/>. If <see langword="true"/>, the input
        /// formatter should handle empty input by returning the default value for the type
        /// <see cref="ModelType"/>.
        /// </summary>
        public bool TreatEmptyInputAsDefaultValue { get; }

        /// <summary>
        /// Gets the <see cref="Microsoft.AspNetCore.Http.HttpContext"/> associated with the current operation.
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// Gets the name of the model. Used as the key or key prefix for errors added to <see cref="ModelState"/>.
        /// </summary>
        public string ModelName { get; }

        /// <summary>
        /// Gets the <see cref="ModelStateDictionary"/> associated with the current operation.
        /// </summary>
        public ModelStateDictionary ModelState { get; }

        /// <summary>
        /// Gets the requested <see cref="ModelMetadata"/> of the request body deserialization.
        /// </summary>
        public ModelMetadata Metadata { get; }

        /// <summary>
        /// Gets the requested <see cref="Type"/> of the request body deserialization.
        /// </summary>
        public Type ModelType { get; }
        
        /// <summary>
        /// Form data
        /// </summary>
        public IFormCollection Form { get; }
    }
}