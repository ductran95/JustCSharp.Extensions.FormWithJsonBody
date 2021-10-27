using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JustCSharp.Extensions.FormWithJsonBody.ModelBindings;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JustCSharp.Extensions.FormWithJsonBody.Swashbuckle.Filters
{
    public class FormWithJsonBodyFilter: IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var apiDescription in context.ApiDescriptions)
            {
#if (NET5_0)
            var formParameter = apiDescription.ParameterDescriptions.FirstOrDefault(x =>
                        x.BindingInfo?.BindingSource == CustomBindingSource.FormWithJsonBody);
#else
            var formParameter = apiDescription.ParameterDescriptions.FirstOrDefault(x =>
                x.ParameterDescriptor.BindingInfo?.BindingSource == CustomBindingSource.FormWithJsonBody);
#endif

                if (formParameter != null)
                {
                    if(context.SchemaRepository.TryLookupByType(formParameter.Type, out var paramSchema))
                    {
                        if (swaggerDoc.Components.Schemas.ContainsKey(paramSchema.Reference.Id))
                        {
                            var schema = swaggerDoc.Components.Schemas[paramSchema.Reference.Id];
                            var formParameterTypeProperties = formParameter.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                            
                            foreach (var propertyInfo in formParameterTypeProperties)
                            {
                                // Remove IFormFile property from Schema
                                if (typeof(IFormFile).IsAssignableFrom(propertyInfo.PropertyType)
                                    || typeof(IFormFileCollection).IsAssignableFrom(propertyInfo.PropertyType)
                                    || typeof(IEnumerable<IFormFile>).IsAssignableFrom(propertyInfo.PropertyType)
                                    || typeof(IFormFile[]).IsAssignableFrom(propertyInfo.PropertyType))
                                {
                                    var schemaProperty = schema.Properties.Where(x =>
                                        x.Key.Equals(propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase)).ToList();

                                    schemaProperty.ForEach(x => schema.Properties.Remove(x.Key));
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}