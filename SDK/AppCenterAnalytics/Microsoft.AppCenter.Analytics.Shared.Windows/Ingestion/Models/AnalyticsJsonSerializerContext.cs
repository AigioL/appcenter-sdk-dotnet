// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#if USE_SYS_JSON
global using JsonProperty = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using System.Text.Json.Serialization;

namespace Newtonsoft.Json
{
}

namespace Microsoft.AppCenter.Analytics.Ingestion.Models
{
    [JsonSerializable(typeof(EventLog))]
    [JsonSerializable(typeof(PageLog))]
    [JsonSerializable(typeof(StartSessionLog))]
    [JsonSourceGenerationOptions]
    public sealed partial class AnalyticsJsonSerializerContext : JsonSerializerContext
    {
        static AnalyticsJsonSerializerContext()
        {
            var o = LogSerializer.GetDefaultJsonSerializerOptions();
            Default = new AnalyticsJsonSerializerContext(o);
        }
    }
}
#endif