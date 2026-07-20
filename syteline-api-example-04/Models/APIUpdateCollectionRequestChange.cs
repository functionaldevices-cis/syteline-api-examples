using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace syteline_api_examples.Models;

public class APIUpdateCollectionRequestChange
{

    public required APIUpdateCollectionRequestChangeAction Action { get; set; }

    public required List<APIUpdateCollectionRequestChangeProperty> Properties { get; set; } = [];

    [SetsRequiredMembers]
    public APIUpdateCollectionRequestChange(APIUpdateCollectionRequestChangeAction action, List<APIUpdateCollectionRequestChangeProperty> properties)
    {
        this.Action = action;
        this.Properties = properties;
    }

}

public enum APIUpdateCollectionRequestChangeAction
{
    Insert = 1,
    Update = 2
}