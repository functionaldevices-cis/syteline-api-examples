using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;

namespace syteline_api_examples.Models;

public class APIUpdateRecordsRequestNestedCollection
{
    public required string IDOName { get; set; }

    public required List<APIUpdateRecordsRequestRecord> Records { get; set; }

    public List<APIUpdateCollectionRequestLink> Links { get; set; }

    [SetsRequiredMembers]
    public APIUpdateRecordsRequestNestedCollection(string idoName, List<APIUpdateCollectionRequestLink> links, List<APIUpdateRecordsRequestRecord> records)
    {
        this.IDOName = idoName;
        this.Records = records;
        this.Links = links;
    }

}
