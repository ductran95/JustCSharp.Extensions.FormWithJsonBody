using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JustCSharp.FormWithJsonBody.ModelBindings
{
    public class CustomBindingSource: BindingSource
    {
        public static readonly BindingSource FormWithJsonBody = new CustomBindingSource(
            "FormWithJsonBody",
            "FormWithJsonBody",
            isGreedy: true,
            isFromRequest: true);

        public CustomBindingSource(string id, string displayName, bool isGreedy, bool isFromRequest) : base(id, displayName, isGreedy, isFromRequest)
        {
        }

        public override bool CanAcceptDataFrom(BindingSource bindingSource)
        {
            if (bindingSource == FormWithJsonBody)
            {
                return true;
            }
            
            return base.CanAcceptDataFrom(bindingSource);
        }
    }
}