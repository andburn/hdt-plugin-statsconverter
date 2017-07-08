using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.StatsConverter.Converters;
using HDT.Plugins.StatsConverter.Converters.CSV;
using HDT.Plugins.StatsConverter.Converters.XML;
using HDT.Plugins.StatsConverter.Utils;

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
				new CSVConverter(),
				new OpenXMLConverter()
			};
			SelectedImporter = Importers.FirstOrDefault();
			ImportCommand = new RelayCommand(() => ImportGames());
		}

		private void ImportGames()
		{
			var filename = ViewModelHelper.OpenFileDialog(
				SelectedImporter.Name,
				SelectedImporter.FileExtension,
				StatsConverter.Settings.Get(Strings.DefaultExportPath));
			Converter.Import(SelectedImporter, filename);
		}
	}
}