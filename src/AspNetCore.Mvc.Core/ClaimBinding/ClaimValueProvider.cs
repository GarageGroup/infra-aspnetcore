using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GGroupp.Infra;

internal sealed class ClaimValueProvider : BindingSourceValueProvider
{
    private readonly ClaimsPrincipal claimsPrincipal;

    internal ClaimValueProvider(BindingSource bindingSource, ClaimsPrincipal claimsPrincipal) : base(bindingSource)
        =>
        this.claimsPrincipal = claimsPrincipal;

    public override bool ContainsPrefix(string prefix)
        =>
        claimsPrincipal.HasClaim(claim => string.Equals(claim.Type, prefix, StringComparison.InvariantCulture));

    public override ValueProviderResult GetValue(string key)
        =>
        claimsPrincipal.FindFirst(key)?.Value switch
        {
            string claimValue => new(claimValue),
            _ => ValueProviderResult.None
        };
}