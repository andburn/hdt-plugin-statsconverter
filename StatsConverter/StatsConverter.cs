using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using HDT.Plugins.Common.Controls;
using HDT.Plugins.Common.Services;
using HDT.Plugins.Common.Settings;
using HDT.Plugins.Common.Utils;
using HDT.Plugins.StatsConverter.ViewModels;
using HDT.Plugins.StatsConverter.Views;
using Ninject;

namespace HDT.Plugins.StatsConverter
{
	public class StatsConverter : IPluggable
	{
		public static IUpdateService Updater;
		public static ILoggingService Logger;
		public static IDataRepository Data;
		public static IEventsService Events;
		public static IGameClientService Client;
		public static IConfigurationRepository Config;
		public static Settings Settings;
		public static MainViewModel MainViewModel;

		private static IKernel _kernel;
		private static Version _version;

		public static readonly string Name = "Stats Converter";

		public StatsConverter(IKernel kernel, Version version)
		{
			_kernel = kernel;
			_version = version;
			// initialize services
			Updater = _kernel.Get<IUpdateService>();
			Logger = _kernel.Get<ILoggingService>();
			Data = _kernel.Get<IDataRepository>();
			Events = _kernel.Get<IEventsService>();
			Client = _kernel.Get<IGameClientService>();
			Config = _kernel.Get<IConfigurationRepository>();
			// load settings
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "HDT.Plugins.StatsConverter.Resources.Default.ini";
			Settings = new Settings(assembly.GetManifestResourceStream(resourceName), "StatsConverter");
			// other
			MainViewModel = new MainViewModel();
		}

		public async void Load()
		{
			await UpdateCheck("andburn", "hdt-plugin-statsconverter");
		}

		public void Unload()
		{
			SlidePanelManager.DetachAll();
			CloseMainView();
		}

		public void ButtonPress()
		{
			ShowMainView();
		}

		private void ShowMainView()
		{
			MainView view = null;
			// check for any open windows
			var open = Application.Current.Windows.OfType<MainView>();
			if (open.Count() == 1)
			{
				view = open.FirstOrDefault();
			}
			else
			{
				CloseMainView();
				// create view
				view = new MainView();
				view.DataContext = MainViewModel;
			}
			view.Show();
			if (view.WindowState == WindowState.Minimized)
				view.WindowState = WindowState.Normal;
			view.Activate();
		}

		private void CloseMainView()
		{
			foreach (var view in Application.Current.Windows.OfType<MainView>())
				view.Close();
		}

		public MenuItem CreateMenu()
		{
			PluginMenu pm = new PluginMenu("Stats Converter", IcoMoon.PieChart,
				new RelayCommand(() => ShowMainView()));
			return pm.Menu;
		}

		public static void Notify(string title, string message, int autoClose, string icon = null, Action action = null)
		{
			SlidePanelManager
				.Notification(_kernel.Get<ISlidePanel>(), title, message, icon, action)
				.AutoClose(autoClose);
		}

		private async Task UpdateCheck(string user, string repo)
		{
			try
			{
				var latest = await Updater.CheckForUpdate(user, repo, _version);
				if (latest.HasUpdate)
				{
					Logger.Info($"Plugin Update available ({latest.Version})");
					Notify("Plugin Update Available",
						$"[DOWNLOAD]({latest.DownloadUrl}) {Name} v{latest.Version}",
						10, IcoMoon.Download3, () => Process.Start(latest.DownloadUrl));
				}
			}
			catch (Exception e)
			{
				Logger.Error($"Github update failed: {e.Message}");
			}
		}

		public void Repeat()
		{
		}

		private Version _version;
		private IKernel _kernel;
		private IPluggable _plugin;

		public Plugin()
		{
			_version = GetVersion() ?? new Version(0, 0, 0, 0);
			_kernel = GetKernel();
			_plugin = new StatsConverter(_kernel, _version);
		}

		private MenuItem _menuItem;

		public MenuItem MenuItem
		{
			get
			{
				if (_menuItem == null)
					_menuItem = _plugin.CreateMenu();
				return _menuItem;
			}
		}

		public string Name => "Stats Converter";

		public string Description => "Import and export game statistics in different formats.";

		public string ButtonText => "Settings";

		public string Author => "andburn";

		public Version Version => _version;

		public void OnButtonPress() => _plugin.ButtonPress();

		public void OnLoad() => _plugin.Load();

		public void OnUnload() => _plugin.Unload();

		public void OnUpdate() => _plugin.Repeat();

		private IKernel GetKernel()
		{
			var kernel = new StandardKernel();
			kernel.Bind<IDataRepository>().To<TrackerDataRepository>().InSingletonScope();
			kernel.Bind<IUpdateService>().To<GitHubUpdateService>().InSingletonScope();
			kernel.Bind<ILoggingService>().To<TrackerLoggingService>().InSingletonScope();
			kernel.Bind<IEventsService>().To<TrackerEventsService>().InSingletonScope();
			kernel.Bind<IGameClientService>().To<TrackerClientService>().InSingletonScope();
			kernel.Bind<IConfigurationRepository>().To<TrackerConfigRepository>().InSingletonScope();
			kernel.Bind<ISlidePanel>().To<MetroSlidePanel>();
			return kernel;
		}

		private Version GetVersion()
		{
			return GitVersion.Get(Assembly.GetExecutingAssembly(), this);
		}
	}
}