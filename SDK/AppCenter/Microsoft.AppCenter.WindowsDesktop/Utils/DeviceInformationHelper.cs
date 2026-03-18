// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
#if USE_WMI_LIGHT
using WmiLight;
#else
using System.Management;
#endif
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;

namespace Microsoft.AppCenter.Utils
{

    /// <summary>
    /// Implements the abstract device information helper class
    /// </summary>
    public class DeviceInformationHelper : AbstractDeviceInformationHelper
    {
#if !USE_WMI_LIGHT
        private IManagmentClassFactory _managmentClassFactory;
#endif
        private const string _defaultVersion = "Unknown";

        public DeviceInformationHelper()
        {
#if !USE_WMI_LIGHT
            _managmentClassFactory = ManagmentClassFactory.Instance;
#endif
        }

#if !USE_WMI_LIGHT
        /// <summary>
        /// Set the specific class factory for the management class.
        /// </summary>
        /// <param name="factory">Specific management class factory.</param>
        internal void SetManagmentClassFactory(IManagmentClassFactory factory)
        {
            _managmentClassFactory = factory;
        }
#endif

        protected override string GetSdkName()
        {
            var sdkName = WindowsHelper.IsRunningAsWpf ? "appcenter.wpf" : "appcenter.winforms";
#if WINDOWS
            sdkName = WindowsHelper.IsRunningAsWinUI ? "appcenter.winui" : $"{sdkName}.net";
#elif NETCOREAPP
            sdkName = $"{sdkName}.netcore";
#endif
            return sdkName;
        }

        protected override string GetDeviceModel()
        {
            try
            {
#if USE_WMI_LIGHT
                using WmiConnection con = new();
                var q = con.CreateQuery("SELECT Model FROM Win32_ComputerSystem");
                foreach (var it in q)
                {
                    var model = it.TryGetValue("Model")?.ToString();
                    return string.IsNullOrEmpty(model) || DefaultSystemProductName == model ? null : model;
                }
#else
                var managementClass = _managmentClassFactory.GetComputerSystemClass();
                foreach (var managementObject in managementClass.GetInstances())
                {
                    var model = (string)managementObject["Model"];
                    return string.IsNullOrEmpty(model) || DefaultSystemProductName == model ? null : model;
                }
#endif
            }
            catch (UnauthorizedAccessException exception)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device model with error: ", exception);
                return string.Empty;
            }
            catch (COMException exception)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device model. Make sure that WMI service is enabled.", exception);
                return string.Empty;
            }
            catch (ManagementException exception)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device model. Make sure that WMI service is enabled.", exception);
                return string.Empty;
            }
            catch (PlatformNotSupportedException exception)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device model. Make sure that .NET Framework is up to date.", exception);
                return string.Empty;
            }
            return string.Empty;
        }

        protected override string GetAppNamespace()
        {
            return Assembly.GetEntryAssembly()?.EntryPoint.DeclaringType?.Namespace;
        }

        protected override string GetDeviceOemName()
        {
            try
            {
#if USE_WMI_LIGHT
                using WmiConnection con = new();
                var q = con.CreateQuery("SELECT Manufacturer FROM Win32_ComputerSystem");
                foreach (var it in q)
                {
                    var manufacturer = it.TryGetValue("Manufacturer")?.ToString();
                    return string.IsNullOrEmpty(manufacturer) || DefaultSystemManufacturer == manufacturer ? null : manufacturer;
                }
#else
                var managementClass = _managmentClassFactory.GetComputerSystemClass();
                foreach (var managementObject in managementClass.GetInstances())
                {
                    var manufacturer = (string)managementObject["Manufacturer"];
                    return string.IsNullOrEmpty(manufacturer) || DefaultSystemManufacturer == manufacturer ? null : manufacturer;
                }
#endif
            }
            catch (UnauthorizedAccessException exception)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OEM name with error: ", exception);
                return string.Empty;
            }
            catch (COMException exception)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OEM name. Make sure that WMI service is enabled.", exception);
                return string.Empty;
            }
            catch (ManagementException exception)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OEM name. Make sure that WMI service is enabled.", exception);
                return string.Empty;
            }
            catch (PlatformNotSupportedException exception)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OEM name. Make sure that .NET Framework is up to date.", exception);
                return string.Empty;
            }
            return string.Empty;
        }

        protected override string GetOsName()
        {
            return "WINDOWS";
        }

        protected override string GetOsBuild()
        {
            using (var hklmKey = Win32.Registry.LocalMachine)
            using (var subKey = hklmKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                // CurrentMajorVersionNumber present in registry starting with Windows 10
                var majorVersion = subKey.GetValue("CurrentMajorVersionNumber");
                if (majorVersion != null)
                {
                    var minorVersion = subKey.GetValue("CurrentMinorVersionNumber", "0");
                    var buildNumber = subKey.GetValue("CurrentBuildNumber", "0");
                    var revisionNumber = subKey.GetValue("UBR", "0");
                    return $"{majorVersion}.{minorVersion}.{buildNumber}.{revisionNumber}";
                }
                else
                {
                    // If CurrentMajorVersionNumber not present in registry then use CurrentVersion
                    var version = subKey.GetValue("CurrentVersion", "0.0");
                    var buildNumber = subKey.GetValue("CurrentBuild", "0");
                    var buildLabEx = subKey.GetValue("BuildLabEx")?.ToString().Split('.');
                    var revisionNumber = buildLabEx?.Length >= 2 ? buildLabEx[1] : "0";
                    return $"{version}.{buildNumber}.{revisionNumber}";
                }
            }
        }

        protected override string GetOsVersion()
        {
            try
            {
#if NETFRAMEWORK || NETCOREAPP3_0_OR_GREATER || NETSTANDARD3_0_OR_GREATER
                var osVersion = Environment.OSVersion.Version;
                return $"{osVersion.Major}.{osVersion.Minor}.{osVersion.Build}";
#elif USE_WMI_LIGHT
                using WmiConnection con = new();
                var q = con.CreateQuery("SELECT Version FROM Win32_OperatingSystem");
                foreach (var it in q)
                {
                    var ver = it.TryGetValue("Version")?.ToString();
                    return ver;
                }
#else
                var managementClass = _managmentClassFactory.GetOperatingSystemClass();
                foreach (var managementObject in managementClass.GetInstances())
                {
                    return (string)managementObject["Version"];
                }
#endif
            }
            catch (UnauthorizedAccessException exception)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OS version with error: ", exception);
                return string.Empty;
            }
            catch (COMException exception)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OS version. Make sure that WMI service is enabled.", exception);
                return string.Empty;
            }
            catch (ManagementException exception)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OS version. Make sure that WMI service is enabled.", exception);
                return string.Empty;
            }
            catch (PlatformNotSupportedException exception)
            {
                AppCenterLog.Warn(AppCenterLog.LogTag, "Failed to get device OS version. Make sure that .NET Framework is up to date.", exception);
                return string.Empty;
            }
            return string.Empty;
        }

        protected override string GetAppVersion()
        {
            return DeploymentVersion ?? ProductVersion ?? _defaultVersion;
        }

        protected override string GetAppBuild()
        {
            return DeploymentVersion ?? AssemblyVersion?.FileVersion ?? _defaultVersion;
        }

        protected override string GetScreenSize()
        {
            WindowsHelper.GetScreenSize(out int width, out int height);
            return $"{width}x{height}";
        }

        private static string ProductVersion
        {
            get
            {
                try
                {
                    return WindowsHelper.GetWinFormsProductVersion();
                }
                catch
                {
                    var assemblyVersion = AssemblyVersion;
                    return assemblyVersion?.ProductVersion ?? assemblyVersion?.FileVersion;
                }
            }
        }

        private static string DeploymentVersion
        {
            get
            {
#if NETFRAMEWORK
                // Get ClickOnce version.
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }

#elif WINDOWS10_0_17763_0_OR_GREATER
                if (WindowsHelper.IsRunningAsUwp)
                {
                    try
                    {
                        var packageVersion = global::Windows.ApplicationModel.Package.Current.Id.Version;
                        return $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}.{packageVersion.Revision}";
                    }
                    catch (InvalidOperationException exception)
                    {
                        AppCenterLog.Warn(AppCenterLog.LogTag, "Package version is available only in MSIX-packaged applications. See link https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-supported-api.", exception);
                    }
                }
#endif
                return null;
            }
        }

        private static FileVersionInfo AssemblyVersion
        {
            get
            {
#if NET6_0_OR_GREATER
                var processPath = Environment.ProcessPath;
                if (string.IsNullOrWhiteSpace(processPath))
                {
                    var fvi = FileVersionInfo.GetVersionInfo(processPath);
                    return fvi;
                }
                return null;
#else
                // The AssemblyFileVersion uniquely identifies a build.
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly != null)
                {
                    var assemblyLocation = entryAssembly.Location;
                    if (string.IsNullOrWhiteSpace(assemblyLocation))
                    {
                        // This is a fix for single file (self-contained publish) api incompatibility in runtime.
                        // Read at https://docs.microsoft.com/en-us/dotnet/core/deploying/single-file#api-incompatibility
                        assemblyLocation = Environment.GetCommandLineArgs()[0];
                    }
                    return FileVersionInfo.GetVersionInfo(assemblyLocation);
                }
                return null;
#endif
            }
        }
    }

#if USE_WMI_LIGHT
#nullable enable
    internal static class WmiLightExtensions
    {
        public static object? TryGetValue(this WmiObject? o, string key)
        {
            try
            {
                var result = o?.GetPropertyValue(key);
                return result;
            }
            catch
            {
                // WmiLight key 不存在时抛出 COM 异常 0x80041002
                return null;
            }
        }
    }
#endif
}
