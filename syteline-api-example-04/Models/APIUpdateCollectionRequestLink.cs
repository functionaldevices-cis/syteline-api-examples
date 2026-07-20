using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace syteline_api_examples.Models;

public class APIUpdateCollectionRequestLink
{
    public required string ParentProperty { get; set; }
    public required string ChildProperty { get; set; }

    [SetsRequiredMembers]
    public APIUpdateCollectionRequestLink(string parentProperty, string childProperty) {
        this.ParentProperty = parentProperty;
        this.ChildProperty = childProperty;
    }

}
