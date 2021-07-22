using chocolatey.infrastructure.results;

namespace ChocoMaker.ViewModels
{
    public class PackageModel : ModelBase
    {
        public PackageResult PackageResult { get; }

        private bool _checked = true;

        public PackageModel(PackageResult packageResult)
        {
            PackageResult = packageResult;
        }

        public string Name => PackageResult.Name;
        public string Version => PackageResult.Version;
        public string Id => PackageResult.Package.Id;
        public string Location => PackageResult.SourceUri;
        public string Publisher => string.Join(",", PackageResult.Package.Authors);
        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                OnPropertyChanged();
            }
        }
    }
}