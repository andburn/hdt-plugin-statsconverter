using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.Common.Enums;
using HDT.Plugins.Common.Models;
using HDT.Plugins.Common.Utils;
using HDT.Plugins.StatsConverter.Converters;
using HDT.Plugins.StatsConverter.Converters.CSV;
using HDT.Plugins.StatsConverter.Converters.XML;
using HDT.Plugins.StatsConverter.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace HDT.Plugins.StatsConverter.ViewModels
{
	public class ExportViewModel : ViewModelBase
	{
		private static readonly Deck ALL_DECK = new Deck(Guid.Empty, "All", false, "All", false);

		private static string _gameCountFormatString = "{0} game{1} found";

		private ObservableCollection<Deck> _decks;

		public ObservableCollection<Deck> Decks
		{
			get { return _decks; }
			set { Set(() => Decks, ref _decks, value); }
		}

		private IEnumerable<GameMode> _gameModes;

		public IEnumerable<GameMode> GameModes
		{
			get { return _gameModes; }
			set { Set(() => GameModes, ref _gameModes, value); }
		}

		private IEnumerable<GameFormat> _gameFormats;

		public IEnumerable<GameFormat> GameFormats
		{
			get { return _gameFormats; }
			set { Set(() => GameFormats, ref _gameFormats, value); }
		}

		private IEnumerable<TimeFrame> _timePeriods;

		public IEnumerable<TimeFrame> TimePeriods
		{
			get { return _timePeriods; }
			set { Set(() => TimePeriods, ref _timePeriods, value); }
		}

		private IEnumerable<Region> _regions;

		public IEnumerable<Region> Regions
		{
			get { return _regions; }
			set { Set(() => Regions, ref _regions, value); }
		}

		private IEnumerable<IStatsConverter> _exporters;

		public IEnumerable<IStatsConverter> Exporters
		{
			get { return _exporters; }
			set { Set(() => Exporters, ref _exporters, value); }
		}

		private GameMode _selectedGameMode;

		public GameMode SelectedGameMode
		{
			get { return _selectedGameMode; }
			set { Set(() => SelectedGameMode, ref _selectedGameMode, value); }
		}

		private GameFormat _selectedGameFormat;

		public GameFormat SelectedGameFormat
		{
			get { return _selectedGameFormat; }
			set { Set(() => SelectedGameFormat, ref _selectedGameFormat, value); }
		}

		private TimeFrame _selectedTimeFrame;

		public TimeFrame SelectedTimeFrame
		{
			get { return _selectedTimeFrame; }
			set { Set(() => SelectedTimeFrame, ref _selectedTimeFrame, value); }
		}

		private Region _selectedRegion;

		public Region SelectedRegion
		{
			get { return _selectedRegion; }
			set { Set(() => SelectedRegion, ref _selectedRegion, value); }
		}

		private Deck _selectedDeck;

		public Deck SelectedDeck
		{
			get { return _selectedDeck; }
			set { Set(() => SelectedDeck, ref _selectedDeck, value); }
		}

		private IStatsConverter _selectedExporter;

		public IStatsConverter SelectedExporter
		{
			get { return _selectedExporter; }
			set { Set(() => SelectedExporter, ref _selectedExporter, value); }
		}

		private bool _couldBeArena;

		public bool CouldBeArena
		{
			get { return _couldBeArena; }
			set { Set(() => CouldBeArena, ref _couldBeArena, value); }
		}

		private int _gameCount;

		public int GameCount
		{
			get { return _gameCount; }
			set
			{
				Set(() => GameCount, ref _gameCount, value);
				GameCountString = string.Format(_gameCountFormatString, _gameCount, _gameCount == 1 ? "" : "s");
				HasGames = _gameCount > 0;
			}
		}

		private string _gameCountString;

		public string GameCountString
		{
			get { return _gameCountString; }
			set { Set(() => GameCountString, ref _gameCountString, value); }
		}

		private bool _hasGames;

		public bool HasGames
		{
			get { return _hasGames; }
			set { Set(() => HasGames, ref _hasGames, value); }
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

		public RelayCommand ExportCommand { get; private set; }

		private static string[] UpdatableProps = new string[] {
			"SelectedDeck", "SelectedGameMode", "SelectedGameFormat", "SelectedTimeFrame", "SelectedRegion"
		};

		public ExportViewModel()
		{
			// initialize selection lists
			GameModes = Enum.GetValues(typeof(GameMode)).OfType<GameMode>();
			GameFormats = Enum.GetValues(typeof(GameFormat)).OfType<GameFormat>();
			TimePeriods = Enum.GetValues(typeof(TimeFrame)).OfType<TimeFrame>();
			Regions = Enum.GetValues(typeof(Region)).OfType<Region>().Where(x => x != Region.UNKNOWN);
			Decks = new ObservableCollection<Deck>();
			Exporters = new List<IStatsConverter>() {
				new CSVConverter(),
				new OpenXMLConverter()
			};
			// set default selections
			CouldBeArena = false;
			StatusObj = new ToastViewModel();
			SelectedGameMode = GameMode.NONE;
			SelectedGameFormat = GameFormat.ANY;
			SelectedRegion = Region.US;
			SelectedTimeFrame = TimeFrame.ALL;
			SelectedExporter = Exporters.FirstOrDefault();

			PropertyChanged += ExportViewModel_PropertyChanged;
			ExportCommand = new RelayCommand(async () => await ExportStats());

			// trigger update with a property change
			SelectedGameMode = GameMode.ALL;
		}

		private async void ExportViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (UpdatableProps.Contains(e.PropertyName))
			{
				StatsConverter.Logger.Debug("UpdatableProp: " + e.PropertyName);
				GameCountString = "Loading, Please Wait";
				try
				{
					if (e.PropertyName == "SelectedGameFormat" || e.PropertyName == "SelectedGameMode" || SelectedDeck == null)
						await FilterDecks(SelectedGameMode, SelectedGameFormat);
					await UpdateGameCount();
					UpdateArenaStatus();
				}
				catch (Exception err)
				{
					GameCountString = "Loading stats, Failed";
					StatsConverter.Logger.Error(err);
				}
			}
		}

		public async Task FilterDecks(GameMode mode, GameFormat format)
		{
			Decks.Clear();
			var allDecks = await GetAllDecks();
			allDecks.Where(d =>
				{
					var include = true;
					switch (mode)
					{
						case GameMode.ALL:
							include = include && true; break;

						case GameMode.ARENA:
							include = include && d.IsArena; break;

						default:
							include = include && !d.IsArena; break;
					}
					switch (format)
					{
						case GameFormat.ANY:
							include = include && true; break;

						case GameFormat.STANDARD:
							include = include && d.IsStandard; break;

						case GameFormat.WILD:
							include = include && !d.IsStandard; break;
					}
					return include;
				})
				.OrderBy(d => d.Name)
				.ToList()
				.ForEach(d => Decks.Add(d));
			Decks.Insert(0, ALL_DECK);
			Decks.Insert(1, Deck.None);
			SelectedDeck = ALL_DECK;
		}

		public async Task ExportStats()
		{
			var deck = SelectedDeck == ALL_DECK ? null : SelectedDeck;
			var filter = new GameFilter(deck?.Id, SelectedRegion, SelectedGameMode, SelectedTimeFrame, SelectedGameFormat);
			var filename = string.Empty;
			if (StatsConverter.Settings.Get(Strings.ExportWithoutDialog).Bool)
			{
				filename = Path.Combine(
					StatsConverter.Settings.Get(Strings.DefaultExportPath),
					ViewModelHelper.GetDefaultFileName() + "." + SelectedExporter.FileExtension);
			}
			else
			{
				filename = ViewModelHelper.SaveFileDialog(
					SelectedExporter.Name,
					SelectedExporter.FileExtension,
					StatsConverter.Settings.Get(Strings.DefaultExportPath));
			}
			Status = await Converter.Export(StatsConverter.Data, SelectedExporter, filter, filename);
		}

		private void UpdateArenaStatus()
		{
			CouldBeArena = SelectedGameMode == GameMode.ARENA || SelectedDeck.IsArena;
		}

		private async Task<List<Deck>> GetAllDecks()
		{
			return await Task.Run(() => StatsConverter.Data.GetAllDecks());
		}

		private async Task UpdateGameCount()
		{
			var deck = SelectedDeck == ALL_DECK ? null : SelectedDeck;
			var filter = new GameFilter(deck?.Id, SelectedRegion, SelectedGameMode, SelectedTimeFrame, SelectedGameFormat);
			var games = await Task.Run(() => StatsConverter.Data.GetAllGames());
			GameCount = filter.Apply(games).Count;
		}

		private async Task UpdateStatusObj()
		{
			StatusObj.Icon = Status ? "\uea10" : "\uea0f";
			StatusObj.Message = Status ? "Export Successful" : "Export Failed";
			StatusObj.FgColor = Brushes.White;
			StatusObj.BgColor = Status ? Brushes.Green : Brushes.Red;
			await StatusObj.Show(4);
		}
	}
}