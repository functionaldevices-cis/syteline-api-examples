using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using syteline_api_examples.Models;

namespace syteline_api_examples.Helpers;

public class SytelineAPI
{
    private int RequestCap
    {
        get; init;
    }

    private HttpClient HttpClient
    {
        get; init;
    }

    public SytelineConnection SytelineConnection
    {
        get; set;
    }

    private APIAccessTokenDetails AccessTokenDetails {
        get; set;
    } = new();

    public SytelineAPI(SytelineConnection connection, int requestCap = 200)
    {

        this.SytelineConnection = connection;

        // INIT SETTINGS

        this.RequestCap = requestCap;

        // INIT HTTP CLIENT

        this.HttpClient = new();
        this.HttpClient.DefaultRequestHeaders.Add("X-Infor-MongooseConfig", "");

    }

    public APIAccessTokenDetails GetAccessToken()
    {

        // CHECK TO SEE IF WE NEED A NEW TOKEN

        if ((this.AccessTokenDetails.Token == "") || ((this.AccessTokenDetails.Expiration != null && this.AccessTokenDetails.Expiration >= DateTime.Now.AddMinutes(10))))
        {

            // TRY TO GET THE TOKEN

            try
            {

                if (this.SytelineConnection.APIType == "Direct")
                {
                    // LOAD THE REQUEST

                    HttpResponseMessage httpResponse = this.HttpClient.SendAsync(new HttpRequestMessage()
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri($"{this.SytelineConnection.BaseURL}/IDORequestService/ido/token/{this.SytelineConnection.Config}"),
                        Headers =
                        {
                            { "username", this.SytelineConnection.CredentialsDirect.Username },
                            { "password", this.SytelineConnection.CredentialsDirect.Password }
                        }
                    }).Result;

                    Dictionary<string, object> parsedResponseContent = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(httpResponse.Content.ReadAsStringAsync().Result) ?? throw new Exception("Unable to parse response.");

                    this.AccessTokenDetails.Token = (parsedResponseContent["Token"] ?? "").ToString() ?? "";
                    this.AccessTokenDetails.Expiration = DateTime.Now.AddSeconds((7200));
                    this.AccessTokenDetails.Valid = (parsedResponseContent["Success"].ToString() == "True");

                    this.AccessTokenDetails.Message = this.AccessTokenDetails.Valid ? "Successfully connected and authenticated." : "Unable to load access token. Please check credentials.";

                }
                else
                {


                    HttpResponseMessage httpResponse = this.HttpClient.SendAsync(new HttpRequestMessage()
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(this.SytelineConnection.CredentialsION.obtain_token_endpoint),
                        Content = new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            { "client_id", this.SytelineConnection.CredentialsION.client_id },
                            { "client_secret", this.SytelineConnection.CredentialsION.client_secret },
                            { "grant_type", "password" },
                            { "username", this.SytelineConnection.CredentialsION.service_account_access_key },
                            { "password", this.SytelineConnection.CredentialsION.service_account_secret_key }
                        })
                    }).Result;

                    if (httpResponse.IsSuccessStatusCode)
                    {

                        Dictionary<string, object> parsedResponseContent = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(httpResponse.Content.ReadAsStringAsync().Result) ?? throw new Exception("Unable to parse response.");

                        this.AccessTokenDetails.Token = "Bearer " + (parsedResponseContent["access_token"] ?? "").ToString() ?? "";
                        this.AccessTokenDetails.Expiration = DateTime.Now.AddSeconds(int.Parse(parsedResponseContent["expires_in"].ToString() ?? "7200"));
                        this.AccessTokenDetails.Valid = true;

                        this.AccessTokenDetails.Message = this.AccessTokenDetails.Valid ? "Successfully connected and authenticated." : "Unable to load access token. Please check credentials.";

                    }

                }
            }
            catch (Exception ex)
            {
                this.AccessTokenDetails.Token = "";
                this.AccessTokenDetails.Expiration = null;
                this.AccessTokenDetails.Valid = false;
                this.AccessTokenDetails.Message = ex.Message;

            }

        }

        return this.AccessTokenDetails;

    }

    public APILoadCollectionResponse LoadCollection(string idoName, List<string> properties, string? filter = null, List<OrderByProperty>? orderBy = null, int? recordCap = null, bool? distinct = null, string? clm = null, List<string>? clmParam = null, string? pqc = null, bool? readOnly = null, string paginationMode = "bookmark", string paginationProperty = "RowPointer")
    {

        // PARSE INPUT PARAMS

        orderBy ??= [];
        string? orderByString = orderBy == null ? null : string.Join(", ", (orderBy ?? []).Select(property => property.OrderBy));

        // INIT GENERAL VARS

        APILoadCollectionResponse parsedResponseContent;
        List<Dictionary<string, object?>> data = [];

        // INIT PAGINATION VARS

        int requestCap = recordCap != null ? Math.Min((int)recordCap, this.RequestCap) : this.RequestCap;
        bool haveToPaginate = recordCap == null || recordCap >= this.RequestCap;
        bool isFirstQuery = true;
        bool moreRowsExist;
        bool totalCapNotMet = true;
        string? bookmark = null;

        string? batchFilter;
        string? batchOrderByString;

        string paginationPropertyHighestValue = "";

        // IF WE ARE PAGINATING WITH PROPERTY-BASED PAGINATION, WE HAVE TO ADD THE ORDERBY AND PAGING PROPERTIES TO THE QUERY

        List<string> propsAddedForPagination = paginationMode == "propertyBased" ? (new List<string>() { paginationProperty }).Concat(orderBy.Select(property => property.PropertyName)).Distinct().Where(property => !properties.Contains(property)).ToList() : [];

        do
        {

            // SET UP BATCH VARIABLE DEFAULTS

            batchOrderByString = orderByString;
            batchFilter = filter;

            // INSERT PAGINATION FILTERS AND ORDERBY

            if (haveToPaginate && paginationMode == "propertyBased")
            {

                batchOrderByString = paginationProperty + " ASC";

                if (!isFirstQuery)
                {

                    isFirstQuery = false;

                    string paginationFilter = $"( {paginationProperty} > '{paginationPropertyHighestValue}' )";

                    batchFilter = $"{((filter ?? "") != "" ? $"( {filter} ) AND " : "")}{paginationFilter}";

                }

                isFirstQuery = false;

            }

            parsedResponseContent = LoadCollectionBatch(
                idoName: idoName,
                properties: properties,
                filter: batchFilter,
                orderBy: batchOrderByString,
                requestCap: requestCap,
                distinct: distinct,
                clm: clm,
                clmParam: clmParam,
                bookmark: bookmark,
                pqc: pqc,
                readOnly: readOnly
            );

            if (parsedResponseContent.Success == false)
            {
                break;
            }

            // UPDATE LOOP VARS

            if (paginationMode == "bookmark")
            {
                bookmark = parsedResponseContent.Bookmark;
                moreRowsExist = parsedResponseContent.MoreRowsExist;
            }
            else
            {
                moreRowsExist = parsedResponseContent.Items.Count == requestCap;
                if (moreRowsExist)
                {
                    if (parsedResponseContent.Items.Count > 0)
                    {
                        Dictionary<string, object?> lastItem = parsedResponseContent.Items.Last();
                        if (lastItem.Keys.Contains(paginationProperty))
                        {
                            paginationPropertyHighestValue = lastItem[paginationProperty]!.ToString() ?? "";
                        }
                    }

                }
            }

            data = data.Concat(parsedResponseContent.Items).ToList();

            if (recordCap != null)
            {
                if (recordCap <= data.Count)
                {
                    totalCapNotMet = false;
                }
            }

        } while (moreRowsExist && totalCapNotMet);

        parsedResponseContent.Items = data;
        parsedResponseContent.Config = this.SytelineConnection.Config;

        return parsedResponseContent;

    }

    public APIUpdateCollectionResponse UpdateCollection(string idoName, List<APIUpdateCollectionRequestChange> changes, bool refreshAfterSave = false)
    {

        // BUILD THE REQUEST

        string requestURL = this.SytelineConnection.BaseURL + "/IDORequestService/ido/update/" + idoName + "?" + BuildUpdateCollectionParametersString(
            refresh: refreshAfterSave
        );

        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(requestURL),
            Headers =
            {
                { "Accept", "application/json" },
                { "X-Infor-MongooseConfig", this.SytelineConnection.Config }
            },
            Content = new StringContent(
                content: System.Text.Json.JsonSerializer.Serialize(
                    new Dictionary<string, List<APIUpdateCollectionRequestChange>>()
                    {
                        ["Changes"] = changes
                    }
                ),
                encoding: Encoding.UTF8,
                mediaType: "application/json"
            )
        };
        request.Headers.TryAddWithoutValidation("Authorization", this.GetAccessToken().Token);

        // LOAD THE REQUEST

        HttpResponseMessage httpResponse = this.HttpClient.SendAsync(request).Result;

        // PARSE THE REQUEST

        APIUpdateCollectionResponse parsedResponse = System.Text.Json.JsonSerializer.Deserialize<APIUpdateCollectionResponse>(httpResponse.Content.ReadAsStringAsync().Result) ?? throw new Exception("Unable to parse response.");

        return parsedResponse;

    }
    
    private APILoadCollectionResponse LoadCollectionBatch(string idoName, List<string> properties, string? filter, string? orderBy = null, int? requestCap = 0, bool? distinct = null, string? clm = null, List<string>? clmParam = null, string? bookmark = null, string? pqc = null, bool? readOnly = null)
    {

        // BUILD THE REQUEST

        string requestURL = this.SytelineConnection.BaseURL + "/IDORequestService/ido/load/" + idoName + "?" +  BuildLoadCollectionParametersString(
            properties: properties,
            filter: filter,
            orderBy: orderBy,
            recordCap: requestCap,
            distinct: distinct,
            clm: clm,
            clmParam: clmParam,
            loadType: "NEXT",
            bookmark: bookmark,
            pqc: pqc,
            readOnly: readOnly
        );

        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(requestURL),
            Headers =
            {
                { "Accept", "application/json" },
                { "X-Infor-MongooseConfig", this.SytelineConnection.Config }
            }
        };
        request.Headers.TryAddWithoutValidation("Authorization", this.GetAccessToken().Token);

        // LOAD THE REQUEST

        HttpResponseMessage httpResponse = this.HttpClient.SendAsync(request).Result;

        // PARSE THE REQUEST

        APILoadCollectionResponse parsedResponse = System.Text.Json.JsonSerializer.Deserialize<APILoadCollectionResponse>(httpResponse.Content.ReadAsStringAsync().Result) ?? throw new Exception("Unable to parse response.");

        return parsedResponse;

    }

    private static string BuildLoadCollectionParametersString(List<string> properties, string? filter = null, string? orderBy = null, int? recordCap = null, bool? distinct = null, string? clm = null, List<string>? clmParam = null, string? loadType = null, string? bookmark = null, string? pqc = null, bool? readOnly = null)
    {

        // CREATE LIST OF PARAMTERS TO STRINGY

        List<string> lQueryPrameters = [
            "properties=" + string.Join(",", properties)
        ];

        if (filter != null && filter != "")
        {
            lQueryPrameters.Add("filter=" +  EncodeValue(filter));
        }

        if (orderBy != null && orderBy != "")
        {
            lQueryPrameters.Add("orderBy=" +  EncodeValue(orderBy));
        }

        if (recordCap != null)
        {
            lQueryPrameters.Add("recordCap=" + EncodeValue(recordCap));
        }

        if (distinct != null)
        {
            lQueryPrameters.Add("distinct=" + EncodeValue(distinct));
        }

        if (clm != null && clm != "")
        {
            lQueryPrameters.Add("clm=" + clm);
        }

        if (clm != null && clm != "" && clmParam != null && clmParam.Count > 0)
        {
            lQueryPrameters.Add("clmParam=" + EncodeValue(string.Join(",", clmParam)));
        }

        if (loadType != null && loadType != "")
        {
            lQueryPrameters.Add("loadType=" + EncodeValue(loadType));
        }

        if (bookmark != null && bookmark != "")
        {
            lQueryPrameters.Add("bookmark=" + EncodeValue(bookmark));
        }

        if (pqc != null && pqc != "")
        {
            lQueryPrameters.Add("pqc=" + EncodeValue(pqc));
        }

        if (readOnly != null)
        {
            lQueryPrameters.Add("readOnly=" + EncodeValue(readOnly));
        }

        // BUILD THE REQUEST URL

        return string.Join("&", lQueryPrameters);

    }

    private static string BuildUpdateCollectionParametersString(bool? refresh = null)
    {

        // CREATE LIST OF PARAMTERS TO STRINGY

        List<string> lQueryPrameters = [];

        if (refresh != null)
        {
            lQueryPrameters.Add("refresh=" + EncodeValue(refresh));
        }

        // BUILD THE REQUEST URL

        return string.Join("&", lQueryPrameters);

    }

    private static string EncodeValue(object value)
    {

        return Uri.EscapeDataString(ConvertToString(value));

    }

    private static string ConvertToString(object rawValue)
    {

        System.Globalization.CultureInfo cultureInfo = System.Globalization.CultureInfo.InvariantCulture;
        if (rawValue is Enum)
        {
            string? name = Enum.GetName(rawValue.GetType(), rawValue);
            if (name != null)
            {
                System.Reflection.FieldInfo? field = System.Reflection.IntrospectionExtensions.GetTypeInfo(rawValue.GetType()).GetDeclaredField(name);
                if (field != null)
                {
                    System.Runtime.Serialization.EnumMemberAttribute? attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) as System.Runtime.Serialization.EnumMemberAttribute;
                    if (attribute != null)
                    {
                        return attribute.Value ?? name;
                    }
                }
            }
        }
        else if (rawValue is bool)
        {
            return (Convert.ToString(rawValue, cultureInfo) ?? "true").ToLowerInvariant();
        }
        else if (rawValue is byte[])
        {
            return Convert.ToBase64String((byte[])rawValue);
        }
        else if (rawValue != null && (rawValue.GetType().IsArray))
        {
            var array = Enumerable.OfType<object>((Array)rawValue);
            return string.Join(",", Enumerable.Select(array, o => ConvertToString(o)));
        }

        return Convert.ToString(rawValue, cultureInfo) ?? "";

    }

}
