using System;
using JustCSharp.FormWithJsonBody.ModelBindings;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JustCSharp.FormWithJsonBody.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FromFormWithJsonBodyAttribute: Attribute, IBindingSourceMetadata, IModelNameProvider
    {
        public BindingSource BindingSource => CustomBindingSource.FormWithJsonBody;
        
        public string Name { get; set; }
    }
}