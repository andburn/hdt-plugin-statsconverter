using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HDT.Plugins.StatsConverter.Controls
{
	/// <summary>
	/// Interaction logic for MoonTextButton.xaml
	/// </summary>
	public partial class MoonTextButton : UserControl
	{
		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(MoonTextButton), new PropertyMetadata(""));

		public string Icon
		{
			get { return (string)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}

		public static readonly DependencyProperty IconProperty =
			DependencyProperty.Register("Icon", typeof(string), typeof(MoonTextButton), new PropertyMetadata("\uea08"));

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(MoonTextButton), null);

		public object Param
		{
			get { return (object)GetValue(ParamProperty); }
			set { SetValue(ParamProperty, value); }
		}

		public static readonly DependencyProperty ParamProperty =
			DependencyProperty.Register("Param", typeof(object), typeof(MoonTextButton), null);

		public MoonTextButton()
		{
			InitializeComponent();
		}
	}
}