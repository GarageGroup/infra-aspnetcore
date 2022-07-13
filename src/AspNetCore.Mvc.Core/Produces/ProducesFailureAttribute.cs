using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class ProducesFailureAttribute : Attribute, IApiResponseMetadataProvider
{
    public ProducesFailureAttribute(FailureStatusCode statusCode)
        =>
        StatusCode = statusCode.GetFailureStatusCode();

    public Type Type => typeof(ProblemDetails);

    public int StatusCode { get; }

    public void SetContentTypes(MediaTypeCollection contentTypes)
        =>
        contentTypes.Add(new("application/problem+json"));
}