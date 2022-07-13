using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace GGroupp.Infra;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class ProducesSuccessAttribute : Attribute, IApiResponseMetadataProvider
{
    public ProducesSuccessAttribute(Type type, SuccessStatusCode statusCode)
        =>
        (Type, StatusCode) = (type, (int)statusCode);

    public ProducesSuccessAttribute(Type type)
        =>
        (Type, StatusCode) = (type, StatusCodes.Status200OK);

    public Type Type { get; }

    public int StatusCode { get; }

    public void SetContentTypes(MediaTypeCollection contentTypes)
        =>
        contentTypes.Add(new(MediaTypeNames.Application.Json));
}