using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using chocolatey.infrastructure.results;

namespace ChocoMaker.ViewModels
{
    public class MainViewModel : ModelBase
    {
        private string _statusMessage;
        private bool _addChocoInstallCmd;

        public MainViewModel()
        {
            Installed = new ObservableCollection<SoftwareInfoModel>(SoftwareDetector.GetInstalledPrograms().Select(si =>
            {
                var r = new SoftwareInfoModel(si);
                r.PropertyChanged += OnItemPropertyChanged;
                return r;
            }));
        }
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }
        public bool HasValidSelection => Installed.Count(x => x.Checked) > 0;
        public ObservableCollection<SoftwareInfoModel> Installed { get; set; }
        public ObservableCollection<PackageModel> AvailablePackages { get; set; }
        public bool IsLoading => Installed.Any(m => m.Checked && m.Loading);
        public bool CanGenerate => AvailablePackages?.Any(m => m.Checked) ?? false;
        public bool AddChocoInstallCmd
        {
            get => _addChocoInstallCmd;
            set
            {
                _addChocoInstallCmd = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Script));
            }
        }

        public string Script
        {
            get
            {
                return ChocoScriptGenerator.Generate(AvailablePackages?.Where(m => m.Checked).Select(model => model.PackageResult), AddChocoInstallCmd);
            }
            set { }
        }


        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Checked")
                OnPropertyChanged(nameof(HasValidSelection));

            if (e.PropertyName == "Loading")
                CheckStatus();
        }


        private void CheckStatus()
        {
            OnPropertyChanged(nameof(IsLoading));
            StatusMessage = IsLoading ? "Please wait while Searching for matching packages" : "Select Chocolatey packages to generate Script for";
            if (!IsLoading)
            {
                AvailablePackages = new ObservableCollection<PackageModel>(
                    Installed.Where(model =>
                            model.Checked && model.SearchTask?.Result != null && model.SearchTask.Exception == null)
                        .Select(model =>
                        {
                            var packageModel = new PackageModel(model.SearchTask.Result);
                            packageModel.PropertyChanged += (sender, args) =>
                            {
                                OnPropertyChanged(nameof(CanGenerate));
                                OnPropertyChanged(nameof(Script));
                            };
                            return packageModel;
                        }));

                OnPropertyChanged(nameof(AvailablePackages));
                OnPropertyChanged(nameof(Script));
                OnPropertyChanged(nameof(CanGenerate));
            }
        }


    }
}