namespace OTLandIPChanger
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    [SettingsProvider("System.Configuration.LocalFileSettingsProvider")]
    public sealed class Config : ApplicationSettingsBase
    {
        private static Config _config = new Config();

        [DefaultSettingValue("false"), UserScopedSetting]
        public bool AlwaysLaunchNewClient
        {
            get
            {
                return (bool) this["AlwaysLaunchNewClient"];
            }
            set
            {
                this["AlwaysLaunchNewClient"] = value;
            }
        }

        [DefaultSettingValue(""), UserScopedSetting]
        public List<TibiaPathEntry> ClientPaths
        {
            get
            {
                return (List<TibiaPathEntry>) this["ClientPaths"];
            }
            set
            {
                this["ClientPaths"] = value;
            }
        }

        [UserScopedSetting, DefaultSettingValue("false")]
        public bool ForceGraphicsEngine
        {
            get
            {
                return (bool) this["ForceGraphicsEngine"];
            }
            set
            {
                this["ForceGraphicsEngine"] = value;
            }
        }

        public static Config Instance
        {
            get
            {
                return _config;
            }
            set
            {
                _config = value;
            }
        }

        [DefaultSettingValue("false"), UserScopedSetting]
        public bool LaunchWithGamemasterFlag
        {
            get
            {
                return (bool) this["LaunchWithGamemasterFlag"];
            }
            set
            {
                this["LaunchWithGamemasterFlag"] = value;
            }
        }

        [DefaultSettingValue("0"), UserScopedSetting]
        public int SelectedGraphicsEngine
        {
            get
            {
                return (int) this["SelectedGraphicsEngine"];
            }
            set
            {
                this["SelectedGraphicsEngine"] = value;
            }
        }

        [UserScopedSetting, DefaultSettingValue("false")]
        public bool StartWithWindows
        {
            get
            {
                return (bool) this["StartWithWindows"];
            }
            set
            {
                this["StartWithWindows"] = value;
            }
        }

        [UserScopedSetting, DefaultSettingValue("false")]
        public bool StoreClientConfigurationSeparate
        {
            get
            {
                return (bool) this["StoreClientConfigurationSeparate"];
            }
            set
            {
                this["StoreClientConfigurationSeparate"] = value;
            }
        }

        [DefaultSettingValue("127.0.0.1"), UserScopedSetting]
        public string TargetHostname
        {
            get
            {
                return (string) this["TargetHostname"];
            }
            set
            {
                this["TargetHostname"] = value;
            }
        }

        [UserScopedSetting, DefaultSettingValue("")]
        public string TargetVersion
        {
            get
            {
                return (string) this["TargetVersion"];
            }
            set
            {
                this["TargetVersion"] = value;
            }
        }

        [UserScopedSetting, DefaultSettingValue("True")]
        public bool UpdateRequired
        {
            get
            {
                return (bool) this["UpdateRequired"];
            }
            set
            {
                this["UpdateRequired"] = value;
            }
        }
    }
}

