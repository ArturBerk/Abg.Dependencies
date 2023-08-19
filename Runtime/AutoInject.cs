using System;

namespace Abg.Dependencies
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
    public sealed class AutoInject : Attribute
    {
        // public object Key { get; set; }
        //
        // public AutoInject(object key = null)
        // {
        //     Key = key;
        // }
    }
}