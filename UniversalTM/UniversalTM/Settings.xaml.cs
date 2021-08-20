using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UniversalTM
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public int inputshowvalue;
        public Settings()
        {
            InitializeComponent();
        }

        private void ExitSettings(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ApplySettings(object sender, RoutedEventArgs e)
        {
            
            ComboBoxItem theme = (ComboBoxItem)this.theme_box.SelectedItem;
            string str_theme = theme.Content.ToString();
            if (!string.IsNullOrEmpty(str_theme))
            {
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri("Themes\\" + str_theme + ".xaml", UriKind.Relative);
                App.Current.Resources.MergedDictionaries.Add(dict);
            }

            ComboBoxItem simTMFont = (ComboBoxItem)this.TM_font_changer.SelectedItem;
            ComboBoxItem execFont = (ComboBoxItem)this.font_changer.SelectedItem;
            ComboBoxItem tapeFont = (ComboBoxItem)this.Tape_font_changer.SelectedItem;
            if (!string.IsNullOrEmpty(simTMFont.FontFamily.ToString()))
            {
                App.Current.Resources["simTMRes"] = new FontFamily(simTMFont.FontFamily.ToString());
            }
            if (!string.IsNullOrEmpty(execFont.FontFamily.ToString()))
            {
                App.Current.Resources["ExecFontRes"] = new FontFamily(execFont.FontFamily.ToString());
            }
            if (!string.IsNullOrEmpty(tapeFont.FontFamily.ToString()))
            {
                App.Current.Resources["TapeFontRes"] = new FontFamily(tapeFont.FontFamily.ToString());
            }
            string txt_value = this.Num_of_lines.Text;
            if (int.Parse(txt_value) > 0)
            {
                inputshowvalue = int.Parse(txt_value);
            }
            
            

            UpdateLayout();
        }
        public int getValue()
        {
            return this.inputshowvalue;
        }
        private void Reset(object sender, RoutedEventArgs e)
        {

        }
    }
}
