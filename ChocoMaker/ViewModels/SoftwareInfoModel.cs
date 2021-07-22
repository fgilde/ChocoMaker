using System;
using System.Threading.Tasks;
using chocolatey.infrastructure.results;
using ChocolateyGen;

namespace ChocoMaker.ViewModels
{
    public class SoftwareInfoModel : ModelBase
    {
        private bool _checked;
        
        private SoftwareInfo info { get; }

        public SoftwareInfoModel(SoftwareInfo info)
        {
            this.info = info;
        }

        public string Name => info.Name;
        public Version Version => info.Version;
        public string Publisher => info.Publisher;
        public bool Loading => !SearchTask?.IsCompleted ?? false;

        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                OnPropertyChanged();
                if (value)
                {
                    SearchPackage();
                }
            }
        }

        internal Task<PackageResult> SearchTask;

        private void SearchPackage()
        {
            if (SearchTask == null)
            {
                SearchTask = Task.Run(() => SoftwareDetector.GetAvailableChocoPackageFor(info));
                OnPropertyChanged(nameof(Loading));
                SearchTask.ContinueWith(task => OnPropertyChanged(nameof(Loading)));
            }
        }
    }
}