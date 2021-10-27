using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using JustCSharp.FormWithJsonBody.ModelBindings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace JustCSharp.FormWithJsonBody.ApiExplorer
{
    public class FomWithJsonBodyApiDescriptionProvider: IApiDescriptionProvider
    {
        private readonly MvcOptions _mvcOptions;
        private readonly FormWithJsonBodyModelBinderOptions _options;
        private readonly RouteOptions _routeOptions;
        private readonly IInlineConstraintResolver _constraintResolver;
        private readonly IModelMetadataProvider _modelMetadataProvider;
        
        public int Order => 0;
        
        public FomWithJsonBodyApiDescriptionProvider(
            IOptions<MvcOptions> mvcOptionsAccessor,
            IOptions<FormWithJsonBodyModelBinderOptions> optionsAccessor,
            IInlineConstraintResolver constraintResolver,
            IModelMetadataProvider modelMetadataProvider,
            IActionResultTypeMapper mapper,
            IOptions<RouteOptions> routeOptions)
        {
            _mvcOptions = mvcOptionsAccessor.Value;
            _options = optionsAccessor.Value;
            _constraintResolver = constraintResolver;
            _modelMetadataProvider = modelMetadataProvider;
            _routeOptions = routeOptions.Value;
        }
        
        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
            var result = context.Results;

            foreach (var apiDescription in result)
            {
                var formParameter =
                    apiDescription.ParameterDescriptions.FirstOrDefault(x =>
                        x.Source == CustomBindingSource.FormWithJsonBody);

                if (formParameter != null)
                {
                    formParameter.Source = BindingSource.Form;
                    formParameter.Name = _options.BodyName;

                    var formParameterProperties = formParameter.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var propertyInfo in formParameterProperties)
                    {
                        if (typeof(IFormFile).IsAssignableFrom(propertyInfo.PropertyType)
                        || typeof(IFormFileCollection).IsAssignableFrom(propertyInfo.PropertyType)
                        || typeof(IEnumerable<IFormFile>).IsAssignableFrom(propertyInfo.PropertyType)
                        || typeof(IFormFile[]).IsAssignableFrom(propertyInfo.PropertyType))
                        {
                            var isRequired = propertyInfo.GetCustomAttribute<RequiredAttribute>() != null;
                            var modelMetadata = _modelMetadataProvider.GetMetadataForProperty(formParameter.Type, propertyInfo.Name);
                            var parameterDescriptor = new ParameterDescriptor
                            {
                                Name = propertyInfo.Name,
#if (NET5_0)
                                BindingInfo = formParameter.BindingInfo,
#else
                                BindingInfo = formParameter.ParameterDescriptor.BindingInfo,
#endif
                                ParameterType = propertyInfo.PropertyType
                            };
                            
                            var propertyParameter = new ApiParameterDescription
                            {
                                Source = formParameter.Source,
                                Type = propertyInfo.PropertyType,
                                Name = propertyInfo.Name,
                                IsRequired = isRequired,
                                ModelMetadata = modelMetadata,
#if (NET5_0)
                                BindingInfo = formParameter.BindingInfo,
#endif
                                ParameterDescriptor = parameterDescriptor
                            };
                            
                            apiDescription.ParameterDescriptions.Add(propertyParameter);
                        }
                    }
                }
            }
        }

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
        }
    }
}