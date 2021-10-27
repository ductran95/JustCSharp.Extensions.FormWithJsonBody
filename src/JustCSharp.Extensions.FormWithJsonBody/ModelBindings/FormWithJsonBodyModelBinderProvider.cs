using System;
using JustCSharp.Extensions.FormWithJsonBody.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JustCSharp.Extensions.FormWithJsonBody.ModelBindings
{
    public class FormWithJsonBodyModelBinderProvider: IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.BindingInfo.BindingSource != null &&
                context.BindingInfo.BindingSource.CanAcceptDataFrom(CustomBindingSource.FormWithJsonBody))
            {
                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                var mvcOptions = context.Services.GetRequiredService<IOptions<MvcOptions>>().Value;
                var formFormatter = context.Services.GetRequiredService<IFormFormatter>();
                var options = context.Services.GetRequiredService<IOptions<FormWithJsonBodyModelBinderOptions>>().Value;
                
                return new FormWithJsonBodyModelBinder(options, mvcOptions, formFormatter, loggerFactory);
            }

            return null;
        }
    }
}