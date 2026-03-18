// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.

using Microsoft.AppCenter.Ingestion.Models;
using Newtonsoft.Json;

namespace Microsoft.AppCenter.Crashes.Ingestion.Models
{
    /// <summary>
    /// Binary (library) definition for any platform.
    /// </summary>
    public partial class Binary
    {
        /// <summary>
        /// Initializes a new instance of the Binary class.
        /// </summary>
#if USE_SYS_JSON
        [System.Text.Json.Serialization.JsonConstructor]
#endif
        public Binary()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Binary class.
        /// </summary>
        /// <param name="primaryArchitectureId">CPU primary
        /// architecture.</param>
        /// <param name="architectureVariantId">CPU architecture
        /// variant.</param>
        public Binary(string id, string startAddress, string endAddress, string name, string path, string architecture = default(string), long? primaryArchitectureId = default(long?), long? architectureVariantId = default(long?))
        {
            Id = id;
            StartAddress = startAddress;
            EndAddress = endAddress;
            Name = name;
            Path = path;
            Architecture = architecture;
            PrimaryArchitectureId = primaryArchitectureId;
            ArchitectureVariantId = architectureVariantId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("startAddress")]
        public string StartAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("endAddress")]
        public string EndAddress { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty("architecture")]
        public string Architecture { get; set; }

        /// <summary>
        /// Gets or sets CPU primary architecture.
        /// </summary>
        [JsonProperty("primaryArchitectureId")]
        public long? PrimaryArchitectureId { get; set; }

        /// <summary>
        /// Gets or sets CPU architecture variant.
        /// </summary>
        [JsonProperty("architectureVariantId")]
        public long? ArchitectureVariantId { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Id == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "Id");
            }
            if (StartAddress == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "StartAddress");
            }
            if (EndAddress == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "EndAddress");
            }
            if (Name == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "Name");
            }
            if (Path == null)
            {
                throw new ValidationException(ValidationException.Rule.CannotBeNull, "Path");
            }
        }
    }
}
