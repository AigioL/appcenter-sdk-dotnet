// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

#if USE_SYS_JSON
global using JsonProperty = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using Microsoft.AppCenter.Ingestion.Models.Serialization;
using System.Text.Json.Serialization;

namespace Newtonsoft.Json
{
}

namespace Microsoft.AppCenter.Crashes.Ingestion.Models
{
    [JsonSerializable(typeof(AbstractErrorLog))]
    [JsonSerializable(typeof(Binary))]
    [JsonSerializable(typeof(global::Microsoft.AppCenter.Crashes.Ingestion.Models.Exception))]
    [JsonSerializable(typeof(HandledErrorLog))]
    [JsonSerializable(typeof(ManagedErrorLog))]
    [JsonSerializable(typeof(StackFrame))]
    [JsonSerializable(typeof(ErrorAttachmentLog))]
    [JsonSourceGenerationOptions]
    public sealed partial class CrashesJsonSerializerContext : JsonSerializerContext
    {
        static CrashesJsonSerializerContext()
        {
            var o = LogSerializer.GetDefaultJsonSerializerOptions();
            Default = new CrashesJsonSerializerContext(o);
        }
    }
}
#endif