using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace GameStore.PL.Attributes
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Parameter)]
    public class DecodeFromRouteAttribute : Attribute, IBindingSourceMetadata
    {
        public DecodeFromRouteAttribute() { }

        public BindingSource BindingSource { get; }
    }
}
