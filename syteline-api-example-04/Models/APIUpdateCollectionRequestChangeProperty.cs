using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace syteline_api_examples.Models;

public class APIUpdateCollectionRequestChangeProperty
{

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; } = null;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Value { get; set; } = null;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Modified { get; set; } = null;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsNull => this.Value == null && this.NestedCollection == null ? true : (bool?)null;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsNestedCollection => this.NestedCollection != null ? true : (bool?)null;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public APIUpdateCollectionRequest? NestedCollection { get; set; } = null;

    [SetsRequiredMembers]
    public APIUpdateCollectionRequestChangeProperty(string name, string value, bool modified = false)
    {
        this.Name = name;
        this.Value = value;
        this.Modified = modified;
    }

    [SetsRequiredMembers]
    public APIUpdateCollectionRequestChangeProperty(APIUpdateCollectionRequest? nestedCollection = null)
    {
        this.NestedCollection = nestedCollection;
    }

}
