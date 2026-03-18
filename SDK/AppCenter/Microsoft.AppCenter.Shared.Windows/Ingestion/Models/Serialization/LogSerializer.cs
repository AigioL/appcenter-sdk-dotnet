// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Newtonsoft.Json;
using System.Collections.Generic;
#if USE_SYS_JSON
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.Encodings.Web;
#endif

namespace Microsoft.AppCenter.Ingestion.Models.Serialization
{
#if USE_SYS_JSON
    [JsonSerializable(typeof(LogContainer))]
    [JsonSourceGenerationOptions]
    public partial class LogSerializer : JsonSerializerContext
    {
        static LogSerializer()
        {
            var o = GetDefaultJsonSerializerOptions();
            Default = new LogSerializer(o);
            SerializationSettings = GetSerializationSettings();
        }

        public static JsonSerializerOptions GetDefaultJsonSerializerOptions()
        {
            JsonSerializerOptions o = new()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowReadingFromString,

                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            o.Converters.Add(DateTimeIsoDateFormatJsonConverter.Instance);
            return o;
        }

        static JsonSerializerOptions GetSerializationSettings()
        {
            var o = GetDefaultJsonSerializerOptions();
            o.TypeInfoResolverChain.Insert(0, LogJsonTypeInfoResolver.Instance);
            return o;
        }

        public static JsonSerializerOptions SerializationSettings { get; }

        public static void AddLogType(string typeName, Type type)
        {
            var t = new JsonDerivedType(type, typeName);
            LogJsonTypeInfoResolver.Instance.Options.DerivedTypes.Add(t);
        }

        public static void AddLogType(IJsonTypeInfoResolver jsonTypeInfoResolver)
        {
            LogJsonTypeInfoResolver.TypeInfoResolverChain.Add(jsonTypeInfoResolver);
        }
#else
    public static class LogSerializer
    {
        internal static readonly JsonSerializerSettings SerializationSettings;
        private static readonly LogJsonConverter Converter = new LogJsonConverter();

        static LogSerializer()
        {
            SerializationSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                Converters = { Converter }
            };
        }

        public static void AddLogType(string typeName, Type type)
        {
            Converter.AddLogType(typeName, type);
        }
#endif

        public static string Serialize(LogContainer logContainer)
        {
#if USE_SYS_JSON
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            return JsonSerializer.Serialize(logContainer, SerializationSettings);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#else
            return JsonConvert.SerializeObject(logContainer, SerializationSettings);
#endif
        }

        public static string Serialize(Log log)
        {
#if USE_SYS_JSON
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            return JsonSerializer.Serialize(log, SerializationSettings);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#else
            return JsonConvert.SerializeObject(log, SerializationSettings);
#endif
        }

        public static Log DeserializeLog(string json)
        {
#if USE_SYS_JSON
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
            return JsonSerializer.Deserialize<Log>(json, SerializationSettings);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#else
            return JsonConvert.DeserializeObject<Log>(json, SerializationSettings);
#endif
        }
    }

#if USE_SYS_JSON
#nullable enable
    file sealed class DateTimeIsoDateFormatJsonConverter : JsonConverter<DateTime>
    {
        internal static readonly DateTimeIsoDateFormatJsonConverter Instance = new();
        public sealed override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TryGetDateTime(out var dt))
            {
                return dt;
            }
            var str = reader.GetString();
            if (string.IsNullOrWhiteSpace(str))
            {
                return default;
            }
            dt = DateTime.ParseExact(str, "O", System.Globalization.CultureInfo.InvariantCulture);
            return dt;
        }

        public sealed override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var str = value.ToUniversalTime().ToString("O", System.Globalization.CultureInfo.InvariantCulture);
            writer.WriteStringValue(str);
        }
    }

    file sealed class LogJsonTypeInfoResolver : IJsonTypeInfoResolver
    {
        internal static readonly LogJsonTypeInfoResolver Instance = new();

        internal static readonly List<IJsonTypeInfoResolver> TypeInfoResolverChain = [LogSerializer.Default];

        internal readonly JsonPolymorphismOptions Options = new()
        {
            TypeDiscriminatorPropertyName = LogJsonConverter.TypeIdKey,
            IgnoreUnrecognizedTypeDiscriminators = true,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
            DerivedTypes =
            {
                new JsonDerivedType(typeof(StartServiceLog), StartServiceLog.JsonIdentifier),
            }
        };

        JsonTypeInfo? IJsonTypeInfoResolver.GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            if (type == typeof(Log))
            {
                var jsonTypeInfo = LogSerializer.Default.Log;
                jsonTypeInfo.PolymorphismOptions = Options;
                return jsonTypeInfo;
            }
            else
            {
                foreach (var jsonTypeInfoResolver in TypeInfoResolverChain)
                {
                    var jsonTypeInfo = jsonTypeInfoResolver.GetTypeInfo(type, options);
                    if (jsonTypeInfo != null)
                    {
                        return jsonTypeInfo;
                    }
                }
            }
            return null;
        }
    }
#endif
}
