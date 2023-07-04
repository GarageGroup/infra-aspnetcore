using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GarageGroup.Infra;

[AttributeUsage(AttributeTargets.Parameter)]
public sealed class FromClaimAttribute : Attribute, IBindingSourceMetadata, IModelNameProvider
{
    public FromClaimAttribute(string type)
        =>
        Name = type ?? string.Empty;

    public string Name { get; }

    public BindingSource BindingSource => ClaimValueProviderFactory.ClaimBindingSource;
}
