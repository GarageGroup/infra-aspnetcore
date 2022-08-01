using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;

namespace GGroupp.Infra;

internal sealed class SwaggerBuilder : ISwaggerBuilder
{
    private readonly Stack<Action<OpenApiDocument>> configurators;

    internal SwaggerBuilder()
        =>
        configurators = new();

    public ISwaggerBuilder Use(Action<OpenApiDocument> configurator!!)
    {
        configurators.Push(configurator);
        return this;
    }

    public OpenApiDocument Build()
    {
        var document = new OpenApiDocument();

        while (configurators.TryPop(out var configurator))
        {
            configurator.Invoke(document);
        }

        return document;
    }
}