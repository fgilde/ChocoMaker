using System.Collections.Generic;
using System.Linq;
using System.Text;
using chocolatey.infrastructure.results;
using ChocoMaker.Properties;

namespace ChocoMaker
{
    public static class ChocoScriptGenerator
    {
        public static string Generate(IEnumerable<PackageResult> packages, bool addChocoInstallCmd)
        {
            var packageResults = packages != null ? packages as PackageResult[] ?? packages.ToArray() : null;
            if (packageResults != null && packageResults.Any())
            {
                var builder = new StringBuilder();
                if (addChocoInstallCmd)
                    builder.AppendLine(Resources.ChocolateyInstallCmd);
                builder.AppendLine();
                foreach (var packageResult in packageResults)
                    builder.AppendLine(GenerateInstallCmd(packageResult));
                builder.AppendLine("pause;");
                return builder.ToString();
            }

            return string.Empty;
        }

        public static string GenerateInstallCmd(PackageResult packageResult)
        {
            var cmd = Settings.Default.InstallCmd;
            return !string.IsNullOrEmpty(cmd) ? cmd.Replace("{package}", packageResult.Package.Id) : $"choco install {packageResult.Package.Id} -y";
        }
    }
}