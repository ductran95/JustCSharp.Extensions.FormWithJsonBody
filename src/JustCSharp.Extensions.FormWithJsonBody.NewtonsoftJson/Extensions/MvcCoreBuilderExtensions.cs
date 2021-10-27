using System;
using JustCSharp.FormWithJsonBody.Formatters;
using JustCSharp.FormWithJsonBody.ModelBindings;
using JustCSharp.FormWithJsonBody.NewtonsoftJson.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcCoreBuilderExtensions
    {
        public static IMvcBuilder AddFormWithNewtonsoftJsonBody(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));

            return mvcBuilder.AddFormWithNewtonsoftJsonBody(null);
        }
        
        public static IMvcBuilder AddFormWithNewtonsoftJsonBody(this IMvcBuilder mvcBuilder, Action<FormWithJsonBodyModelBinderOptions> setupAction)
        {
            mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));

            mvcBuilder.AddFormWithJsonBodyCore(setupAction);
            
            mvcBuilder.Services.Configure<MvcOptions>(c =>
                c.AddFormWithJsonBodyCore());

            mvcBuilder.Services.TryAddSingleton<IFormFormatter, NewtonsoftJsonFormFormatter>();

            return mvcBuilder;
        }
    }
}