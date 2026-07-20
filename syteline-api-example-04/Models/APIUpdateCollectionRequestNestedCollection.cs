using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace syteline_api_examples.Models;

public class APIUpdateCollectionRequestNestedCollection
{

    public required string IDOName { get; set; }

    public required bool RefreshAfterSave { get; set; }

    public required List<APIUpdateCollectionRequestChange> Changes { get; set; }
            
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<APIUpdateCollectionRequestLink> Links { get; set; }

    [SetsRequiredMembers]
    public APIUpdateCollectionRequestNestedCollection(string idoName, List<APIUpdateCollectionRequestLink> links, List<APIUpdateCollectionRequestChange> changes, bool refreshAfterSave = false)
    {
        this.IDOName = idoName;
        this.RefreshAfterSave = refreshAfterSave;
        this.Changes = changes;
        this.Links = links;
    }

}
