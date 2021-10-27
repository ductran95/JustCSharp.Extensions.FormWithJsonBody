using System;
using JustCSharp.FormWithJsonBody.ApiExplorer;
using JustCSharp.FormWithJsonBody.Formatters;
using JustCSharp.FormWithJsonBody.ModelBindings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
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