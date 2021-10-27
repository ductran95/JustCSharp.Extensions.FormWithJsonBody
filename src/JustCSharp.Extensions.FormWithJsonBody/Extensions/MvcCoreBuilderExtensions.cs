using System;
using JustCSharp.Extensions.FormWithJsonBody.ApiExplorer;
using JustCSharp.Extensions.FormWithJsonBody.Formatters;
using JustCSharp.Extensions.FormWithJsonBody.ModelBindings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JustCSharp.Extensions.FormWithJsonBody.Extensions
{
    public static class MvcCoreBuilderExtensions
    {
        public static IMvcBuilder AddFormWithJsonBody(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));

            return mvcBuilder.AddFormWithJsonBody(null);
        }
        
        public static IMvcBuilder AddFormWithJsonBody(this IMvcBuilder mvcBuilder, Action<FormWithJsonBodyModelBinderOptions> setupAction)
        {
            mvcBuilder = mvcBuilder ?? throw new ArgumentNullException(nameof(mvcBuilder));

            mvcBuilder.AddFormWithJsonBodyCore(setupAction);
            
            mvcBuilder.Services.Configure<MvcOptions>(c =>
                c.AddFormWithJsonBodyCore());

            mvcBuilder.Services.TryAddSingleton<IFormFormatter, SystemTextJsonFormFormatter>();

            return mvcBuilder;
        }

        public static IMvcBuilder AddFormWithJsonBodyCore(this IMvcBuilder mvcBuilder,
            Action<FormWithJsonBodyModelBinderOptions> setupAction)
        {
            mvcBuilder.Services.TryAddEnumerable(
                ServiceDescriptor.Transient<IApiDescriptionProvider, FomWithJsonBodyApiDescriptionProvider>());
            
            if (setupAction != null)
            {
                mvcBuilder.Services.Configure<FormWithJsonBodyModelBinderOptions>(setupAction);
            }

            return mvcBuilder;
        }
        
        public static MvcOptions AddFormWithJsonBodyCore(
            this MvcOptions mvcOptions
            )
        {
            mvcOptions.ModelBinderProviders.Insert(0, new FormWithJsonBodyModelBinderProvider());

            return mvcOptions;
        }
    }
}