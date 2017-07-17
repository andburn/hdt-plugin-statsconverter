using GalaSoft.MvvmLight;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace HDT.Plugins.StatsConverter.ViewModels
{
	public class ToastViewModel : ViewModelBase
	{
		public ToastViewModel()
			: this(false, false)
		{
		}

		public ToastViewModel(bool ok)
			: this(ok, false)
		{
		}

		public ToastViewModel(bool ok, bool visible)
		{
			Ok = ok;
			Visible = visible ? Visibility.Visible : Visibility.Hidden;
			Icon = string.Empty;
			Message = string.Empty;
			FgColor = Brushes.Black;
			BgColor = Brushes.White;
		}

		private bool _ok;

		public bool Ok
		{
			get { return _ok; }
			set { Set(() => Ok, ref _ok, value); }
		}

		private Visibility _visible;

		public Visibility Visible
		{
			get { return _visible; }
			set { Set(() => Visible, ref _visible, value); }
		}

		private string _icon;

		public string Icon
		{
			get { return _icon; }
			set { Set(() => Icon, ref _icon, value); }
		}

		private string _message;

		public string Message
		{
			get { return _message; }
			set { Set(() => Message, ref _message, value); }
		}

		private Brush _fgColor;

		public Brush FgColor
		{
			get { return _fgColor; }
			set { Set(() => FgColor, ref _fgColor, value); }
		}

		private Brush _bgColor;

		public Brush BgColor
		{
			get { return _bgColor; }
			set { Set(() => BgColor, ref _bgColor, value); }
		}

		public async Task Show(int seconds = 10)
		{
			Visible = Visibility.Visible;
			await Task.Delay(TimeSpan.FromSeconds(seconds));
			Visible = Visibility.Hidden;
		}
	}
}