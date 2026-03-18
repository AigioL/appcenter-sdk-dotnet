// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.AppCenter.Utils
{
    /// <summary>
    /// Represents a store of persistent application settings that is behaves like a dictionary.
    /// </summary>
    public interface IApplicationSettings
    {
#if NET5_0_OR_GREATER
        protected const DynamicallyAccessedMemberTypes AllMembersAndInterfaces =
#if NET10_0_OR_GREATER
            DynamicallyAccessedMemberTypes.AllConstructors |
            DynamicallyAccessedMemberTypes.AllEvents |
            DynamicallyAccessedMemberTypes.AllFields |
            DynamicallyAccessedMemberTypes.AllMethods |
            DynamicallyAccessedMemberTypes.AllNestedTypes |
            DynamicallyAccessedMemberTypes.AllProperties |
            DynamicallyAccessedMemberTypes.Interfaces;
#else
            DynamicallyAccessedMemberTypes.All;
#endif
#endif

        // Returns the object corresponding to 'key'. If there is no such object, it creates one with the given default value, and returns that
        T GetValue<
#if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(AllMembersAndInterfaces)]
#endif
        T>(string key, T defaultValue = default(T));
        void SetValue<
#if NET6_0_OR_GREATER
            [DynamicallyAccessedMembers(AllMembersAndInterfaces)]
#endif
        T>(string key, T value);
        bool ContainsKey(string key);
        void Remove(string key);
    }
}
