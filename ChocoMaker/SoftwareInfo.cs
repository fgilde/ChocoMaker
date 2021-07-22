using System;
using System.ComponentModel;

namespace ChocolateyGen
{
    public class SoftwareInfo
    {
        public string DisplayName { get; set; }
        public string DisplayVersion { get; set; }
        public string Publisher { get; set; }
        public string InstallLocation { get; set; }
        public string InstallSource { get; set; }
        public Version Version => Version.TryParse(DisplayVersion ?? "0", out Version v) ? v : new Version(0, 0);   
        internal bool IsEmpty => DisplayName == null;
        public string Name => DisplayName.Contains("(") ? DisplayName.Substring(0, DisplayName.IndexOf("(")).Trim() : DisplayName;
    }
}