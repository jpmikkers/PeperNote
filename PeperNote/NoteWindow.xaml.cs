namespace PeperNote;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;


/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class NoteWindow : Window
{
	private const double MaxFontSize = 256.0;

	//private System.Windows.Forms.NotifyIcon notifyIcon = null;
	private readonly DispatcherTimer saveTimer;
	private volatile int noteRevision = 0;
	private volatile int savedRevision = 0;
	private readonly string contentFilename;
	private readonly string settingsFilename;
	private int colorSelect = 1;
	private bool deleted = false;

	private readonly Color[] colors = new Color[]
	{
			Colors.Yellow,
			Colors.Orange,
			Colors.LightBlue,
			Colors.LightCyan,
			Colors.AliceBlue,
			Colors.LightSalmon
	};

	private readonly XmlSerializer settingsSerializer = new XmlSerializer(typeof(NoteSettings));

	public NoteWindow(string settingFile)
	{
		contentFilename = System.IO.Path.ChangeExtension(settingFile, "rtf");
		settingsFilename = System.IO.Path.ChangeExtension(settingFile, "xml");

		saveTimer = new DispatcherTimer(DispatcherPriority.Background, this.Dispatcher);
		saveTimer.Interval = TimeSpan.FromSeconds(1.0);
		saveTimer.Tick += SaveTimer_Tick;

		InitializeComponent();

		stackPanelTool.Visibility = Visibility.Collapsed;
		dockPanelTool.Visibility = Visibility.Hidden;

		LoadNote();
	}

	private void SaveTimer_Tick(object sender, EventArgs e)
	{
		saveTimer.Stop();
		if(savedRevision != noteRevision)
		{
			SaveNote();
		}
	}

	private void Window_MouseEnter(object sender, MouseEventArgs e)
	{
		dockPanelTool.Visibility = Visibility.Visible;
		stackPanelTool.Visibility = Visibility.Visible;
	}

	private void Window_MouseLeave(object sender, MouseEventArgs e)
	{
		stackPanelTool.Visibility = Visibility.Collapsed;
		dockPanelTool.Visibility = Visibility.Hidden;
	}

#if NEVER
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var converter = new ScreenBoundsConverter(this);

            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                Rect bounds = converter.ConvertBounds(screen.Bounds);

                this.Left = bounds.Left;
                this.Top = bounds.Top;
                this.Width = bounds.Width;
                this.Height = bounds.Height;
            }
        }
#endif

	private void CheckBox_Checked(object sender, RoutedEventArgs e)
	{
		ToTop();
		ReviseNow();
	}

	private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
	{
		Topmost = false;
		ReviseNow();
	}

	private void ToTop()
	{
		this.Topmost = true;
		this.Activate();
	}

	private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		//var brush = ((ComboBoxItem)comboBoxColor.SelectedItem).Background;
		//comboBoxColor.Background = brush;
		//this.Background = brush;
	}

	private void cmbColors_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		//Color selectedColor = (Color)(cmbColors.SelectedItem as PropertyInfo).GetValue(null, null);
		//this.Background = new SolidColorBrush(selectedColor);
	}

	private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		try
		{
			DragMove();
		}
		catch
		{
		}
	}

	private void buttonClose_Click(object sender, RoutedEventArgs e)
	{
		if(MessageBox.Show(this, "Permanently delete this note?", "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
		{
			deleted = true;

			if(File.Exists(settingsFilename))
			{
				File.Delete(settingsFilename);
			}

			if(File.Exists(contentFilename))
			{
				File.Delete(contentFilename);
			}
			Close();
		}
	}

	private void Window_GotFocus(object sender, RoutedEventArgs e)
	{
		stackPanelTool.Visibility = Visibility.Visible;
		buttonClose.Visibility = Visibility.Visible;
	}

	private void Window_LostFocus(object sender, RoutedEventArgs e)
	{
		stackPanelTool.Visibility = Visibility.Collapsed;
		buttonClose.Visibility = Visibility.Hidden;
	}

	private void Window_Deactivated(object sender, EventArgs e)
	{
		if(buttonAlwaysVisible.IsChecked ?? false)
		{
			ToTop();
		}
	}

	private static FontWeight GetFontWeight(TextSelection sel)
	{
		return sel.GetPropertyValue(TextElement.FontWeightProperty).CastOrDefault(FontWeights.Normal);
	}

	private static TextDecorationCollection GetTextDecorations(TextSelection sel)
	{
		return sel.GetPropertyValue(Paragraph.TextDecorationsProperty).CastOrDefault<TextDecorationCollection>(null);
	}

	private static TextAlignment GetTextAlignment(TextSelection sel)
	{
		return sel.GetPropertyValue(Block.TextAlignmentProperty).CastOrDefault(TextAlignment.Left);
	}

	private static FontStyle GetFontStyle(TextSelection sel)
	{
		return sel.GetPropertyValue(TextBlock.FontStyleProperty).CastOrDefault(FontStyles.Normal);
	}

	private void richTextBox_SelectionChanged(object sender, RoutedEventArgs e)
	{
		var selection = richTextBox.Selection;

		buttonBold.IsChecked = GetFontWeight(selection) == FontWeights.Bold;
		buttonItalic.IsChecked = GetFontStyle(selection) == FontStyles.Italic;

		var decorations = GetTextDecorations(selection);
		if(decorations != null)
		{
			buttonUnderline.IsChecked = (decorations == TextDecorations.Underline);
			buttonStrikeThrough.IsChecked = (decorations == TextDecorations.Strikethrough);
		}
		else
		{
			buttonUnderline.IsChecked = false;
			buttonStrikeThrough.IsChecked = false;
		}

		var alignment = GetTextAlignment(selection);
		buttonL.IsChecked = alignment == TextAlignment.Left;
		buttonC.IsChecked = alignment == TextAlignment.Center || alignment == TextAlignment.Justify;
		buttonR.IsChecked = alignment == TextAlignment.Right;
	}

	private void buttonNoteColor_Click(object sender, RoutedEventArgs e)
	{
		colorSelect = (colorSelect + 1) % colors.Length;
		this.Background = new SolidColorBrush(colors[colorSelect]);
	}

	private void ButtonLargerFont_Click(object sender, RoutedEventArgs e)
	{
		var cr = richTextBox.Selection;
		var fontSize = cr.GetPropertyValue(TextElement.FontSizeProperty).CastOrDefault(richTextBox.FontSize);
		richTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, (fontSize * 1.1).Clip(richTextBox.FontSize / 4.0, MaxFontSize));
		richTextBox.Focus();
	}

	private void ButtonSmallerFont_Click(object sender, RoutedEventArgs e)
	{
		var cr = richTextBox.Selection;
		var fontSize = cr.GetPropertyValue(TextElement.FontSizeProperty).CastOrDefault(richTextBox.FontSize);
		richTextBox.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, (fontSize / 1.1).Clip(richTextBox.FontSize / 4.0, MaxFontSize));
		richTextBox.Focus();
	}

	private void buttonBold_Click(object sender, RoutedEventArgs e)
	{
		var fontWeight = GetFontWeight(richTextBox.Selection);
		if(fontWeight == FontWeights.Bold)
		{
			richTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
		}
		else
		{
			richTextBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
		}
		richTextBox.Focus();
	}

	private void buttonAlignCenter_Click(object sender, RoutedEventArgs e)
	{
		buttonL.IsChecked = false;
		buttonR.IsChecked = false;

		var sel = richTextBox.Selection;
		sel.ApplyPropertyValue(Block.TextAlignmentProperty, TextAlignment.Center);
		richTextBox.Focus();
	}

	private void buttonAlignLeft_Click(object sender, RoutedEventArgs e)
	{
		buttonC.IsChecked = false;
		buttonR.IsChecked = false;

		var sel = richTextBox.Selection;
		sel.ApplyPropertyValue(Block.TextAlignmentProperty, TextAlignment.Left);
		richTextBox.Focus();
	}

	private void buttonAlignRight_Click(object sender, RoutedEventArgs e)
	{
		buttonL.IsChecked = false;
		buttonC.IsChecked = false;

		var sel = richTextBox.Selection;
		sel.ApplyPropertyValue(Block.TextAlignmentProperty, TextAlignment.Right);
		richTextBox.Focus();
	}

	private void buttonItalic_Click(object sender, RoutedEventArgs e)
	{
		//var sel = richTextBox.Selection;
		//TextRange textRange = new TextRange(richTextBox.Selection.Start, richTextBox.Selection.Start);
		//var indent=(double)textRange.GetPropertyValue(Paragraph.TextIndentProperty);
		//sel.ApplyPropertyValue(Paragraph.TextIndentProperty, indent + richTextBox.FontSize); ;
		//richTextBox.Focus();

		var sel = richTextBox.Selection;
		var fontStyle = GetFontStyle(sel);

		if(fontStyle == FontStyles.Normal)
		{
			sel.ApplyPropertyValue(TextBlock.FontStyleProperty, FontStyles.Italic);
		}
		else
		{
			sel.ApplyPropertyValue(TextBlock.FontStyleProperty, FontStyles.Normal);
		}

		richTextBox.Focus();
	}

	private void buttonUnderline_Click(object sender, RoutedEventArgs e)
	{
		buttonStrikeThrough.IsChecked = false;

		var sel = richTextBox.Selection;
		var dec = GetTextDecorations(sel);

		if(dec == TextDecorations.Underline)
		{
			sel.ApplyPropertyValue(Paragraph.TextDecorationsProperty, null);

		}
		else
		{
			sel.ApplyPropertyValue(Paragraph.TextDecorationsProperty, TextDecorations.Underline);
		}
		richTextBox.Focus();
	}

	private void buttonStrikeThrough_Click(object sender, RoutedEventArgs e)
	{
		buttonUnderline.IsChecked = false;

		var sel = richTextBox.Selection;
		var dec = GetTextDecorations(sel);

		if(dec == TextDecorations.Strikethrough)
		{
			sel.ApplyPropertyValue(Paragraph.TextDecorationsProperty, null);
		}
		else
		{
			sel.ApplyPropertyValue(Paragraph.TextDecorationsProperty, TextDecorations.Strikethrough);
		}
		richTextBox.Focus();
	}

	private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		Revise();
	}

	private void Revise()
	{
		if(saveTimer.IsEnabled) saveTimer.Stop();
		if(savedRevision == noteRevision)
		{
			noteRevision++;
		}
		saveTimer.Start();
	}

	private void ReviseNow()
	{
		noteRevision++;
		if(saveTimer.IsEnabled) saveTimer.Stop();
		SaveNote();
	}

	private void windowNote_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
	{
		ReviseNow();
	}

	private void windowNote_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		ReviseNow();
	}

	private void SaveNote()
	{
		if(deleted) return;

		savedRevision = noteRevision;

		var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);

		using(var fStream = new FileStream(contentFilename, FileMode.Create))
		{
			//range.Save(fStream, DataFormats.XamlPackage);
			range.Save(fStream, DataFormats.Rtf);
		}

		using(var fStream = new FileStream(settingsFilename, FileMode.Create))
		{
			settingsSerializer.Serialize(fStream, new NoteSettings()
			{
				Left = this.Left,
				Top = this.Top,
				Width = this.Width,
				Height = this.Height,
				AlwaysOnTop = Topmost,
				NoteColor = this.Background is SolidColorBrush sb ? sb.Color : Colors.Yellow
			});
		}
	}

	private void LoadNote()
	{
		try
		{
			if(File.Exists(contentFilename))
			{
				var range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
				using(var fStream = File.OpenRead(contentFilename))
				{
					range.Load(fStream, DataFormats.Rtf);
				}
			}
		}
		catch
		{
		}

		try
		{
			if(File.Exists(settingsFilename))
			{
				using(var fStream = File.OpenRead(settingsFilename))
				{
					var settings = (NoteSettings)settingsSerializer.Deserialize(fStream);
					Width = settings.Width;
					Height = settings.Height;
					Left = settings.Left;
					Top = settings.Top;
					Background = new SolidColorBrush(settings.NoteColor);

					if(settings.AlwaysOnTop)
					{
						ToTop();
					}
				}
			}
		}
		catch
		{
		}
	}

	private void buttonNewNote_Click(object sender, RoutedEventArgs e)
	{

	}
}
