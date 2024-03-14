using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Infra;

internal sealed class SwaggerBuilder : ISwaggerBuilder
{
    private readonly ICollection<Action<OpenApiDocument>> configurators;

    internal SwaggerBuilder()
        =>
        configurators = [];

    public ISwaggerBuilder Use(Action<OpenApiDocument> configurator)
    {
        ArgumentNullException.ThrowIfNull(configurator);

        configurators.Add(configurator);
        return this;
    }

    public OpenApiDocument Build()
    {
        var document = new OpenApiDocument();

        foreach (var configurator in configurators.Reverse())
        {
            configurator.Invoke(document);
        }

        return document;
    }
}