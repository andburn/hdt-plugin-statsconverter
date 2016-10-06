using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.StatsConverter.Converters;
using HDT.Plugins.StatsConverter.Converters.CSV;

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

		public RelayCommand ImportCommand { get; private set; }

		public ImportViewModel()
		{
			Importers = new List<IStatsConverter>() {
				new CSVConverter()
			};
			SelectedImporter = Importers.FirstOrDefault();
			ImportCommand = new RelayCommand(() => ImportGames());
		}

		private void ImportGames()
		{
			// TODO opens save file, want open file
			var filename = Utilities.SelectFile(
				SelectedImporter.Name,
				SelectedImporter.FileExtension,
				StatsConverter.Settings.Get("DefaultExportPath"));
			Converter.Import(SelectedImporter, filename);
		}
	}
}