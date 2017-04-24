using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Settings
{
    internal static class SettingsManager
    {
        private static readonly Dictionary<string, string> _settings = new Dictionary<string, string>();

        static SettingsManager()
        {
            var settingsFile = "./settings.cfg";

            if (!File.Exists(settingsFile)) return;

            var strings = File.ReadAllLines(settingsFile);

            foreach (var rawLine in strings)
            {
                var line = rawLine.Trim();

                if (string.IsNullOrEmpty(line)) continue;
                if (line.StartsWith("#")) continue;

                var varName = "";
                var value = "";

                var rawData = line.Split('=');

                if (rawData.Length < 2) continue;

                var varData = rawData[0].Trim().Split(' ');

                if (varData.Length == 1)
                {
                    varName = varData[0].Trim().ToLower();
                }
                else if (varData.Length == 2)
                {
                    var osType = varData[0].Trim().ToLower();
                    varName = varData[1].Trim().ToLower();

                    if (osType != "mac" && (Application.platform == RuntimePlatform.OSXDashboardPlayer || Application.platform == RuntimePlatform.OSXEditor ||
                                            Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXWebPlayer))
                    {
                        continue;
                    }

                    if (osType != "win" && (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor ||
                                            Application.platform == RuntimePlatform.WindowsWebPlayer))
                    {
                        continue;
                    }
                }

                if (_settings.ContainsKey(varName)) continue;

                value = rawData[1];

                if (rawData.Length > 2)
                {
                    for (var i = 2; i < rawData.Length; i++)
                    {
                        value += "=" + rawData[i];
                    }
                }

                _settings.Add(varName, value.Replace("\\n", "\n").Trim());
            }
        }

        internal static string Get(object ip)
        {
            throw new NotImplementedException();
        }

        public static string Get(string key)
        {
            return Get(key, string.Empty);
        }

        public static int GetInt(string key)
        {
            return GetInt(key, 0);
        }

        public static bool GetBool(string key)
        {
            return GetBool(key, false);
        }

        public static string Get(string key, string def)
        {
            return _settings.ContainsKey(key) ? _settings[key] : def;
        }

        public static int GetInt(string key, int def)
        {
            return !_settings.ContainsKey(key) ? def : Convert.ToInt32(_settings[key]);
        }

        public static int GetInt(string key, string def)
        {
            int value;

            try
            {
                value = Convert.ToInt32(def);
            }
            catch (Exception)
            {
                value = 0;
            }

            return GetInt(key, value);
        }

        public static bool GetBool(string key, bool def)
        {
            if (!_settings.ContainsKey(key)) return def;

            return string.Equals(_settings[key], "yes", StringComparison.CurrentCultureIgnoreCase) || string.Equals(_settings[key], "true", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool GetBool(string key, string def)
        {
            if (!_settings.ContainsKey(key)) return string.Equals(def, "yes", StringComparison.CurrentCultureIgnoreCase) || string.Equals(def, "true", StringComparison.CurrentCultureIgnoreCase);

            return string.Equals(_settings[key], "yes", StringComparison.CurrentCultureIgnoreCase) || string.Equals(_settings[key], "true", StringComparison.CurrentCultureIgnoreCase);
        }



    }
}