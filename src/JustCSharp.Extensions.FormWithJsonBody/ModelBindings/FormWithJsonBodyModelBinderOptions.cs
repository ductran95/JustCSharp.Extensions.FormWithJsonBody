using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JustCSharp.FormWithJsonBody.ModelBindings
{
    public class FormWithJsonBodyModelBinderOptions
    {
        public string BodyName { get; set; } = "Body";
    }
}