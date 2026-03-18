// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Runtime.Serialization;

namespace Microsoft.AppCenter.Crashes
{
    [Serializable]
    public partial class TestCrashException
    {
        /// <summary>
        /// Deserialization constructor. Not intended for public use.
        /// </summary>
#pragma warning disable SYSLIB0051
        protected TestCrashException(SerializationInfo info, StreamingContext context) : base(info, context)
#pragma warning restore SYSLIB0051
        {
        }
    }
}
