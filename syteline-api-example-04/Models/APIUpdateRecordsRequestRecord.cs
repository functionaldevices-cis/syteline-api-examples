using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;

namespace syteline_api_examples.Models;

public class APIUpdateRecordsRequestRecord
{
    public required Dictionary<string, string> Properties { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<APIUpdateRecordsRequestNestedCollection>? NestedCollections { get; set; }

    [SetsRequiredMembers]
    public APIUpdateRecordsRequestRecord(Dictionary<string, string> properties, List<APIUpdateRecordsRequestNestedCollection>? nestedCollections = null)
    {
        this.Properties = properties;
        this.NestedCollections = nestedCollections;
    }

}
