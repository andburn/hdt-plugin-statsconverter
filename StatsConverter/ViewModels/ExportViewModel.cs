﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.Common.Models;
using HDT.Plugins.Common.Util;
using HDT.Plugins.StatsConverter.Converters;
using HDT.Plugins.StatsConverter.Converters.CSV;

namespace HDT.Plugins.StatsConverter.ViewModels
{
	public class ExportViewModel : ViewModelBase
	{
		private List<Deck> _allDecks;

		private static readonly Deck ALL_DECK = new Deck(Guid.Empty, "All", false);

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
			set
			{
				Set(() => GameModes, ref _gameModes, value);
				UpdateGameCount();
			}
		}

		private IEnumerable<TimeFrame> _timePeriods;

		public IEnumerable<TimeFrame> TimePeriods
		{
			get { return _timePeriods; }
			set
			{
				Set(() => TimePeriods, ref _timePeriods, value);
				UpdateGameCount();
			}
		}

		private IEnumerable<Region> _regions;

		public IEnumerable<Region> Regions
		{
			get { return _regions; }
			set
			{
				Set(() => Regions, ref _regions, value);
				UpdateGameCount();
			}
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
			set
			{
				Set(() => SelectedGameMode, ref _selectedGameMode, value);
				UpdateArenaStatus();
				FilterDecks(value);
				UpdateGameCount();
			}
		}

		private TimeFrame _selectedTimeFrame;

		public TimeFrame SelectedTimeFrame
		{
			get { return _selectedTimeFrame; }
			set
			{
				Set(() => SelectedTimeFrame, ref _selectedTimeFrame, value);
				UpdateGameCount();
			}
		}

		private Region _selectedRegion;

		public Region SelectedRegion
		{
			get { return _selectedRegion; }
			set
			{
				Set(() => SelectedRegion, ref _selectedRegion, value);
				UpdateGameCount();
			}
		}

		private Deck _selectedDeck;

		public Deck SelectedDeck
		{
			get { return _selectedDeck; }
			set
			{
				Set(() => SelectedDeck, ref _selectedDeck, value);
				UpdateGameCount();
				UpdateArenaStatus();
			}
		}

		private IStatsConverter _selectedExporter;

		public IStatsConverter SelectedExporter
		{
			get { return _selectedExporter; }
			set
			{
				Set(() => SelectedExporter, ref _selectedExporter, value);
				UpdateGameCount();
			}
		}

		private bool _includeArenaExtras;

		public bool IncludeArenaExtras
		{
			get { return _includeArenaExtras; }
			set { Set(() => IncludeArenaExtras, ref _includeArenaExtras, value); }
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

		public RelayCommand ExportCommand { get; private set; }

		public ExportViewModel()
		{
			_allDecks = StatsConverter.Data.GetAllDecks();
			// initialize selection lists
			GameModes = Enum.GetValues(typeof(GameMode)).OfType<GameMode>();
			TimePeriods = Enum.GetValues(typeof(TimeFrame)).OfType<TimeFrame>();
			Regions = Enum.GetValues(typeof(Region)).OfType<Region>().Where(x => x != Region.UNKNOWN);
			Decks = new ObservableCollection<Deck>();
			Exporters = new List<IStatsConverter>() {
				new CSVConverter()
			};
			// set default selections
			SelectedGameMode = GameMode.ALL;
			SelectedRegion = Region.US;
			SelectedTimeFrame = TimeFrame.ALL;
			SelectedExporter = Exporters.FirstOrDefault();

			FilterDecks(SelectedGameMode);
			CouldBeArena = false;

			ExportCommand = new RelayCommand(() => ExportStats());
		}

		public void FilterDecks(GameMode mode)
		{
			Decks.Clear();
			_allDecks
				.Where(d =>
				{
					switch (mode)
					{
						case GameMode.ALL:
							return true;

						case GameMode.ARENA:
							return d.IsArena;

						default:
							return !d.IsArena;
					}
				})
				.OrderBy(d => d.Name)
				.ToList()
				.ForEach(d => Decks.Add(d));
			Decks.Insert(0, ALL_DECK);
			SelectedDeck = ALL_DECK;
		}

		public void ExportStats()
		{
			var deck = SelectedDeck == ALL_DECK ? null : SelectedDeck;
			var filter = new GameFilter(deck?.Id, SelectedRegion, SelectedGameMode, SelectedTimeFrame);
			var filename = Utilities.SelectFile(
				SelectedExporter.Name,
				SelectedExporter.FileExtension,
				StatsConverter.Settings.Get("DefaultExportPath"),
				true);
			Converter.Export(SelectedExporter, filter, filename);
		}

		private void UpdateArenaStatus()
		{
			CouldBeArena = SelectedDeck == null ? false : SelectedDeck.IsArena || SelectedGameMode == GameMode.ARENA;
		}

		private void UpdateGameCount()
		{
			var deck = SelectedDeck == ALL_DECK ? null : SelectedDeck;
			var filter = new GameFilter(deck?.Id, SelectedRegion, SelectedGameMode, SelectedTimeFrame);
			var games = StatsConverter.Data.GetAllGames();
			GameCount = filter.Apply(games).Count;
		}
	}
}