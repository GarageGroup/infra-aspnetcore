using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GGroupp.Infra;

public sealed class ClaimValueProviderFactory : IValueProviderFactory
{
    internal static readonly BindingSource ClaimBindingSource
        =
        new(id: "Claim", displayName: "BindingSource_Claim", isGreedy: false, isFromRequest: false);

    public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
    {
        var claimValueProvider = new ClaimValueProvider(ClaimBindingSource, context.ActionContext.HttpContext.User);
        context.ValueProviders.Add(claimValueProvider);

        return Task.CompletedTask;
    }
}