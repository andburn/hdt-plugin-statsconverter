using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.StatsConverter.Converters;
using HDT.Plugins.StatsConverter.Converters.CSV;
using HDT.Plugins.StatsConverter.Converters.XML;
using HDT.Plugins.StatsConverter.Utils;
using System.Windows.Media;
using System.Threading.Tasks;
using HDT.Plugins.Common.Utils;

namespace HDT.Plugins.StatsConverter.ViewModels
{
	public class ImportViewModel : ViewModelBase
	{
		private IEnumerable<IStatsConverter> _importers;

		public IEnumerable<IStatsConverter> Importers
		{
			get { return _importers; }
			set { Set(() => Importers, ref _importers, value); }
		}

		private IStatsConverter _selectedImporter;

		public IStatsConverter SelectedImporter
		{
			get { return _selectedImporter; }
			set { Set(() => SelectedImporter, ref _selectedImporter, value); }
		}

		private bool _status;

		public bool Status
		{
			get { return _status; }
			set
			{
				Set(() => Status, ref _status, value);
				StatsConverter.Logger.Debug($"Status: {_status}");
				UpdateStatusObj().Forget();
			}
		}

		private ToastViewModel _statusObj;

		public ToastViewModel StatusObj
		{
			get { return _statusObj; }
			set { Set(() => StatusObj, ref _statusObj, value); }
		}

		public bool ShowWarning
		{
			get { return StatsConverter.Settings.Get(Strings.ShowWarning).Bool; }
		}

		public RelayCommand ImportCommand { get; private set; }

		public ImportViewModel()
		{
			Importers = new List<IStatsConverter>() {
				new CSVConverter(),
				new OpenXMLConverter()
			};
			StatusObj = new ToastViewModel();
			SelectedImporter = Importers.FirstOrDefault();
			ImportCommand = new RelayCommand(() => ImportGames());
		}

		private void ImportGames()
		{
			var filename = ViewModelHelper.OpenFileDialog(
				SelectedImporter.Name,
				SelectedImporter.FileExtension,
				StatsConverter.Settings.Get(Strings.DefaultExportPath));
			Status = Converter.Import(SelectedImporter, filename);
		}

		private async Task UpdateStatusObj()
		{
			StatusObj.Icon = Status ? "\uea10" : "\uea0f";
			StatusObj.Message = Status ? "Import Successful" : "Import Failed";
			StatusObj.FgColor = Brushes.White;
			StatusObj.BgColor = Status ? Brushes.Green : Brushes.Red;
			await StatusObj.Show(4);
		}

	}
}