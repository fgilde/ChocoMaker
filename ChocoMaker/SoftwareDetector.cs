using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using chocolatey;
using chocolatey.infrastructure.app.domain;
using chocolatey.infrastructure.results;
using ChocolateyGen;
using Microsoft.Win32;
using Registry = Microsoft.Win32.Registry;

namespace ChocoMaker
{
    public class SoftwareDetector
    {
        public static IEnumerable<SoftwareInfo> GetInstalledPrograms()
        {
            var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            return key?.GetSubKeyNames().Select(s => ReadAs<SoftwareInfo>(key.OpenSubKey(s))).Where(info => !info.IsEmpty);
        }

        public static IEnumerable<PackageResult> GetAvailableChocoPackagesForInstalledPrograms()
        {
            return GetAvailableChocoPackagesFor(GetInstalledPrograms());
        }

        public static IEnumerable<PackageResult> GetAvailableChocoPackagesFor(IEnumerable<SoftwareInfo> infos)
        {
            return infos.Select(GetAvailableChocoPackageFor).Where(result => result != null);
        }

        public static PackageResult GetAvailableChocoPackageFor(SoftwareInfo info)
        {
            return FindChocoPackage(info.DisplayName) ?? (info.Name != info.DisplayName ? FindChocoPackage(info.Name) : null);
        }

        private static PackageResult FindChocoPackage(string name)
        {
            return FindChocoPackages(name, 1).FirstOrDefault();
        }

        public static List<PackageResult> GetInstalledChocoPackages()
        {
            return new GetChocolatey().Set(config =>
            {
                config.CommandName = CommandNameType.list.ToString();
                config.ListCommand.LocalOnly = true;
            }).List<PackageResult>().ToList();
        }

        public static List<PackageResult> FindChocoPackages(string name, int limit = 10)
        {
            return new GetChocolatey().Set(config =>
            {
                config.CommandName = CommandNameType.list.ToString();
                config.Input = name;
                config.ListCommand.Page = 0;
                config.ListCommand.PageSize = limit;
                config.ListCommand.OrderByPopularity = true;
            }).List<PackageResult>().ToList();
        }

        private static T ReadAs<T>(RegistryKey subkey)
            where T : new()
        {
            var result = new T();
            foreach (var propertyInfo in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (propertyInfo.CanWrite && propertyInfo.PropertyType == typeof(string))
                    propertyInfo.SetValue(result, subkey.GetValue(propertyInfo.Name)?.ToString());
            }
            return result;
        }
    }
}