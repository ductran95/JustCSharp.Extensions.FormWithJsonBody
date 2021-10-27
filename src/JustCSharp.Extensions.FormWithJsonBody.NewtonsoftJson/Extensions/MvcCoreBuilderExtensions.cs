using System;
using JustCSharp.Extensions.FormWithJsonBody.Extensions;
using JustCSharp.Extensions.FormWithJsonBody.Formatters;
using JustCSharp.Extensions.FormWithJsonBody.ModelBindings;
using JustCSharp.Extensions.FormWithJsonBody.NewtonsoftJson.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JustCSharp.Extensions.FormWithJsonBody.NewtonsoftJson.Extensions
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