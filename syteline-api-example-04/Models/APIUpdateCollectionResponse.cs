using System;
using System.Collections.Generic;

namespace syteline_api_examples.Models;

public class APIUpdateCollectionResponse : APIMethodResponse
{

    public List<Dictionary<string, object?>> RefreshItems
    {
        get; set;
    }

    public string Bookmark
    {
        get; set;
    }

    public APIUpdateCollectionResponse(bool success, string message = "", List<Dictionary<string, object?>>? refreshItems = null, string bookmark = "", string? batchStartTimestamp = "")
    {
        this.RefreshItems = refreshItems ?? [];
        this.Bookmark = bookmark;
        this.Success = success;
        this.Message = message;
        this.BatchStartTimestamp = batchStartTimestamp ?? "";
    }

}
