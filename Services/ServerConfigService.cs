using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace MyMiningPlugin.Services
{
    // The "send to server" URL used to be a free-text field the user retyped every time,
    // which meant anyone could accidentally point it at the wrong host/scheme. It now
    // lives in ServerConfig.xml next to the plugin DLL, so it's set once (by whoever
    // deploys the plugin) and the send dialog just uses it.
    public static class ServerConfigService
    {
        private const string DefaultUrl = "https://mica.edu.vn/mineterra3d:55320/";
        private static string _cachedUrl;

        public static string GetServerUrl()
        {
            if (_cachedUrl != null) return _cachedUrl;

            string path = ConfigPath;
            try
            {
                if (!File.Exists(path))
                {
                    WriteDefault(path);
                    _cachedUrl = DefaultUrl;
                    return _cachedUrl;
                }

                var doc = XDocument.Load(path);
                string url = doc.Root?.Element("Url")?.Value?.Trim();
                _cachedUrl = string.IsNullOrEmpty(url) ? DefaultUrl : url;
            }
            catch
            {
                // A malformed/unreadable config shouldn't stop the plugin from working —
                // fall back to the built-in default instead of throwing into the UI.
                _cachedUrl = DefaultUrl;
            }

            return _cachedUrl;
        }

        private static string ConfigPath
        {
            get
            {
                string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Combine(dir ?? string.Empty, "ServerConfig.xml");
            }
        }

        private static void WriteDefault(string path)
        {
            var doc = new XDocument(new XElement("ServerConfig", new XElement("Url", DefaultUrl)));
            doc.Save(path);
        }
    }
}
